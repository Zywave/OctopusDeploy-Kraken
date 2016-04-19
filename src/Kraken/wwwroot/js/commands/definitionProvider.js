define([
    'cmdr',
    'commands/env',
    'commands/proj',
    'commands/batch',
    'commands/mkbatch',
    'commands/updbatch',
    'commands/rmbatch',
    'commands/linkproj',
    'commands/unlinkproj',
    'commands/syncbatch',
    'commands/deploybatch',
    'commands/nubatch',
    'commands/progress',
    'commands/lockbatch',
    'commands/unlockbatch',
    'commands/logout',
],
function (cmdr) {
    var provider = new cmdr.DefinitionProvider();

    for (var i = 1, l = arguments.length; i < l; i++) {
        provider.addDefinition(arguments[i]);
    }
    
    return provider;
});