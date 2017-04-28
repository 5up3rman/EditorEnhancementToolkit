module.exports = function () {
  var instanceRoot = "C:\\inetpub\\wwwroot\\modules.local";
  var config = {
    websiteRoot: instanceRoot + "\\Website",
    sitecoreLibraries: instanceRoot + "\\Website\\bin",
    licensePath: instanceRoot + "\\Data\\license.xml",
    solutionName: "EditorEnhancementToolkit",
    buildConfiguration: "Debug",
    runCleanBuilds: false
  };
  return config;
}