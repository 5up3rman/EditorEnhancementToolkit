using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Shell.Applications.ContentManager;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.VirtualSectionMapping
{
    public interface IVirtualSection
    {
        IEnumerable<Editor.Field> FavoriteFields(IEnumerable<Field> fieldCollection);
        Editor.Section Section();
    }
}