<a name="1.0.0-prerelease.6"></a>
# [1.0.0-prerelease.6](https://github.com/zywave/OctopusDeploy-Kraken/compare/1.0.0-prerelease.5...v1.0.0-prerelease.6) (2016-03-18)


### Bug Fixes

* logs file powershell fix ([fd3fccf](https://github.com/zywave/OctopusDeploy-Kraken/commit/fd3fccf))



<a name="1.0.0-prerelease.5"></a>
# [1.0.0-prerelease.5](https://github.com/zywave/OctopusDeploy-Kraken/compare/1.0.0-prerelease.4...v1.0.0-prerelease.5) (2016-03-18)


### Bug Fixes

* force logs directory to be created ([5ebf680](https://github.com/zywave/OctopusDeploy-Kraken/commit/5ebf680))
* improved exception handling ([9074d09](https://github.com/zywave/OctopusDeploy-Kraken/commit/9074d09))
* remove duplicate octopus.client references ([0978baa](https://github.com/zywave/OctopusDeploy-Kraken/commit/0978baa))

### Features

* allow for enabling of stdout logging from octovariable ([674b5a7](https://github.com/zywave/OctopusDeploy-Kraken/commit/674b5a7))
* progress command cleanup ([fbc94f5](https://github.com/zywave/OctopusDeploy-Kraken/commit/fbc94f5))



<a name="1.0.0-prerelease.4"></a>
# [1.0.0-prerelease.4](https://github.com/zywave/OctopusDeploy-Kraken/compare/1.0.0-prerelease.3...v1.0.0-prerelease.4) (2016-03-17)


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



<a name="1.0.0-prerelease.3"></a>
# [1.0.0-prerelease.3](https://github.com/zywave/OctopusDeploy-Kraken/compare/1.0.0-prerelease.2...v1.0.0-prerelease.3) (2016-03-04)


### Bug Fixes

* appveyor artifact push fix ([53111a8](https://github.com/zywave/OctopusDeploy-Kraken/commit/53111a8)), closes [#31](https://github.com/zywave/OctopusDeploy-Kraken/issues/31)



<a name="1.0.0-prerelease.2"></a>
# [1.0.0-prerelease.2](https://github.com/zywave/OctopusDeploy-Kraken/compare/1.0.0-prerelease.1...v1.0.0-prerelease.2) (2016-03-04)


### Bug Fixes

* increase request timeout from 2 to 5 mins ([290ba55](https://github.com/zywave/OctopusDeploy-Kraken/commit/290ba55))
* nuget/semver prerelease version ([a75574e](https://github.com/zywave/OctopusDeploy-Kraken/commit/a75574e)), closes [#31](https://github.com/zywave/OctopusDeploy-Kraken/issues/31)



<a name="1.0.0-prerelease.1"></a>
# [1.0.0-prerelease.1](https://github.com/zywave/OctopusDeploy-Kraken/compare/1.0.0-prerelease.0...v1.0.0-prerelease.1) (2016-03-04)


### Bug Fixes

* prevent shell opening if linkproj succeeds ([0b99d4e](https://github.com/zywave/OctopusDeploy-Kraken/commit/0b99d4e))
* select2 bootstrap styles ([b903f35](https://github.com/zywave/OctopusDeploy-Kraken/commit/b903f35))
* store true username (from octo) rather than provided value ([d6af485](https://github.com/zywave/OctopusDeploy-Kraken/commit/d6af485)), closes [#28](https://github.com/zywave/OctopusDeploy-Kraken/issues/28)
* using select2's ajax support to lazy load projects ([430f459](https://github.com/zywave/OctopusDeploy-Kraken/commit/430f459))

### Features

* api key auth support ([21cfe4f](https://github.com/zywave/OctopusDeploy-Kraken/commit/21cfe4f)), closes [#20](https://github.com/zywave/OctopusDeploy-Kraken/issues/20)
* update project name/slug on sync ([cf7a023](https://github.com/zywave/OctopusDeploy-Kraken/commit/cf7a023)), closes [#15](https://github.com/zywave/OctopusDeploy-Kraken/issues/15)
* user display names for audit info ([19e56b8](https://github.com/zywave/OctopusDeploy-Kraken/commit/19e56b8)), closes [#29](https://github.com/zywave/OctopusDeploy-Kraken/issues/29)



<a name="1.0.0-prerelease.0"></a>
## 1.0.0-prerelease.0 (2016-02-25)



