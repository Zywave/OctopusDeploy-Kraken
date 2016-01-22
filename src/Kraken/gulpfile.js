"use strict";

var gulp = require("gulp"),
    glob = require("glob"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    requirejs = require("requirejs");

gulp.task("clean:js", function (cb) {
    rimraf("./wwwroot/js/app.built.js", cb);
});

gulp.task("clean:css", function (cb) {
    rimraf("./wwwroot/css/site.min.css", cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:css", function () {
    return gulp.src(["./wwwroot/css/**/*.css!./wwwroot/css/**/*.min.css"])
        .pipe(concat("./wwwroot/css/site.min.css"))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:css"]);

gulp.task("build", function () {

    var htmlIncludes = glob.sync("html/**/*.html", { cwd: process.cwd() + "/wwwroot" }).map(function (path) {
        return "text!" + path;
    });
    
    requirejs.optimize({
        baseUrl: "./wwwroot/js",
        name: "../../node_modules/almond/almond",
        paths: {
            context: "empty:",
            jquery: "empty:",
            knockout: "empty:",
            bootstrap: "empty:",
            cmdr: "empty:",
            moment: "empty:",

            html: "../html",

            text: "../../node_modules/requirejs-text/text"
        },
        include: ["app"].concat(htmlIncludes),
        inlineText: true,
        stubModules: ["text"],
        wrap: true,
        preserveLicenseComments: false,
        out: "./wwwroot/js/app.built.js"
    });

});
