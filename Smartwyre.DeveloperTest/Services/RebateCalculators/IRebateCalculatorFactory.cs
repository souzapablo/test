using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public interface IRebateCalculatorFactory
{
    bool TryGet(IncentiveType incentive, out IRebateCalculator calculator);
}
