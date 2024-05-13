using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Extensions
{
    public static class XmlExtensions
    {
        public static bool IsEmptyValue(this XElement? element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.Value.Length == 0;
        }

        public static string GetTagValueAsString(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);

                return taggedElement.Value;
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, $"Unknown tag Supplied");
                throw;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Failed to get tag ({tag}) value from element ({element}). Returning empty string.");
                return string.Empty;
            }
        }

        private static XElement GetTaggedElement(this XElement element, string tag)
        {
            XElement? inner = element.Element(tag);
            if (inner == null)
                throw new ArgumentException($"Missing Tag ({tag}) in supplied element ({element})");
            return inner;
        }

        public static int GetTagValueAsInt(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);

                bool success = int.TryParse(taggedElement.Value, out int result);

                if (!success)
                    throw new InvalidCastException("Failed to cast value of xml element to int");

                return result;
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, $"Unknown tag Supplied");
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

        public static long GetTaggedValueAsLong(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);

                bool success = long.TryParse(taggedElement.Value, out long result);

                if (!success)
                    throw new InvalidCastException("Failed to cast value of xml element to int");

                return result;
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, $"Unknown tag Supplied");
                throw;
            }
            catch (InvalidCastException ice)
            {
                logger.LogWarning(ice,
                    $"Unable to cast tag ({tag}) of element ({element}) to float. Returning default value.");
                return default;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Failed to get tag ({tag}) value from element ({element}). Returning default value.");
                return default;
            }
        }

        public static float GetTaggedValueAsFloat(this XElement element, string tag, ILogger logger)
        {
            try
            {
                XElement taggedElement = element.GetTaggedElement(tag);

                string toParse = taggedElement.Value.Split(' ')[0];
                bool success = float.TryParse(toParse.Length == 0 ? "0" : toParse, out float result);

                if (!success)
                    throw new InvalidCastException("Failed to cast value of xml element to int");

                return result;
            }
            catch (ArgumentException ae)
            {
                logger.LogError(ae, $"Unknown tag Supplied");
                throw;
            }
            catch (InvalidCastException ice)
            {
                logger.LogWarning(ice,
                    $"Unable to cast tag ({tag}) of element ({element}) to float. Returning default value.");
                return default;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"Failed to get tag ({tag}) value from element ({element}). Returning default value.");
                return default;
            }
        }
    }
}