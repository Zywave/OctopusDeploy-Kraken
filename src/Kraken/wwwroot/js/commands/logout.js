define(['cmdr', 'context'], function(cmdr, context) {

    return new cmdr.Definition({
        name: 'LOGOUT',
        description: 'Logout of Kraken.',
        main: function () {
            var deferred = this.defer();
            this.shell.write('Are you sure you want to logout of Kraken? ', 'warning');
            this.shell.readLine(function (value) {
                if (value.toLowerCase() !== 'y') {
                    deferred.resolve();
                    return;
                }
                document.location = context.basePath + 'logout';
                deferred.resolve();
            }.bind(this));
            return deferred;
        }
    });

});