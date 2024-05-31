namespace Fashionhero.Portal.BusinessLogic.Test.Core
{
    // Todo: Find a better name...
    public static class TestHelpers
    {
        public static string LoadXmlFileContent(string fileName)
        {
            return File.ReadAllText(Path.Combine(@"..\..\..\", fileName));
        }
    }
}