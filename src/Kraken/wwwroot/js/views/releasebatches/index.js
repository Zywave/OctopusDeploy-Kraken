ko.components.register('views/releasebatches/index', {
    viewModel: function (params) {
        
        this.releaseBatches = ko.observableArray();

        var mapReleaseBatch = function(batch) {
            batch.viewUrl = app.basePath + 'app/releasebatches/details?id=' + batch.id;
            return batch;
        }

        var loadReleaseBatches = function() {
            $.get(app.basePath + 'api/releasebatches').then(function(data) {
                this.releaseBatches($.map(data, mapReleaseBatch));
            }.bind(this));
        }.bind(this);

        $(document).on('releasebatches.add releasebatches.update releasebatches.delete', function (data) {
            loadReleaseBatches();
        }.bind(this));
        
        loadReleaseBatches();
    },
    template: { html: true }
});