define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'DEPLOYBATCH',
        description: 'Deploys all items in a release batch.',
        usage: 'DEPLOYBATCH batchIdOrName environmentIdOrName [--force]\n\nEnvironment id can be full (Environments-123) or short (123)',
        main: function (batchIdOrName, environmentIdOrName, force) {
            if (!batchIdOrName || !environmentIdOrName) {
                this.shell.writeLine('Batch id or name and environment id or name required', 'error');
                return;
            }
            
            return releaseBatchesService.deployReleaseBatch(batchIdOrName, environmentIdOrName, force === '--force').then(function (data) {
                this.shell.writeLine();
                this.shell.writeLine('                $$$$$$$');
                this.shell.writeLine('            $$$$$$$$$$$$$$');
                this.shell.writeLine('         $$$$$$$$$$$$$$$$$$');
                this.shell.writeLine('        $$$$$$$$$$$$$$$$$$$$');
                this.shell.writeLine('       $$$$$$$$$$$$$$$O$$$$$    $$$$$$');
                this.shell.writeLine('       $$$$$$$$O$$$$$$$$$$$$   $$$  $$$');
                this.shell.writeLine('       $$$$$$$$$$$$$$$$$$$$  $$$$    $$');
                this.shell.writeLine('        $$$$$$$$$$$$$$$$$$$$$$$$');
                this.shell.writeLine('         $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$');
                this.shell.writeLine(' $$$$$$     $$$$$$$$$$$$$$$$$$        $$');
                this.shell.writeLine('$$   $$$$$$$$$$$$$$$$$$$$$$$         $$');
                this.shell.writeLine(' $$$    $$$$$$$$$$$$$$$$$$$$$$$$$$$$');
                this.shell.writeLine('     $$$$$$$$  $$$ $$$$$$          $$');
                this.shell.writeLine('   $$$$       $$$  $$$ $$$      $$$$');
                this.shell.writeLine('  $$$       $$$$   $$$  $$$');
                this.shell.writeLine('   $$$$$   $$$     $$$   $$$$    $$');
                this.shell.writeLine('    $$$    $$$$$$  $$$    $$$$$$$$');
                this.shell.writeLine('            $$$     $$$$$   $$$$');
                this.shell.writeLine();
                this.shell.writeLine('         RELEASE THE KRAKEN!');
                this.shell.writeLine();
                this.shell.writeLine('Release batch deploys started.  Check Octopus for status.', 'success');
                return data;
            }.bind(this)).fail(this.fail.bind(this));
        },
        autocompleteKeys: ['releaseBatches', 'environments']
    });

});