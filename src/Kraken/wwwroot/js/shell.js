define(['cmdr', 'commands/definitionProvider'], function(cmdr, definitionProvider) {

    return new cmdr.OverlayShell({
        definitionProvider: definitionProvider
    });

});