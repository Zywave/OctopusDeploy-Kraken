define(['cmdr', 'commands/definitionProvider', 'commands/autocompleteProvider', 'commands/commandHandler'], function (cmdr, definitionProvider, autocompleteProvider, commandHandler) {

    var shell = new cmdr.OverlayShell({
        definitionProvider: definitionProvider,
        autocompleteProvider: autocompleteProvider,
        commandHandler: commandHandler
    });

    shell.writeLine('Welcome to the Kraken console!');
    shell.writeLine();
    shell.echo = false;
    shell.execute('help');
    shell.echo = true;
    shell.writeLine('* Press (Tab) key to autocomplete commands or arguments.');
    shell.writeLine('* Press (`/~) key to open the console.');
    shell.writeLine('* Press (Esc) key to close the console.');

    return shell;
});