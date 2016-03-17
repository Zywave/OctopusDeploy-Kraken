### Bug Fixes

* deployments were linking to first environment deployments only ([2b7651e](https://github.com/zywave/OctopusDeploy-Kraken/commit/2b7651e))
* improved deploybatch api ([a9f585c](https://github.com/zywave/OctopusDeploy-Kraken/commit/a9f585c))
* kraken now shows environments specific to users permissions ([772f6a2](https://github.com/zywave/OctopusDeploy-Kraken/commit/772f6a2))
* model state checks for all api methods ([bd04c4f](https://github.com/zywave/OctopusDeploy-Kraken/commit/bd04c4f))
* progression style cleanup ([5569466](https://github.com/zywave/OctopusDeploy-Kraken/commit/5569466))
* stop checking progress every 5 seconds if kraken encounters an error ([0c78936](https://github.com/zywave/OctopusDeploy-Kraken/commit/0c78936))

### Features

* allow batch logo update via updbatch command ([fa52898](https://github.com/zywave/OctopusDeploy-Kraken/commit/fa52898)), closes [#14](https://github.com/zywave/OctopusDeploy-Kraken/issues/14)
* get releaseId for release batch progress ([1639696](https://github.com/zywave/OctopusDeploy-Kraken/commit/1639696))
* linkproj now supports optional releaseVersion ([d0c7f21](https://github.com/zywave/OctopusDeploy-Kraken/commit/d0c7f21)), closes [#22](https://github.com/zywave/OctopusDeploy-Kraken/issues/22)
* progress improvements: less API calls, more info closes #26 ([c9f78cc](https://github.com/zywave/OctopusDeploy-Kraken/commit/c9f78cc)), closes [#26](https://github.com/zywave/OctopusDeploy-Kraken/issues/26)
* Show deployment progress for batches ([054af64](https://github.com/zywave/OctopusDeploy-Kraken/commit/054af64))
* some more enhancements to issue #26 to display text to cmdrjs ([534e79c](https://github.com/zywave/OctopusDeploy-Kraken/commit/534e79c))
* upgrade octopus.client to 3.3.3 ([c1b79ef](https://github.com/zywave/OctopusDeploy-Kraken/commit/c1b79ef))