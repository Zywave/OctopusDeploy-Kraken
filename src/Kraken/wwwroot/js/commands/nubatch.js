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
            var versionedReleases = [];
            var unversionedReleases = [];

            var getVersionForRelease = function (release) {
                this.shell.write('Could not determine a version for project \'' + release.projectId + '\'. Please enter a release version: ');
                this.shell.readLine(function (value) {
                    release.version = value;
                    versionedReleases.push(release);
                    if (!unversionedReleases.length) {
                        return releaseBatchesService.createReleases(batchIdOrName, versionedReleases).then(function (data) {
                            this.shell.writeLine('Releases created and batch synced with latest', 'success');
                            deferred.resolve();
                        }.bind(this)).fail(this.fail.bind(this));
                    }
                    getVersionForRelease(unversionedReleases.pop());
                }.bind(this));
            }.bind(this);

            var checkVersionedOrUnversioned = function(releases) {
                if (!releases.length) {
                    return;
                }
                var release = releases.pop();
                if (release.version) {
                    versionedReleases.push(release);
                } else {
                    unversionedReleases.push(release);
                }
                checkVersionedOrUnversioned(releases);
            }

            releaseBatchesService.previewReleases(batchIdOrName).then(function (releases) {
                checkVersionedOrUnversioned(releases);
                if (unversionedReleases.length) {
                    getVersionForRelease(unversionedReleases.pop());
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