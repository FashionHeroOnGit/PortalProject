using System.Text;

namespace Fashionhero.Portal.BusinessLogic.Extensions
{
    public static class StringExtensions
    {
        public static string ConvertToEuropeanNumberStyle(this string source)
        {
            var indexes = source.AllIndexesOf('.', ',');
            if (!indexes.Any())
                return source;

            int last = indexes.Last();
            var builder = new StringBuilder(source);
            foreach (int index in indexes)
                builder[index] = index == last ? ',' : '.';

            return builder.ToString();
        }

        public static string TrimEnd(this string source, string end)
        {
            return !source.EndsWith(end) ? source : source.Remove(source.LastIndexOf(end, StringComparison.Ordinal));
        }

        private static ICollection<int> AllIndexesOf(this string source, params char[] chars)
        {
            var indexes = new List<int>();
            for (var index = 0; index < source.Length; index++)
            {
                if (chars.Contains(source[index]))
                    indexes.Add(index);
            }

            return indexes;
        }
    }
}