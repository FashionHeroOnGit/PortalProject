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
    public class ImageFilterTests
    {
        private readonly Mock<ILogger<ImageFilter>> mockedLogger;

        public ImageFilterTests()
        {
            mockedLogger = new Mock<ILogger<ImageFilter>>();
        }

        private static ICollection<IProduct> GenerateInvalidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Images = new List<IImage>
                    {
                        new Image
                        {
                            Url = "some-invalid-extension.png",
                        },
                    },
                },
                new Product
                {
                    Images = new List<IImage>
                    {
                        new Image
                        {
                            Url = "some-invalid-extension.png",
                        },
                    },
                },
            ]).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateValidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Images = new List<IImage>
                    {
                        new Image
                        {
                            Url = "some-invalid-extension.jpg",
                        },
                    },
                },
                new Product
                {
                    Images = new List<IImage>
                    {
                        new Image
                        {
                            Url = "some-invalid-extension.jpg",
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
            var sut = new ImageFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProduct()
        {
            const string expectedLogMessageFragment = "as it does not have any .jpg images";
            var original = GenerateInvalidProducts();
            var sut = new ImageFilter(mockedLogger.Object);

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
            var sut = new ImageFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanImageFilter()
        {
            const bool expected = false;
            var sut = new ImageFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_CURRENCY);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsImageFilter()
        {
            const bool expected = true;
            var sut = new ImageFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_IMAGE);

            actual.Should().Be(expected);
        }
    }
}