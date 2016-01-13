ko.components.register('views/releasebatches/details', {
    viewModel: function (params) {

        this.releaseBatch = ko.observable();
        this.environments = ko.observableArray();

        this.loadReleaseBatch = function () {
            $.get(app.basePath + 'api/releasebatches/' + params.id).then(function (data) {
                this.releaseBatch(data);
            }.bind(this));
        }.bind(this);
        
        this.loadEnvironments = function () {
            $.get(app.basePath + 'api/environments').then(function (data) {
                this.environments(data);
            }.bind(this));
        }.bind(this);

        this.loadReleaseBatch();
        this.loadEnvironments();

    },
    template: { html: true }
});