using System;
using System.Collections.Generic;
using System.Linq;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;

namespace EditorEnhancementToolkit.Foundation.ContentEditor
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> AddOrReplaceItem<T>(this List<T> list, T item) where T : class, IMapItemBase
        {
            var idx =
                list.FindIndex(
                    x => x.Title.Equals(item.Title, StringComparison.InvariantCultureIgnoreCase));

            if (idx.Equals(-1))
                list.Add(item);
            else
                list[idx] = item;

            return list;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
    }
}
