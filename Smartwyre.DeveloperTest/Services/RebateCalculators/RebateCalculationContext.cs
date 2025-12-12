using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public class RebateCalculationContext
{
    public decimal RebateAmount { get; init; }
    public decimal ProductPrice { get; init; }
    public decimal Volume { get; init; }
    public decimal RebatePercentage { get; init; }

    public static RebateCalculationContext Create(Product product, Rebate rebate, decimal volume)
    {
        return new RebateCalculationContext
        {
            RebateAmount = rebate.Amount,
            ProductPrice = product.Price,
            Volume = volume,
            RebatePercentage = rebate.Percentage
        };
    }
}
