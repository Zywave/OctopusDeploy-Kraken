(function (app, cmdr, $) {

    var shell = new cmdr.OverlayShell();

    function define(definition) {
        shell.definitionProvider.addDefinition(new cmdr.Definition(definition));
    }

    define({
        name: 'batch',
        description: 'Lists all of the project batches',
        callback: function () {
            return $.get(app.basePath + 'api/projectbatches').then(function (data) {
                this.shell.writeLine("Project batches");
                this.shell.writeLine();
                this.shell.writePad('id', ' ', 10);
                this.shell.writeLine('name');
                this.shell.writePad('--', ' ', 10);
                this.shell.writeLine('----');
                $.each(data, function (index, item) {
                    this.shell.writePad(item.id.toString(), ' ', 10);
                    this.shell.writeLine(item.name);
                }.bind(this));
                this.shell.writeLine();
            }.bind(this)).fail(function (xhr, error) {
                this.shell.writeLine(error, 'error');
                this.shell.writeLine('Project batch operation failed', 'error');
            }.bind(this));
        }
    });

    define({
        name: 'mkbatch',
        description: 'Creates a new project batch (e.g. mkbatch "My Batch")',
        callback: function (name) {
            if (!name) {
                this.shell.writeLine('Project batch name required', 'error');
                return;
            }
            return $.ajax({
                type: 'POST',
                url: app.basePath + 'api/projectbatches',
                data: JSON.stringify({ name: name }),
                contentType: 'application/json'
            }).then(function () {
                $(document).trigger('projectbatches.add');
                this.shell.writeLine('Project batch created', 'success');
            }.bind(this)).fail(function (xhr, error) {
                this.shell.writeLine(error, 'error');
                this.shell.writeLine('Project batch operation failed', 'error');
            }.bind(this));
        }
    });

    define({
        name: 'rmbatch',
        description: 'Deletes a project batch by id (e.g. rmbatch 123)',
        callback: function (id) {
            if (!id) {
                this.shell.writeLine('Project batch id required', 'error');
                return;
            }
            var deferred = $.Deferred();
            this.shell.write('Are you sure you want to delete this project batch (y/n)? ', 'warning');
            this.shell.readLine(function (value) {
                if (value.toLowerCase() !== 'y') {
                    deferred.resolve();
                }
                $.ajax({
                    url: app.basePath + 'api/projectbatches/' + id,
                    type: 'DELETE'
                }).then(function () {
                    $(document).trigger('projectbatches.delete');
                    this.shell.writeLine('Project batch deleted', 'success');
                    deferred.resolve();
                }.bind(this)).fail(function (xhr, error) {
                    this.shell.writeLine(error, 'error');
                    this.shell.writeLine('Project batch operation failed', 'error');
                    deferred.resolve();
                }.bind(this));
            }.bind(this));
            return deferred.promise();
        }
    });
    
    app.shell = shell;

})(window.app, window.cmdr, window.jQuery)