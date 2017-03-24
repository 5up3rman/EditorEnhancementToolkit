using System.Web;
using Sitecore;

namespace EditorEnhancementToolkit.Foundation.ContentEditor
{
    public class FileExtensions
    {
        public static string GetMappedFilePath(string directoryPath, string fileName)
        {
            var filePath = HttpContext.Current.Server.MapPath($"{directoryPath}{StringUtil.EnsurePrefix('/', fileName)}");
            return System.IO.File.Exists(filePath) ? filePath : string.Empty;
        }
    }
}