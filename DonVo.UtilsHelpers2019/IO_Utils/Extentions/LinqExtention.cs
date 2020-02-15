using System.Collections.Generic;

namespace IO_Utils.Extentions
{
    public static class LinqExtention
    {
        public static IEnumerable<T> Repeat<T>(this T item, int times)
        {
            for (int i = 0; i < times; i++)
            {
                yield return item;
            }
        }
    }
}
