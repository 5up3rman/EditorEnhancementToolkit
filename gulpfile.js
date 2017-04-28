var gulp = require("gulp");
var msbuild = require("gulp-msbuild");
var debug = require("gulp-debug");
var foreach = require("gulp-foreach");
var rename = require("gulp-rename");
var watch = require("gulp-watch");
var merge = require("merge-stream");
var newer = require("gulp-newer");
var util = require("gulp-util");
var runSequence = require("run-sequence");
var path = require("path");
var config = require("./gulp-config.js")();
var nugetRestore = require('gulp-nuget-restore');
var fs = require('fs');
var unicorn = require("./scripts/unicorn.js");
var paragon = require("./scripts/paragon.js");
var yargs = require("yargs").argv;

module.exports.config = config; 

gulp.task("default", function (callback) {
  config.runCleanBuilds = true;
  return runSequence(
    "01-Nuget-Restore",
    "02-Publish-All-Projects",
    "03-Sync-Unicorn",
	callback);
});

/*****************************
  Initial setup
*****************************/
gulp.task("01-Nuget-Restore", function (callback) {
  var solution = "./" + config.solutionName + ".sln";
  return gulp.src(solution).pipe(nugetRestore());
});


gulp.task("02-Publish-All-Projects", function (callback) {
  return runSequence(
    "Build-Solution",
    "Publish-Foundation-Projects",
    "Publish-Feature-Projects",
    "Publish-Project-Projects", callback);
});

gulp.task("03-Sync-Unicorn", function (callback) {
  var options = {};
  options.siteHostName = paragon.getSiteUrl();
  options.authenticationConfigFile = config.websiteRoot + "/App_config/Include/Unicorn/Unicorn.UI.config";

  unicorn(function() { return callback() }, options);
});

/*****************************
  Copy assemblies to all local projects
*****************************/
gulp.task("Copy-Local-Assemblies", function () {
  console.log("Copying site assemblies to all local projects");
  var files = config.sitecoreLibraries + "/**/*";

  var root = "./src";
  var projects = root + "/**/code/bin";
  return gulp.src(projects, { base: root })
    .pipe(foreach(function (stream, file) {
      console.log("copying to " + file.path);
      gulp.src(files)
        .pipe(gulp.dest(file.path));
      return stream;
    }));
});

/*****************************
  Publish
*****************************/
var publishStream = function (stream, dest) {
  var targets = ["Build"];

  return stream
    .pipe(debug({ title: "Building project:" }))
    .pipe(msbuild({
      targets: targets,
      configuration: config.buildConfiguration,
      logCommand: false,
      verbosity: "minimal",
      stdout: true,
      errorOnFail: true,
      maxcpucount: 0,
      toolsVersion: 14.0,
      properties: {
        DeployOnBuild: "true",
        DeployDefaultTarget: "WebPublish",
        WebPublishMethod: "FileSystem",
        DeleteExistingFiles: "false",
        publishUrl: dest,
        _FindDependencies: "false"
      }
    }));
}

var publishProject = function (location, dest) {
  dest = dest || config.websiteRoot;

  console.log("publish to " + dest + " folder");
  return gulp.src(["./src/" + location + "/code/*.csproj"])
    .pipe(foreach(function (stream, file) {
      return publishStream(stream, dest);
    }));
}

var publishProjects = function (location, dest) {
  dest = dest || config.websiteRoot;

  console.log("publish to " + dest + " folder");
  return gulp.src([location + "/**/code/*.csproj"])
    .pipe(foreach(function (stream, file) {
      return publishStream(stream, dest);
    }));
};

gulp.task("Build-Solution", function () {
  var targets = ["Build"];
  if (config.runCleanBuilds) {
    targets = ["Clean", "Build"];
  }
  var solution = "./" + config.solutionName + ".sln";
  return gulp.src(solution)
      .pipe(msbuild({
          targets: targets,
          configuration: config.buildConfiguration,
          logCommand: false,
          verbosity: "minimal",
          stdout: true,
          errorOnFail: true,
          maxcpucount: 0,
          toolsVersion: 14.0
        }));
});

gulp.task("Publish-Foundation-Projects", function () {
  return publishProjects("./src/Foundation");
});

gulp.task("Publish-Feature-Projects", function () {
  return publishProjects("./src/Feature");
});

gulp.task("Publish-Project-Projects", function () {
  return publishProjects("./src/Project");
});

gulp.task("Publish-Project", function () {
  if(yargs && yargs.m && typeof(yargs.m) == 'string') {
    return publishProject(yargs.m);
  } else {
    throw "\n\n------\n USAGE: -m Layer/Module \n------\n\n";
  }
});

gulp.task("Publish-Assemblies", function () {
  var root = "./src";
  var binFiles = root + "/**/code/**/bin/Paragon.{Feature,Foundation}.*.{dll,pdb}";
  var destination = config.websiteRoot + "/bin/";
  return gulp.src(binFiles, { base: root })
    .pipe(rename({ dirname: "" }))
    .pipe(newer(destination))
    .pipe(debug({ title: "Copying " }))
    .pipe(gulp.dest(destination));
});

gulp.task("Publish-All-Views", function () {
  var root = "./src";
  var roots = [root + "/**/Views", "!" + root + "/**/obj/**/Views"];
  var files = "/**/*.cshtml";
  var destination = config.websiteRoot + "\\Views";
  return gulp.src(roots, { base: root }).pipe(
    foreach(function (stream, file) {
      console.log("Publishing from " + file.path);
      gulp.src(file.path + files, { base: file.path })
        .pipe(newer(destination))
        .pipe(debug({ title: "Copying " }))
        .pipe(gulp.dest(destination));
      return stream;
    })
  );
});

gulp.task("Publish-All-Configs", function () {
  var root = "./src";
  var roots = [root + "/**/App_Config", "!" + root + "/**/obj/**/App_Config"];
  var files = "/**/*.config";
  var destination = config.websiteRoot + "\\App_Config";
  return gulp.src(roots, { base: root }).pipe(
    foreach(function (stream, file) {
      console.log("Publishing from " + file.path);
      gulp.src(file.path + files, { base: file.path })
        .pipe(newer(destination))
        .pipe(debug({ title: "Copying " }))
        .pipe(gulp.dest(destination));
      return stream;
    })
  );
});

/*****************************
 Watchers
*****************************/
gulp.task("Auto-Publish-Css", function () {
  var root = "./src";
  var roots = [root + "/**/assets/css", "!" + root + "/**/obj/**/assets/css"];
  var files = "/**/*.css";
  var destination = config.websiteRoot + "\\styles";
  gulp.src(roots, { base: root }).pipe(
    foreach(function (stream, rootFolder) {
      gulp.watch(rootFolder.path + files, function (event) {
        if (event.type === "changed") {
          console.log("publish this file " + event.path);
          gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
        }
        console.log("published " + event.path);
      });
      return stream;
    })
  );
});

gulp.task("Auto-Publish-Views", function () {
  var root = "./src";
  var roots = [root + "/**/Views", "!" + root + "/**/obj/**/Views"];
  var files = "/**/*.cshtml";
  var destination = config.websiteRoot + "\\Views";
  gulp.src(roots, { base: root }).pipe(
    foreach(function (stream, rootFolder) {
      gulp.watch(rootFolder.path + files, function (event) {
        if (event.type === "changed") {
          console.log("publish this file " + event.path);
          gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
        }
        console.log("published " + event.path);
      });
      return stream;
    })
  );
});

gulp.task("Auto-Publish-Assemblies", function () {
  var root = "./src";
  var roots = [root + "/**/code/**/bin"];
  var files = "/**/Pargon.{Feature,Foundation}.*.{dll,pdb}";;
  var destination = config.websiteRoot + "/bin/";
  gulp.src(roots, { base: root }).pipe(
    foreach(function (stream, rootFolder) {
      gulp.watch(rootFolder.path + files, function (event) {
        if (event.type === "changed") {
          console.log("publish this file " + event.path);
          gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
        }
        console.log("published " + event.path);
      });
      return stream;
    })
  );
});
