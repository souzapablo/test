using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public class AmountPerUomCalculator : IRebateCalculator
{
    public IncentiveType Incentive => IncentiveType.AmountPerUom;

    public CalculateRebateResult Calculate(RebateCalculationContext context)
    {
        if (context.RebateAmount == 0 || context.Volume == 0)
            return CalculateRebateResult.Failure();

        return CalculateRebateResult.Success(context.RebateAmount * context.Volume);
    }
}
