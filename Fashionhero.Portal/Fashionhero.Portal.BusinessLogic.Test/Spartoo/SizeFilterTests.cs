using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.BusinessLogic.Test.Core;
using Fashionhero.Portal.BusinessLogic.Test.Extensions;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Spartoo
{
    public class SizeFilterTests
    {
        private readonly Mock<ILogger<SizeFilter>> mockedLogger;

        public SizeFilterTests()
        {
            mockedLogger = new Mock<ILogger<SizeFilter>>();
        }

        private static ICollection<IProduct> GenerateInvalidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Sizes = new List<ISize>()
                    {
                        new Size(),
                    },
                },
                new Product
                {
                    Sizes = new List<ISize>()
                    {
                        new Size(),
                    },
                },
            ]).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateValidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Sizes = new List<ISize>()
                    {
                        new Size()
                        {
                            Primary = "XL",
                        },
                    },
                },
                new Product
                {
                    Sizes = new List<ISize>()
                    {
                        new Size()
                        {
                            Primary = "XL",
                        },
                    },
                },
            ]).Cast<IProduct>().ToList();
        }

        [Fact]
        public void ItDoesNotRemovesProductsWhenApplyingTheFilter()
        {
            var expected = GenerateValidProducts();
            var original = GenerateValidProducts();
            var sut = new SizeFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProduct()
        {
            const string expectedLogMessageFragment = "as one or more sizes is missing its";
            var original = GenerateInvalidProducts();
            var sut = new SizeFilter(mockedLogger.Object);

            sut.FilterProducts(original);

            mockedLogger.VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.TryGetInvocation(expectedLogMessageFragment);
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public void ItRemovesInvalidProductsWhenApplyingTheFilter()
        {
            var expected = TestEntitiesBuilder.GenerateEmptyProductsList();
            var original = GenerateInvalidProducts();
            var sut = new SizeFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanSizeFilter()
        {
            const bool expected = false;
            var sut = new SizeFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_CURRENCY);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsSizeFilter()
        {
            const bool expected = true;
            var sut = new SizeFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_SIZE);

            actual.Should().Be(expected);
        }
    }
}