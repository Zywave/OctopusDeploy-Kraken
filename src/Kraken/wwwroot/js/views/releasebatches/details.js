define(['knockout', 'bootstrap', 'select2', 'koselect2', 'moment', 'utils/koAsyncExtender', 'shell', 'bus', 'services/releaseBatches', 'services/environments', 'services/projects', 'services/users', 'context'], function (ko, bs, select2, koselect2, moment, koAsyncExtender, shell, bus, releaseBatchesService, environmentsService, projectsService, usersService, context) {
    return function(params) {
        
        this.environments = ko.observableArray();
        this.selectedProject = ko.observable();
        this.projectsSelect2Options = ko.observable({
            width: 'off',
            placeholder: 'Link project...',
            ajax: {
                url: context.basePath + 'api/projects',
                data: function(params) {
                    return {
                        nameFilter: params.term
                    };
                },
                dataType: 'json',
                processResults: function(data) {
                    data = $.map(data, function(obj) {
                        obj.text = obj.text || obj.name;
                        return obj;
                    });
                    return {
                        results: data
                    };
                },
                minimumInputLength: 1
            },
            theme: 'bootstrap'
        });

        this.releaseBatch = ko.observable();        
        var releaseBatch = this.releaseBatch;

        this.loadReleaseBatch = function() {
            releaseBatchesService.getReleaseBatch(params.id).then(function (data) {
                data.logoUrl = context.basePath + 'api/releasebatches/' + data.id + '/logo?cb=' + new Date(data.updateDateTime).getTime();
                this.releaseBatch(data);
            }.bind(this));
        }.bind(this);

        this.loadEnvironments = function() {
            environmentsService.getEnvironments('DeploymentCreate').then(function(data) {
                this.environments(data);
            }.bind(this));
        }.bind(this);

        this.deploy = function (environment) {
            shell.open();
            shell.execute('DEPLOYBATCH', params.id, environment.id).then(function () {
                this.loadReleaseBatch();
            }.bind(this));
        }.bind(this);

        this.syncEnvironment = function (environment) {
            shell.open();
            shell.execute('SYNCBATCH', params.id, environment.id).then(function () {
                this.loadReleaseBatch();
            }.bind(this));
        }.bind(this);

        this.syncReleases = function () {
            shell.open();
            shell.execute('SYNCBATCH', params.id).then(function () {
                this.loadReleaseBatch();
            }.bind(this));
        }.bind(this);

        this.syncPackages = function () {
            shell.open();
            shell.execute('NUBATCH', params.id).then(function () {
                this.loadReleaseBatch();
            }.bind(this));
        }.bind(this);

        this.linkProject = function () {
            shell.execute('LINKPROJ', params.id, this.selectedProject()).then(function () {
                this.loadReleaseBatch();
            }.bind(this), function () {
                shell.open();
            }.bind(this));
        }.bind(this);

        var displayNames = {};
        function getUserDisplayName(userName) {
            if (!userName) return null;
            if (!displayNames.hasOwnProperty(userName)) {
                displayNames[userName] = ko.computed(function () {
                    return usersService.getUser(userName).then(function (user) {
                        return user.displayName || user.userName;
                    });
                }).extend({ async: userName });
            }
            return displayNames[userName]();
        }

        this.getAuditInfo = function (action, userName, dateTime, environmentName) {
            var info = '';
            info += 'Last ' + action;
            if (environmentName) {
                info += ' to ' + environmentName;
            }
            info += ' by ' + getUserDisplayName(userName);
            info += ' at ' + moment(dateTime).format('l LTS');
            return info;
        }.bind(this);

        this.getProjectUrl = function (item) {
            return context.octopusServerAddress + '/app#/projects/' + item.projectSlug;
        }.bind(this);

        this.getReleaseUrl = function (item) {
            return this.getProjectUrl(item) + '/releases/' + item.releaseVersion;
        }.bind(this);
        
        this.manage = function () {
            shell.open();
        }.bind(this);
        
        bus.subscribe('releasebatches:update', function (idOrName) {
            if (releaseBatch().id === idOrName || releaseBatch().name === idOrName) {
                this.loadReleaseBatch();
            }
        }.bind(this));
        bus.subscribe('releasebatches:delete', function (idOrName) {
            if (releaseBatch().id === idOrName || releaseBatch().name === idOrName) {
                document.location = context.basePath + 'app';
            }
        }.bind(this));

        this.loadReleaseBatch();
        this.loadEnvironments();
    };
});