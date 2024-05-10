using System.Xml.Linq;

namespace Fashionhero.Portal.BusinessLogic.Extensions
{
    public static class XmlExtensions
    {
        public static int GetValueAsInt(this XElement? element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            bool success = int.TryParse(element.Value, out int result);

            if (!success)
                throw new InvalidCastException("Failed to cast value of xml element to int");

            return result;
        }

        public static string GetValue(this XElement? element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.Value;
        }
    }
}