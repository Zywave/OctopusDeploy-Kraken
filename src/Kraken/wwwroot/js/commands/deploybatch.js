define(['cmdr', 'bus', 'services/releaseBatches'], function (cmdr, bus, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'DEPLOYBATCH',
        description: 'Deploys all items in a release batch',
        usage: 'DEPLOYBATCH batchId environmentIdOrName',
        main: function (batchId, environmentIdOrName) {
            if (!batchId || !environmentIdOrName) {
                this.shell.writeLine('Batch id and environment id or name required', 'error');
                return;
            }
            return releaseBatchesService.deployReleaseBatch(batchId, environmentIdOrName).then(function (data) {
                bus.publish('releasebatches:deploy', batchId);
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