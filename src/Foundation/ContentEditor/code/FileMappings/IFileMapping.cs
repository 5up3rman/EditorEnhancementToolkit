using System.Collections.Generic;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.FileMappings
{
    public interface IFileMapping 
    {
        IEnumerable<T> GetItems<T>() where T : IFileMapping;
    }
}