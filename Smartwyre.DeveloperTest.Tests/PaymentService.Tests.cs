using NSubstitute;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private IRebateDataStore _rebateDataStore = Substitute.For<IRebateDataStore>();
    private IProductDataStore _productDataStore = Substitute.For<IProductDataStore>();

    private RebateService _rebateService => new(_rebateDataStore, _productDataStore);

    [Fact(DisplayName = "Should return failure when rebate is not found in datastore")]
    public void Should_ReturnFailure_When_RabateIsNotFound()
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "non-existing-rebate",
            ProductIdentifier = "some-product"
        };

        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.IsSuccess);
        _rebateDataStore.Received(1).GetRebate("non-existing-rebate");
    }

    [Fact(DisplayName = "Should return failure when product is not found in datastore")]
    public void Should_ReturnFailure_When_ProductIsNotFound()
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "non-existing-product"
        };

        _rebateDataStore.GetRebate(Arg.Is("some-rebate")).Returns(new Rebate
        {
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 100m
        });

        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.IsSuccess);
        _rebateDataStore.Received(1).GetRebate("some-rebate");
        _productDataStore.Received(1).GetProduct("non-existing-product");
    }
}
