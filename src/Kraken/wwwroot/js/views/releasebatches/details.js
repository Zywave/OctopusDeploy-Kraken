define(['knockout', 'bootstrap', 'services/releaseBatches', 'services/environments'], function(ko, bs, releaseBatchesService, environmentsService) {
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

        this.deploy = function(environment) {
            releaseBatchesService.deployReleaseBatch(params.id, environment.id).then(function () {
                alert('Deployed');
            });
        }.bind(this);

        this.sync = function (environment) {
            releaseBatchesService.syncReleaseBatch(params.id, environment.id).then(function (data) {
                releaseBatch(data);
            });
        }.bind(this);

        this.loadReleaseBatch();
        this.loadEnvironments();
    };
});