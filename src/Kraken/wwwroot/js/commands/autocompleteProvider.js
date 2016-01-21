define(['cmdr', 'bus', 'commands/definitionProvider', 'services/releaseBatches', 'services/projects', 'services/environments'], function(cmdr, bus, definitionProvider, releaseBatchesService, projectsService, environmentsService) {

    function shouldLookup(key, context) {
        if (!context.parsed.name) return false;
        
        var definitions = definitionProvider.getDefinitions(context.parsed.name);
        if (definitions.length !== 1) return false;

        var autocompleteKeys = definitions[0].autocompleteKeys;
        if (!autocompleteKeys) return false;

        return autocompleteKeys[context.parsed.args.length] === key;
    }

    function mapNames(data) {
        return data.map(function (item) {
            var name = item.name;
            if (name.indexOf(' ') > -1) {
                name = '"' + name + '"';
            }
            return name;
        });
    }

    var releaseBatchNames = null;
    function lookupReleaseBatches(context) {
        if (shouldLookup('releaseBatches', context)) {
            if (!releaseBatchNames) {
                releaseBatchNames = releaseBatchesService.getReleaseBatches().then(mapNames);
            }
            return releaseBatchNames;
        }
        return null;
    }

    function clearReleaseBatchNames() {
        releaseBatchNames = null;
    }

    bus.subscribe('releasebatches:add', clearReleaseBatchNames);
    bus.subscribe('releasebatches:update', clearReleaseBatchNames);
    bus.subscribe('releasebatches:delete', clearReleaseBatchNames);

    var projectNames = null;
    function lookupProjects(context) {
        if (shouldLookup('projects', context)) {
            if (!projectNames) {
                projectNames = projectsService.getProjects().then(mapNames);
            }
            return projectNames;
        }
        return null;
    }

    var environmentNames = null;
    function lookupEnvironments(context) {
        if (shouldLookup('environments', context)) {
            if (!environmentNames) {
                environmentNames = environmentsService.getEnvironments().then(mapNames);
            }
            return environmentNames;
        }
        return null;
    }

    var provider = new cmdr.AutocompleteProvider();
    provider.lookups.push(lookupReleaseBatches);
    provider.lookups.push(lookupProjects);
    provider.lookups.push(lookupEnvironments);
    return provider;

});