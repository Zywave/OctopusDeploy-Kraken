(function(app, cmdr, $) {

    var shell = new cmdr.OverlayShell();
    
    shell.define({
        name: 'BATCH',
        description: 'Lists all release batches.',
        main: function() {
            return $.get(app.basePath + 'api/releasebatches').then(function(data) {
                this.shell.writeLine("Project batches:");
                this.shell.writeLine();
                this.shell.writeTable(data, ['id:10', 'name:*'], true);
            }.bind(this)).fail(function(xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

    shell.define({
        name: 'MKBATCH',
        description: 'Creates a new release batch.',
        usage: 'MKBATCH name',
        main: function (name) {
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

    shell.define({
        name: 'RENBATCH',
        description: 'Renames a release batch.',
        usage: 'RENBATCH id name',
        main: function (id, name) {
            if (!id || !name) {
                this.shell.writeLine('Project batch id and name required', 'error');
                return;
            }
            return $.ajax({
                type: 'PUT',
                url: app.basePath + 'api/releasebatches/' + id,
                data: JSON.stringify({ id: id, name: name }),
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

    shell.define({
        name: 'RMBATCH',
        description: 'Deletes a release batch by id.',
        usage: 'RMBATCH id',
        main: function (id) {
            if (!id) {
                this.shell.writeLine('Project batch id required', 'error');
                return;
            }
            var deferred = this.defer();
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
            return deferred;
        }
    });

    shell.define({
        name: 'ENV',
        description: 'Lists all Octopus environments.',
        main: function () {
            return $.get(app.basePath + 'api/environments').then(function(data) {
                this.shell.writeLine('Octopus environments:');
                this.shell.writeLine();
                this.shell.writeTable(data, ['id:20', 'name:*'], true);
            }.bind(this)).fail(function(xhr, error, message) {
                this.shell.writeLine(message, 'error');
                this.shell.writeLine('Operation failed', 'error');
            }.bind(this));
        }
    });

    shell.define({
        name: 'LOGOUT',
        description: 'Logout of Kraken.',
        main: function () {
            var deferred = this.defer();
            this.shell.write('Are you sure you want to logout of Kraken? ', 'warning');
            this.shell.readLine(function(value) {
                if (value.toLowerCase() !== 'y') {
                    deferred.resolve();
                }
                document.location = app.basePath + 'logout';
                deferred.resolve();
            }.bind(this));
            return deferred;
        }
    });

    define({
        name: 'PROJECTS',
        description: 'List all Octopus projects',
        callback: function () {
            return $.get(app.basePath + 'api/projects').then(function (data) {
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