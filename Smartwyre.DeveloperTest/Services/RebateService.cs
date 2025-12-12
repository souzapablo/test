using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;

    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        var result = new CalculateRebateResult();

        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);

        if (rebate is null)
            return CalculateRebateResult.Failure();
        
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (product is null)
            return CalculateRebateResult.Failure();

        var rebateAmount = 0m;

        switch (rebate.Incentive)
        {
            case IncentiveType.FixedCashAmount:
                if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
                {
                    result.IsSuccess = false;
                }
                else if (rebate.Amount == 0)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    rebateAmount = rebate.Amount;
                    result.IsSuccess = true;
                }
                break;

            case IncentiveType.FixedRateRebate:
                if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
                {
                    result.IsSuccess = false;
                }
                else if (rebate.Percentage == 0 || product.Price == 0 || request.Volume == 0)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    rebateAmount += product.Price * rebate.Percentage * request.Volume;
                    result.IsSuccess = true;
                }
                break;

            case IncentiveType.AmountPerUom:
                if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
                {
                    result.IsSuccess = false;
                }
                else if (rebate.Amount == 0 || request.Volume == 0)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    rebateAmount += rebate.Amount * request.Volume;
                    result.IsSuccess = true;
                }
                break;
        }

        if (result.IsSuccess)
        {
            var storeRebateDataStore = new RebateDataStore();
            storeRebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }

        return result;
    }
}
