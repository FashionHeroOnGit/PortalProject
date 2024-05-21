namespace Fashionhero.Portal.BusinessLogic.Extensions
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string source, string end)
        {
            return !source.EndsWith(end) ? source : source.Remove(source.LastIndexOf(end, StringComparison.Ordinal));
        }
    }
}