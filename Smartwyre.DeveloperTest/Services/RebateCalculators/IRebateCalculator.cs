using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public interface IRebateCalculator
{
    IncentiveType Incentive { get; }
    public CalculateRebateResult Calculate(decimal rebateAmount, decimal productPrice, decimal? volume = null);
}
