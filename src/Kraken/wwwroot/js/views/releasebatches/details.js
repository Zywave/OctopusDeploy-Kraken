define(['knockout', 'bootstrap', 'select2', 'koselect2', 'moment', 'underscore', 'utils/koAsyncExtender', 'shell', 'bus', 'services/releaseBatches', 'services/environments', 'services/projects', 'services/users', 'context'], function (ko, bs, select2, koselect2, moment, _, koAsyncExtender, shell, bus, releaseBatchesService, environmentsService, projectsService, usersService, context) {
    return function (params) {

        this.viewEnvironments = ko.observableArray();
        this.deployEnvironments = ko.observableArray();
        this.progress = ko.observable();
        this.selectedProject = ko.observable();
        this.releaseBatch = ko.observable();
        var releaseBatch = this.releaseBatch;
        var checkProgressIntervalId;

        this.projectsSelect2Options = ko.observable({
            width: 'off',
            placeholder: 'Link project...',
            ajax: {
                url: context.basePath + 'api/projects',
                data: function (params) {
                    return {
                        nameFilter: params.term
                    };
                },
                dataType: 'json',
                processResults: function (data) {
                    data = $.map(data, function (obj) {
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

        this.loadReleaseBatch = function () {
            releaseBatchesService.getReleaseBatch(params.id).then(function (data) {
                data.logoUrl = context.basePath + 'api/releasebatches/' + data.id + '/logo?cb=' + new Date(data.updateDateTime).getTime();
                this.releaseBatch(data);
            }.bind(this));
        }.bind(this);

        this.loadEnvironments = function () {
            environmentsService.getEnvironments('DeploymentCreate').then(function (data) {
                this.deployEnvironments(data);
            }.bind(this));
            environmentsService.getEnvironments('EnvironmentView').then(function (data) {
                this.viewEnvironments(data);
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

        this.checkProgress = function () {
            clearInterval(checkProgressIntervalId);

            releaseBatchesService.getProgression(params.id, _.pluck(this.viewEnvironments(), 'id')).then(function (data) {
                this.progress(data);
                // check progress again in 5 seconds
                checkProgressIntervalId = setInterval(function () {
                    this.checkProgress();
                }.bind(this), 5000);
            }.bind(this)).fail(function(e) {
                console.error(e);
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

        this.getDeploymentUrl = function (item, environment) {
            var progress = this.getProgressDataFromProgression(item, environment);
            if (progress) {
                return this.getProjectUrl(item) + '/releases/' + progress.releaseId + '/deployments/' + progress.deploymentId;
            }
        }

        this.getProgressSuccessOrFailedOrExecuting = function(progress) {
            var state = progress.state;
            if (state === 6) { // 'Success'
                return 'success';
            } else if (state === 1 || state === 4 || state === 7) { // 'Canceled' || 'Failed' || 'TimedOut'
                return 'failed';
            } else { // 'Cancelling' || 'Executing' || 'Queued'
                return 'executing';
            }
        }

        this.formatDate = function (date) {
            if (date) {
                return moment(date).format('l LTS');
            }
        }

        this.getProgressDataFromProgression = function (item, environment) {
            if (this.progress()) {
                var progress = _.find(this.progress(), function (p) {
                    return p.projectId === item.projectId && p.environmentId === environment.id;
                });
                return progress;
            }
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
        this.checkProgress();
    };
});
