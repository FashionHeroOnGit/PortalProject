using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Extensions
{
    public static class XmlExtensions
    {
        public static XElement GetTaggedElement(this XElement element, string tag)
        {
            XElement? inner = element.Element(tag);
            if (inner == null)
                throw new ArgumentException($"Missing Tag ({tag}) in supplied element ({element})");
            return inner;
        }

        public static decimal GetTaggedValueAsDecimal(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);

                string toParse = taggedElement.Value.Split(' ')[0].ConvertToEuropeanNumberStyle();
                bool success = decimal.TryParse(toParse.Length == 0 ? "0" : toParse, out decimal result);

                if (!success)
                    throw new InvalidCastException("Failed to cast value of xml element to decimal");

                return result;
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, "Unknown tag Supplied");
                throw;
            }
            catch (InvalidCastException ice)
            {
                logger.LogWarning(ice,
                    $"Unable to cast tag ({tag}) of element ({element}) to decimal. Returning default value.");
                return default;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Failed to get tag ({tag}) value from element ({element}). Returning default value.");
                return default;
            }
        }

        public static long GetTaggedValueAsLong(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);
                string value = taggedElement.Value.TrimEnd("X23");
                if (!value.All(char.IsNumber))
                    throw new InvalidOperationException(
                        $"Unable to parse tag ({tag}) to {nameof(Int64)}, as value ({value}) contains non-number characters.");

                bool success = long.TryParse(value, out long result);

                if (!success)
                    throw new InvalidCastException("Failed to cast value of xml element to long");

                return result;
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, "Unknown tag Supplied");
                throw;
            }
            catch (InvalidCastException ice)
            {
                logger.LogWarning(ice,
                    $"Unable to cast tag ({tag}) of element ({element}) to long. Returning default value.");
                return default;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Failed to get tag ({tag}) value from element ({element}). Returning default value.");
                return default;
            }
        }

        public static int GetTagValueAsInt(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);
                string value = taggedElement.Value.Split('.', ',')[0];
                if (!value.All(char.IsNumber))
                    throw new InvalidOperationException(
                        $"Unable to parse tag ({tag}) to {nameof(Int32)}, as value ({value}) contains non-number characters.");

                bool success = int.TryParse(value, out int result);

                if (!success)
                    throw new InvalidCastException("Failed to cast value of xml element to int");

                return result;
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, "Unknown tag Supplied");
                throw;
            }
            catch (InvalidCastException ice)
            {
                logger.LogWarning(ice,
                    $"Unable to cast tag ({tag}) of element ({element}) to int. Returning default value.");
                return default;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Failed to get tag ({tag}) value from element ({element}). Returning default value.");
                return default;
            }
        }

        public static string GetTagValueAsString(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);

                return taggedElement.Value.Replace("\n", string.Empty).Trim();
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, "Unknown tag Supplied");
                throw;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Failed to get tag ({tag}) value from element ({element}). Returning empty string.");
                return string.Empty;
            }
        }

        public static bool IsEmptyValue(this XElement? element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.Value.Length == 0;
        }
    }
}