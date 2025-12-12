using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore : IRebateDataStore
{
    private readonly Dictionary<string, Rebate> _rebates = new()
    {
        {
            "FIXED-RATE",
            new Rebate
            {
                Identifier = "FIXED-RATE",
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = 0.15m
            }
        },
        {
            "AMOUNT-PER-UOM",
            new Rebate
            {
                Identifier = "AMOUNT-PER-UOM",
                Incentive = IncentiveType.AmountPerUom,
                Amount = 5.50m
            }
        },
        {
            "FIXED-CASH",
            new Rebate
            {
                Identifier = "FIXED-CASH",
                Incentive = IncentiveType.FixedCashAmount,
                Amount = 100.00m
            }
        }
    };

    public Rebate GetRebate(string rebateIdentifier)
    {
        if (_rebates.TryGetValue(rebateIdentifier, out var rebate))
        {
            return rebate;
        }

        Console.WriteLine($"  [DataStore] Rebate not found: {rebateIdentifier}");
        
        return null;
    }

    public void StoreCalculationResult(Rebate account, decimal rebateAmount)
    {
        // Update account in database, code removed for brevity
        Console.WriteLine($"  [DataStore] Stored calculation result: Rebate {account.Identifier} = {rebateAmount:C}");
    }
}

public interface IRebateDataStore
{
    Rebate GetRebate(string rebateIdentifier);
    void StoreCalculationResult(Rebate account, decimal rebateAmount);
}