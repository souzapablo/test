using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore : IProductDataStore
{
    private readonly Dictionary<string, Product> _products = new()
    {
        {
            "PRODUCT-FIXED-RATE",
            new Product
            {
                Id = 1,
                Identifier = "PRODUCT-FIXED-RATE",
                Price = 50.00m,
                Uom = "Unit",
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate
            }
        },
        {
            "PRODUCT-AMOUNT-PER-UOM",
            new Product
            {
                Id = 2,
                Identifier = "PRODUCT-AMOUNT-PER-UOM",
                Price = 30.00m,
                Uom = "Unit",
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            }
        },
        {
            "PRODUCT-FIXED-CASH",
            new Product
            {
                Id = 3,
                Identifier = "PRODUCT-FIXED-CASH",
                Price = 25.00m,
                Uom = "Unit",
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            }
        }
    };

    public Product GetProduct(string productIdentifier)
    {
        if (_products.TryGetValue(productIdentifier, out var product))
        {
            return product;
        }
        
        Console.WriteLine($"  [DataStore] Product not found: {productIdentifier}");
        return null;
    }
}

public interface IProductDataStore
{
    Product GetProduct(string productIdentifier);
}