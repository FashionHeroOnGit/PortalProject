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
    public class ModelProductNumberFilterTests
    {
        private readonly Mock<ILogger<ModelProductNumberFilter>> mockedLogger;

        public ModelProductNumberFilterTests()
        {
            mockedLogger = new Mock<ILogger<ModelProductNumberFilter>>();
        }


        private static ICollection<IProduct> GenerateInvalidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    ModelProductNumber = "",
                },
                new Product
                {
                    ModelProductNumber = "",
                },
            ], true).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateValidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    ModelProductNumber = "some-model-product-number",
                },
                new Product
                {
                    ModelProductNumber = "some-model-product-number",
                },
            ]).Cast<IProduct>().ToList();
        }

        [Fact]
        public void ItDoesNotRemovesProductsWhenApplyingTheFilter()
        {
            var expected = GenerateValidProducts();
            var original = GenerateValidProducts();
            var sut = new ModelProductNumberFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProduct()
        {
            const string expectedLogMessageFragment = "as it is missing a Model Product Number";
            var original = GenerateInvalidProducts();
            var sut = new ModelProductNumberFilter(mockedLogger.Object);

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
            var sut = new ModelProductNumberFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanModelProductNumberFilter()
        {
            const bool expected = false;
            var sut = new ModelProductNumberFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_IMAGE);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsModelProductNumberFilter()
        {
            const bool expected = true;
            var sut = new ModelProductNumberFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_MODEL_PRODUCT_NUMBER);

            actual.Should().Be(expected);
        }
    }
}