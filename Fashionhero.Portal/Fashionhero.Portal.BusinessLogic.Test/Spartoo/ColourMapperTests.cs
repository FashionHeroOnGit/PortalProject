using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Spartoo
{
    public class ColourMapperTests
    {
        [Fact]
        public void ItReturnsDefaultValueWhenKeyDoesNotExistInDictionary()
        {
            const int expected = 534;
            var sut = new ColourMapper();

            object actual = sut.GetDictionaryValue("some random text that is not a key in the dictionary");

            actual.Should().BeOfType<int>();
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
    }
}