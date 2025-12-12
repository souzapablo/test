using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public class FixedRateRebateCalculator: IRebateCalculator
{
    public IncentiveType Incentive => IncentiveType.FixedRateRebate;

    public CalculateRebateResult Calculate(RebateCalculationContext context)
    {
        if (context.RebatePercentage == 0 || context.ProductPrice == 0 || context.Volume == 0)
            return CalculateRebateResult.Failure();

        return CalculateRebateResult.Success(context.ProductPrice * context.Volume * context.RebatePercentage);
    }
}
