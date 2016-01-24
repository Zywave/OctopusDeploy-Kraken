define(['cmdr', 'services/releaseBatches'], function (cmdr, releaseBatchesService) {

    return new cmdr.Definition({
        name: 'NUBATCH',
        description: 'Makes a new release for every project in a batch from the latest nuget packages.',
        usage: 'NUBATCH batchIdOrName [version]',
        main: function (batchIdOrName, version) {
            if (!batchIdOrName) {
                this.shell.writeLine('Batch id or name required', 'error');
                return;
            }

            var deferred = this.defer();

            // TODO clean up
            releaseBatchesService.getNextReleases(batchIdOrName).then(function (releases) {
                var release;
                var versionedReleases = [];
                var unversionedReleases = [];
                while (true) {
                    if (!releases.length) {
                        break;
                    }
                    release = releases.pop();
                    if (release.version) {
                        versionedReleases.push(release);
                    } else {
                        unversionedReleases.push(release);
                    }
                }
                if (unversionedReleases.length) {
                    release = unversionedReleases.pop();
                    this.shell.writeLine('Could not determine a version for project \'' + release.projectId + '\'. Please enter a version.');
                    this.shell.readLine(function (value) {
                        while (true) {
                            release.version = value;
                            versionedReleases.push(release);
                            if (!unversionedReleases.length) {
                                return releaseBatchesService.createReleases(batchIdOrName, versionedReleases).then(function (data) {
                                    this.shell.writeLine('Releases created and batch synced with latest', 'success');
                                    deferred.resolve();
                                }.bind(this)).fail(this.fail.bind(this));
                            }
                            release = unversionedReleases.pop();
                            this.shell.writeLine('Could not determine a version for project \'' + release.projectId + '\'. Please enter a version.');
                        }
                    }.bind(this));
                } else {
                    releaseBatchesService.createReleases(batchIdOrName, versionedReleases).then(function (data) {
                        this.shell.writeLine('Releases created and batch synced with latest', 'success');
                        deferred.resolve();
                    }.bind(this)).fail(this.fail.bind(this));
                }
            }.bind(this)).fail(this.fail.bind(this));

            return deferred;
            
        },
        autocompleteKeys: ['releaseBatches']
    });

});