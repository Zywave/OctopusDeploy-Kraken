ko.components.register('views/projectbatches/details', {
    viewModel: function (params) {

        this.projectBatch = ko.observable();
        this.environments = ko.observableArray();

        this.loadProjectBatch = function () {
            $.get(app.basePath + 'api/projectbatches/' + params.id).then(function (data) {
                this.projectBatch(data);
            }.bind(this));
        }.bind(this);
        
        this.loadEnvironments = function () {
            $.get(app.basePath + 'api/environments').then(function (data) {
                this.environments(data);
            }.bind(this));
        }.bind(this);

        this.loadProjectBatch();
        this.loadEnvironments();

    },
    template: { html: true }
});