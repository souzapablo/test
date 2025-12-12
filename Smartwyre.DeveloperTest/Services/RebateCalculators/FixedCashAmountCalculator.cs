
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public class FixedCashAmountCalculator: IRebateCalculator
{
    public IncentiveType Incentive => IncentiveType.FixedCashAmount;

    public CalculateRebateResult Calculate(RebateCalculationContext context)
    {
        if (context.RebateAmount == 0)
            return CalculateRebateResult.Failure();
        
        return CalculateRebateResult.Success(context.RebateAmount);
    }
}