ko.components.register('views/projectbatches/index', {
    viewModel: function (params) {
        
        this.projectBatches = ko.observableArray();

        var mapProjectBatch = function(batch) {
            batch.viewUrl = app.basePath + 'app/projectbatches/details?id=' + batch.id;
            return batch;
        }

        var loadProjectBatches = function() {
            $.get(app.basePath + 'api/projectbatches').then(function(data) {
                this.projectBatches($.map(data, mapProjectBatch));
            }.bind(this));
        }.bind(this);

        $(document).on('projectbatches.add projectbatches.delete', function (data) {
            loadProjectBatches();
        }.bind(this));
        
        loadProjectBatches();
    },
    template: { html: true }
});