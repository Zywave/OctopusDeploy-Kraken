define(['cmdr', 'commands/definitionProvider', 'commands/autocompleteProvider', 'commands/commandHandler'], function (cmdr, definitionProvider, autocompleteProvider, commandHandler) {

    return new cmdr.OverlayShell({
        definitionProvider: definitionProvider,
        autocompleteProvider: autocompleteProvider,
        commandHandler: commandHandler
    });

});