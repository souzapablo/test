using NSubstitute;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Services.RebateCalculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private IRebateDataStore _rebateDataStore = Substitute.For<IRebateDataStore>();
    private IProductDataStore _productDataStore = Substitute.For<IProductDataStore>();
    private IRebateCalculatorFactory _rebateCalculatorFactory = Substitute.For<IRebateCalculatorFactory>();

    private RebateService _rebateService => new(_rebateDataStore, _productDataStore, _rebateCalculatorFactory);

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
        _rebateDataStore.Received(1).GetRebate(Arg.Is(request.RebateIdentifier));
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

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier)).Returns(new Rebate
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

    [Theory(DisplayName = "Should return failure when incentive type is not supported")]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.FixedCashAmount, (SupportedIncentiveType)0)]
    [InlineData(IncentiveType.FixedRateRebate, (SupportedIncentiveType)0)]
    [InlineData(IncentiveType.AmountPerUom, (SupportedIncentiveType)0)]
    public void Should_ReturnFailure_When_IncentiveTypeIsNotSupported(
        IncentiveType incentiveType, 
        SupportedIncentiveType supportedIncentiveTypes)
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "some-product",
            Volume = 10
        };

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier))
            .Returns(new Rebate
            {
                Amount = 50m,
                Percentage = 0.1m,
                Incentive = incentiveType
            });

        _productDataStore.GetProduct(Arg.Is(request.ProductIdentifier))
            .Returns(new Product
            {
                Price = 200m,
                SupportedIncentives = supportedIncentiveTypes
            });

        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact(DisplayName = "Should return failure when calculator is not found by Incentive type")]
    public void ShouldReturnFailure_When_CalculatorIsNotFoundByIncentiveType()
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "some-product",
            Volume = 10
        };

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier))
            .Returns(new Rebate
            {
                Amount = 50m,
                Percentage = 0.1m,
                Incentive = IncentiveType.FixedRateRebate
            });

        _productDataStore.GetProduct(Arg.Is(request.ProductIdentifier))
            .Returns(new Product
            {
                Price = 200m,
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate
            });

        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.IsSuccess);
        _rebateCalculatorFactory.Received(1)
            .TryGet(Arg.Is(IncentiveType.FixedRateRebate), out Arg.Any<IRebateCalculator>());
    }

    [Fact(DisplayName = "Should return failure when incentive is AmountPerUom and volume is invalid")]
    public void Should_ReturnFailure_When_AmountPerUomAndVolumeIsInvalid()
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "some-product",
            Volume = 0
        };

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier))
            .Returns(new Rebate
            {
                Amount = 50m,
                Percentage = 0.1m,
                Incentive = IncentiveType.AmountPerUom
            });

        _productDataStore.GetProduct(Arg.Is(request.ProductIdentifier))
            .Returns(new Product
            {
                Price = 200m,
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            });

        _rebateCalculatorFactory.TryGet(Arg.Is(IncentiveType.AmountPerUom), out Arg.Any<IRebateCalculator>())
            .Returns(callInfo =>
            {
                callInfo[1] = new AmountPerUomCalculator();
                return true;
            });
            
        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.IsSuccess);    
        _rebateCalculatorFactory.Received(1)
            .TryGet(Arg.Is(IncentiveType.AmountPerUom), out Arg.Any<IRebateCalculator>());
    }

    [Fact(DisplayName = "Should return failure when incentive is AmountPerUom and amount is invalid")]
    public void Should_ReturnFailure_When_AmountPerUomAndAmountIsInvalid()
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "some-product",
            Volume = 10
        };

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier))
            .Returns(new Rebate
            {
                Amount = 0,
                Percentage = 0.1m,
                Incentive = IncentiveType.AmountPerUom
            });

        _productDataStore.GetProduct(Arg.Is(request.ProductIdentifier))
            .Returns(new Product
            {
                Price = 200m,
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            });

        _rebateCalculatorFactory.TryGet(Arg.Is(IncentiveType.AmountPerUom), out Arg.Any<IRebateCalculator>())
            .Returns(callInfo =>
            {
                callInfo[1] = new AmountPerUomCalculator();
                return true;
            });
            
        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.IsSuccess);    
        _rebateCalculatorFactory.Received(1)
            .TryGet(Arg.Is(IncentiveType.AmountPerUom), out Arg.Any<IRebateCalculator>());
    }

    [Theory(DisplayName = "Should calculate AmountPerUom when volume is valid")]
    [InlineData(0.5, 10, 5)]
    [InlineData(2, 20, 40)]
    public void Should_Calculate_When_AmountPerUomAndVolumeIsValid(decimal volume,decimal rebateAmount, decimal expectedRebate)
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "some-product",
            Volume = volume
        };

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier))
            .Returns(new Rebate
            {
                Amount = rebateAmount,
                Percentage = 0.1m,
                Incentive = IncentiveType.AmountPerUom
            });

        _productDataStore.GetProduct(Arg.Is(request.ProductIdentifier))
            .Returns(new Product
            {
                Price = 200m,
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            });

        _rebateCalculatorFactory.TryGet(Arg.Is(IncentiveType.AmountPerUom), out Arg.Any<IRebateCalculator>())
            .Returns(callInfo =>
            {
                callInfo[1] = new AmountPerUomCalculator();
                return true;
            });
            
        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.True(result.IsSuccess);    
        Assert.Equal(expectedRebate, result.RebateAmount);
    }

    [Fact(DisplayName = "Should return failure when FixedCashAmount and input is not valid")]
    public void Should_ReturnFailure_When_FixedCashAmountAndRebateIsNotValid()
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "some-product",
            Volume = 10
        };

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier))
            .Returns(new Rebate
            {
                Incentive = IncentiveType.FixedCashAmount
            });

        _productDataStore.GetProduct(Arg.Is(request.ProductIdentifier))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            });

        _rebateCalculatorFactory.TryGet(Arg.Is(IncentiveType.FixedCashAmount), out Arg.Any<IRebateCalculator>())
            .Returns(callInfo =>
            {
                callInfo[1] = new FixedCashAmountCalculator();
                return true;
            });
            
        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.False(result.IsSuccess);
        _rebateCalculatorFactory.Received(1)
            .TryGet(Arg.Is(IncentiveType.FixedCashAmount), out Arg.Any<IRebateCalculator>());
        
    }

    [Theory(DisplayName = "Should calculate FixedCashAmount when input is valid")]
    [InlineData(10)]
    [InlineData(20)]
    public void Should_Calculate_When_FixedCashAmountAndRebateIsValid(decimal rebateAmount)
    {
        // Arrange
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "some-rebate",
            ProductIdentifier = "some-product",
            Volume = 10
        };

        _rebateDataStore.GetRebate(Arg.Is(request.RebateIdentifier))
            .Returns(new Rebate
            {
                Amount = rebateAmount,
                Incentive = IncentiveType.FixedCashAmount
            });

        _productDataStore.GetProduct(Arg.Is(request.ProductIdentifier))
            .Returns(new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            });

        _rebateCalculatorFactory.TryGet(Arg.Is(IncentiveType.FixedCashAmount), out Arg.Any<IRebateCalculator>())
            .Returns(callInfo =>
            {
                callInfo[1] = new FixedCashAmountCalculator();
                return true;
            });
            
        // Act
        var result = _rebateService.Calculate(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(rebateAmount, result.RebateAmount);       
    }
}
