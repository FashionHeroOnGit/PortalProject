using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.BusinessLogic.Test.Core;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Spartoo
{
    public class ColourMapperTests
    {
        private readonly Mock<ILogger<SpartooService>> mockedLogger;

        public ColourMapperTests()
        {
            mockedLogger = new Mock<ILogger<SpartooService>>();
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanColourFilter()
        {
            const bool expected = false;
            var sut = new ColourMapper();

            bool actual = sut.IsMapperOfType(MapType.SPARTOO_TYPE);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsColourFilter()
        {
            const bool expected = true;
            var sut = new ColourMapper();

            bool actual = sut.IsMapperOfType(MapType.SPARTOO_COLOUR);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItReturnsValueFromDictionaryWhenKeyExists()
        {
            const int expected = 1;
            var sut = new ColourMapper();

            object actual = sut.GetDictionaryValue("hvid");

            actual.Should().BeOfType<int>();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ItReturnsDefaultValueWhenKeyDoesNotExistInDictionary()
        {
            const int expected = 534;
            var sut = new ColourMapper();

            object actual = sut.GetDictionaryValue("some random text that is not a key in the dictionary");

            actual.Should().BeOfType<int>();
            actual.Should().Be(expected);
        }
    }
}