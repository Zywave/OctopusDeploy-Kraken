define(['cmdr', 'services/projects'], function (cmdr, projectsService) {

    return new cmdr.Definition({
        name: 'PROJ',
        description: 'List Octopus projects, optionally filtered by name.',
        usage: 'PROJ [nameFilter]',
        main: function (nameFilter) {
            return projectsService.getProjects(nameFilter).then(function (data) {
                this.shell.writeLine('Octopus projects');
                this.shell.writeLine();
                this.shell.writeTable(data, ['id:20', 'slug:50', 'name:*'], true);
            }.bind(this)).fail(this.fail.bind(this));
        }
    });

});