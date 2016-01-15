define([
    'cmdr',
    'commands/env',
    'commands/proj',
    'commands/batch',
    'commands/mkbatch',
    'commands/renbatch',
    'commands/rmbatch',
    'commands/linkproj',
    'commands/unlinkproj'
],
function (cmdr) {
    var provider = new cmdr.DefinitionProvider();

    for (var i = 1, l = arguments.length; i < l; i++) {
        provider.addDefinition(arguments[i]);
    }
    
    return provider;
});