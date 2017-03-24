using Sitecore.Configuration;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules
{
    public struct RulesConstants
    {
        public static string MasterDb = Settings.GetSetting("EnhanceTheEditorExperience.Database.Preferred", "master");
        public static string ContentEditorRulesFolder = Settings.GetSetting("EnhanceTheEditorExperience.RulesItem.Id", "{55F26A63-D476-498F-817B-2FCDE203DB8A}");
        public static string ContentEditorVirtualSectionRulesFolder = Settings.GetSetting("EnhanceTheEditorExperience.VirtualSection.RulesItem.Id", "{662D49FE-9CF9-41FD-87D6-51DBA711E6E9}");
        public static string MappingsDirectory = Settings.GetSetting("EnhanceTheEditorExperience.FileMapping.Directory", "/");
        public static string VirtualSectionMappingsDirectory = Settings.GetSetting("EnhanceTheEditorExperience.VirtualSection.FileMapping.Directory", "/");
    }

    public struct EditorFormatterConstants
    {
        public static bool ShowOriginalFieldToAdmins = Settings.GetBoolSetting("EnhanceTheEditorExperience.ShowOriginalFieldToAdmins", true);
    }
}