"use strict";

var gulp = require('gulp');
var runSequence = require('run-sequence');
var conventionalChangelog = require('gulp-conventional-changelog');
var bump = require('gulp-bump');
var gutil = require('gulp-util');
var insert = require('gulp-insert');
var trim = require('gulp-trim');
var xmlpoke = require('gulp-xmlpoke');
var git = require('gulp-git');
var fs = require('fs');

function getVersion(nuget) {
    var version = JSON.parse(fs.readFileSync('./package.json', 'utf8')).version;

    if (nuget) {
        // nuget doesn't support semver prerelease numbers so converting 1.2.3-prerelease.1 to 1.2.3.1-prerelease
        var match = /((?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*)\.(?:0|[1-9][0-9]*))(?:-([\da-z\-]+)\.(0|[1-9][0-9]*))?/i.exec(version);
        if (match[2]) {
            version = match[1] + '.' + match[3] + '-' + match[2];
        }
    };

    return version;
}

gulp.task('bump-version', function () {
    var type = 'patch';
    var types = ['major', 'minor', 'patch', 'prerelease'];
    for (var i = 0; i < 4; i++) {
        if (process.argv.indexOf('--' + types[i]) > -1) {
            type = types[i];
            break;
        }
    }

    return gulp.src('./package.json')
      .pipe(bump({ type: type, preid: 'prerelease' }).on('error', gutil.log))
      .pipe(gulp.dest('./'));
});

gulp.task('changelog', function () {
    return gulp.src('CHANGELOG.md', { buffer: false })
        .pipe(conventionalChangelog({ preset: 'angular' }))
        .pipe(gulp.dest('./'));
});

gulp.task('releasenotes', function () {
    return gulp.src('RELEASENOTES.md')
        .pipe(insert.transform(function () { return ''; })) 
        .pipe(conventionalChangelog({ preset: 'angular' }, {}, {}, {}, { headerPartial: '' }))
        .pipe(trim())
        .pipe(gulp.dest('./'));
});

gulp.task('update-nuspec', function () {
    var version = getVersion(true);
    var releaseNotes = fs.readFileSync('RELEASENOTES.md', 'utf8');

    return gulp.src('Kraken.nuspec')
        .pipe(xmlpoke({
            replacements: [
                { xpath: '/package/metadata/version', value: version },
                { xpath: '/package/metadata/releaseNotes', value: releaseNotes },
            ]
        }))
        .pipe(gulp.dest('./'));
});

gulp.task('commit-changes', function () {
    return gulp.src('.')
      .pipe(git.add())
      .pipe(git.commit('chore(release): bump version, update changelog'));
});

gulp.task('push-changes', function (cb) {
    git.push('origin', 'master', cb);
});

gulp.task('create-new-tag', function (cb) {
    var version = getVersion();
    git.tag(version, 'create tag for version: ' + version, function (error) {
        if (error) {
            return cb(error);
        }
        git.push('origin', 'master', { args: '--tags' }, cb);
    });
});

gulp.task('release', function (callback) {
    runSequence(
      'bump-version',
      'changelog',
      'releasenotes',
      'update-nuspec',
      'commit-changes',
      'push-changes',
      'create-new-tag',
      function (error) {
          if (error) {
              console.log(error.message);
          } else {
              console.log('Release successful');
          }
          callback(error);
      });
});