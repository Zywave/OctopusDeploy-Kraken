(function(app, cmdr, $) {

    var shell = new cmdr.OverlayShell();

    function define(definition) {
        shell.definitionProvider.addDefinition(new cmdr.Definition(definition));
    }

    define({
        name: 'batch',
        description: 'Lists all release batches',
        callback: function() {
            return $.get(app.basePath + 'api/releasebatches').then(function(data) {
                this.shell.writeLine("Project batches");
                this.shell.writeLine();
                this.shell.writePad('id', ' ', 10);
                this.shell.writeLine('name');
                this.shell.writePad('--', ' ', 10);
                this.shell.writeLine('----');
                $.each(data, function(index, item) {
                    this.shell.writePad(item.id.toString(), ' ', 10);
                    this.shell.writeLine(item.name);
                }.bind(this));
                this.shell.writeLine();
            }.bind(this)).fail(function(xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

    define({
        name: 'mkbatch',
        description: 'Creates a new release batch (e.g. mkbatch "My Batch")',
        callback: function(name) {
            if (!name) {
                this.shell.writeLine('Project batch name required', 'error');
                return;
            }
            return $.ajax({
                type: 'POST',
                url: app.basePath + 'api/releasebatches',
                data: JSON.stringify({ name: name }),
                contentType: 'application/json'
            }).then(function(data) {
                $(document).trigger('releasebatches.add', data.id);
                this.shell.writeLine('Project batch created', 'success');
            }.bind(this)).fail(function(xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

    define({
        name: 'renbatch',
        description: 'Renames a release batch (e.g. renbatch 123 "My Other Batch")',
        callback: function (id, name) {
            if (!id || !name) {
                this.shell.writeLine('Project batch id and name required', 'error');
                return;
            }
            return $.ajax({
                type: 'PUT',
                url: app.basePath + 'api/releasebatches/' + id,
                data: JSON.stringify({ name: name }),
                contentType: 'application/json'
            }).then(function () {
                $(document).trigger('releasebatches.update', id);
                this.shell.writeLine('Project batch updated', 'success');
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

    define({
        name: 'rmbatch',
        description: 'Deletes a release batch by id (e.g. rmbatch 123)',
        callback: function(id) {
            if (!id) {
                this.shell.writeLine('Project batch id required', 'error');
                return;
            }
            var deferred = $.Deferred();
            this.shell.write('Are you sure you want to delete this release batch (y/n)? ', 'warning');
            this.shell.readLine(function(value) {
                if (value.toLowerCase() !== 'y') {
                    deferred.resolve();
                }
                $.ajax({
                    url: app.basePath + 'api/releasebatches/' + id,
                    type: 'DELETE'
                }).then(function() {
                    $(document).trigger('releasebatches.delete', id);
                    this.shell.writeLine('Project batch deleted', 'success');
                    deferred.resolve();
                }.bind(this)).fail(function(xhr, error, message) {
                    this.shell.writeLine(message, 'error');
                    this.shell.writeLine('Operation failed', 'error');
                    deferred.resolve();
                }.bind(this));
            }.bind(this));
            return deferred.promise();
        }
    });

    define({
        name: 'env',
        description: 'Lists all Octopus environments',
        callback: function() {
            return $.get(app.basePath + 'api/environments').then(function(data) {
                this.shell.writeLine('Octopus environments');
                this.shell.writeLine();
                this.shell.writePad('id', ' ', 20);
                this.shell.writeLine('name');
                this.shell.writePad('--', ' ', 20);
                this.shell.writeLine('----');
                $.each(data, function(index, item) {
                    this.shell.writePad(item.id.toString(), ' ', 20);
                    this.shell.writeLine(item.name);
                }.bind(this));
                this.shell.writeLine();
            }.bind(this)).fail(function(xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

    define({
        name: 'logout',
        description: 'Logout of Kraken',
        callback: function() {
            var deferred = $.Deferred();
            this.shell.write('Are you sure you want to logout of Kraken? ', 'warning');
            this.shell.readLine(function(value) {
                if (value.toLowerCase() !== 'y') {
                    deferred.resolve();
                }
                document.location = app.basePath + 'logout';
                deferred.resolve();
            }.bind(this));

            return deferred.promise();
        }
    });

    define({
        name: 'listproj',
        description: 'List project slugs and names in Octopus',
        callback: function () {
            return $.get(app.basePath + 'api/octopus/GetProjects').then(function (data) {
                this.shell.writeLine('Octopus projects');
                this.shell.writeLine();
                this.shell.writePad('slug', ' ', 20);
                this.shell.writeLine('name');
                this.shell.writePad('--', ' ', 20);
                this.shell.writeLine('----');
                $.each(data, function (index, item) {
                    this.shell.writePad(item.slug.toString(), ' ', 20);
                    this.shell.writeLine(item.name);
                }.bind(this));
                this.shell.writeLine();
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    })

    app.shell = shell;

})(window.app, window.cmdr, window.jQuery);