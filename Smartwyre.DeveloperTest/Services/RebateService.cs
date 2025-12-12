using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services.RebateCalculators;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IRebateCalculatorFactory _rebateCalculatorFactory;

    public RebateService(
        IRebateDataStore rebateDataStore, 
        IProductDataStore productDataStore, 
        IRebateCalculatorFactory rebateCalculatorFactory)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _rebateCalculatorFactory = rebateCalculatorFactory;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);

        if (rebate is null)
            return CalculateRebateResult.Failure();
        
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (product is null)
            return CalculateRebateResult.Failure();

        if (!product.SupportsIncentive(rebate.Incentive))
            return CalculateRebateResult.Failure();

        var calculatorExists = _rebateCalculatorFactory.TryGet(rebate.Incentive, out var rebateCalculator);
        if (!calculatorExists)
            return CalculateRebateResult.Failure();

        var context = RebateCalculationContext.Create(product, rebate, request.Volume);
        
        var result = rebateCalculator.Calculate(context);

        if (!result.IsSuccess)
            return CalculateRebateResult.Failure();

        _rebateDataStore.StoreCalculationResult(rebate, result.RebateAmount);

        return result;
    }
}
