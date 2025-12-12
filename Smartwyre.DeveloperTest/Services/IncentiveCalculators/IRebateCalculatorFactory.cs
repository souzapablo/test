using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.IncentiveCalculators;

public interface IRebateCalculatorFactory
{
    bool TryGet(IncentiveType incentive, out IRebateCalculator calculator);
}
