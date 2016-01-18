define(['knockout', 'bootstrap', 'shell', 'services/releaseBatches', 'services/environments'], function(ko, bs, shell, releaseBatchesService, environmentsService) {
    return function(params) {

        this.releaseBatch = ko.observable();
        this.environments = ko.observableArray();

        var releaseBatch = this.releaseBatch;

        this.loadReleaseBatch = function() {
            releaseBatchesService.getReleaseBatch(params.id).then(function(data) {
                this.releaseBatch(data);
            }.bind(this));
        }.bind(this);

        this.loadEnvironments = function() {
            environmentsService.getEnvironments().then(function(data) {
                this.environments(data);
            }.bind(this));
        }.bind(this);

        this.deploy = function (environment) {
            shell.execute('DEPLOYBATCH', params.id, environment.id);
        }.bind(this);

        this.sync = function (environment) {
            shell.execute('SYNCBATCH', params.id, environment.id).then(function (data) {
                releaseBatch(data);
            });
        }.bind(this);

        this.loadReleaseBatch();
        this.loadEnvironments();
    };
});