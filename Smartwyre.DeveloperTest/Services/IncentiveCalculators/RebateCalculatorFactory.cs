using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.IncentiveCalculators;

public class RebateCalculatorFactory : IRebateCalculatorFactory
{
    private readonly IReadOnlyDictionary<IncentiveType, IRebateCalculator> _byType;

    public RebateCalculatorFactory(IEnumerable<IRebateCalculator> calculators)
    {
        _byType = calculators.ToDictionary(c => c.Incentive, c => c);
    }

    public bool TryGet(IncentiveType incentive, out IRebateCalculator calculator) =>
        _byType.TryGetValue(incentive, out calculator);
}
