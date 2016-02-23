"use strict";

var gulp = require("gulp"),
    glob = require("glob"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    requirejs = require("requirejs");

gulp.task("npmlib", function () {
    return gulp.src(['./node_modules/babel-polyfill/dist/**/*']).pipe(gulp.dest('./wwwroot/lib/babel-polyfill/dist'));
});

gulp.task("clean:js", function (cb) {
    rimraf("./wwwroot/js/app.built.js", cb);
});

gulp.task("clean:css", function (cb) {
    rimraf("./wwwroot/css/site.min.css", cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:css", function () {
    return gulp.src(["./wwwroot/css/**/*.css","!./wwwroot/css/**/*.min.css"])
        .pipe(concat("./wwwroot/css/site.min.css"))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:css"]);

gulp.task("build", function () {

    var includes = [];
    includes = includes.concat(glob.sync("html/views/**/*.html", { cwd: process.cwd() + "/wwwroot" }).map(function (path) {
        return "text!" + path;
    }));
    includes = includes.concat(glob.sync("views/**/*.js", { cwd: process.cwd() + "/wwwroot/js" }).map(function (path) {
        return path.slice(0, -3);
    }));
    
    requirejs.optimize({
        baseUrl: "./wwwroot/js",
        name: "app",
        paths: {
            context: "empty:",
            jquery: "empty:",
            knockout: "empty:",
            bootstrap: "empty:",
            cmdr: "empty:",
            moment: "empty:",
            select2: "empty:",
            koselect2: "empty:",

            html: "../html",

            text: "../../node_modules/requirejs-text/text"
        },
        include: includes,
        inlineText: true,
        stubModules: ["text"],
        wrap: true,
        preserveLicenseComments: false,
        out: "./wwwroot/js/app.built.js"
    });

});
