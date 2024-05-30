using Microsoft.Extensions.Logging;
using Moq;

namespace Fashionhero.Portal.BusinessLogic.Test.Extensions
{
    public static class MockedLoggerExtensions
    {
        /// <summary>
        ///     Found here:
        ///     https://stackoverflow.com/questions/62091109/how-to-verify-log-message-in-unit-testing-for-a-passing-test
        /// </summary>
        public static void VerifyLogWarningCalled<T>(this Mock<ILogger<T>> mockedLogger)
        {
            mockedLogger.Verify(
                x => x.Log(It.Is<LogLevel>(l => l == LogLevel.Warning), It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => true), It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((obj, type) => true)), Times.AtLeastOnce());
        }
    }
}