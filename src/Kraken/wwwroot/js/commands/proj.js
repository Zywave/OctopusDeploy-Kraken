define(['cmdr', 'services/projects'], function(cmdr, projectsService) {

    return new cmdr.Definition({
        name: 'PROJ',
        description: 'List all Octopus projects.',
        main: function () {
            return projectsService.getProjects().then(function (data) {
                this.shell.writeLine('Octopus projects');
                this.shell.writeLine();
                this.shell.writeTable(data, ['id:20', 'slug:50', 'name:*'], true);
            }.bind(this)).fail(function (xhr, error, message) {
                this.shell.writeLine(message, 'error');
                if (xhr.responseText) {
                    this.shell.writeLine(xhr.responseText, 'error');
                }
                this.shell.writeLine('Operation Failed', 'error');
            }.bind(this));
        }
    });

});