using Sitecore.Configuration;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules
{
    public struct RulesConstants
    {
        public static string MasterDb = Settings.GetSetting("EnhanceTheEditorExperience.Database.Preferred", "master");
        public static string ContentEditorRulesFolder = Settings.GetSetting("EnhanceTheEditorExperience.RulesItem.Id", "{55F26A63-D476-498F-817B-2FCDE203DB8A}");
        public static string MappingsDirectory = Settings.GetSetting("EnhanceTheEditorExperience.FileMapping.Directory", "/");
    }

    public struct EditorFormatterConstants
    {
        public static bool ShowOriginalFieldToAdmins = Settings.GetBoolSetting("EnhanceTheEditorExperience.ShowOriginalFieldToAdmins", true);
    }
}