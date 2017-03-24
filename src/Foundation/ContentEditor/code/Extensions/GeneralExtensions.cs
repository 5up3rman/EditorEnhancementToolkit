namespace EditorEnhancementToolkit.Foundation.ContentEditor
{
    public static class GeneralExtensions
    {
        public static bool ToBool(this string val)
        {
            var result = false;
            bool.TryParse(val, out result);

            return result;
        }

        public static int ToInt(this string val)
        {
            var result = 0;
            int.TryParse(val, out result);

            return result;
        }
    }
}