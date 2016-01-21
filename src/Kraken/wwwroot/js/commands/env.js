define(['cmdr', 'services/environments'], function (cmdr, environmentsService) {

    return new cmdr.Definition({
        name: 'ENV',
        description: 'Lists all Octopus environments.',
        main: function () {
            var helper = new commandHelper(this);
            return environmentsService.getEnvironments().then(function (data) {
                this.shell.writeLine('Octopus environments:');
                this.shell.writeLine();
                this.shell.writeTable(data, ['id:20', 'name:*'], true);
            }.bind(this)).fail(helper.fail);
        }
    });

});