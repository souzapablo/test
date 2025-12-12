using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Services.RebateCalculators;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        IRebateDataStore rebateDataStore = new RebateDataStore();
        IProductDataStore productDataStore = new ProductDataStore();

        var calculators = new List<IRebateCalculator>
        {
            new FixedCashAmountCalculator(),
            new FixedRateRebateCalculator(),
            new AmountPerUomCalculator()
        };

        IRebateCalculatorFactory calculatorFactory = new RebateCalculatorFactory(calculators);

        IRebateService rebateService = new RebateService(
            rebateDataStore,
            productDataStore,
            calculatorFactory);

        if (args.Length >= 3)
        {
            var request = new CalculateRebateRequest
            {
                RebateIdentifier = args[0],
                ProductIdentifier = args[1],
                Volume = decimal.TryParse(args[2], out decimal volume) ? volume : 0m
            };

            Console.WriteLine("Rebate Calculation");
            Console.WriteLine("==================");
            Console.WriteLine($"Rebate Identifier: {request.RebateIdentifier}");
            Console.WriteLine($"Product Identifier: {request.ProductIdentifier}");
            Console.WriteLine($"Volume: {request.Volume}");
            Console.WriteLine();

            CalculateRebateResult result = rebateService.Calculate(request);
            DisplayResult(result);
            return;
        }

        Console.WriteLine("Rebate Calculation");
        Console.WriteLine("==================");
        Console.WriteLine("Type 'quit' to exit");
        Console.WriteLine();

        while (true)
        {
            Console.Write("Enter Rebate Identifier (or 'quit' to exit): ");
            string rebateIdentifier = Console.ReadLine() ?? string.Empty;

            if (rebateIdentifier.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            Console.Write("Enter Product Identifier: ");
            string productIdentifier = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter Volume: ");
            string volumeInput = Console.ReadLine() ?? "0";
            decimal volume = decimal.TryParse(volumeInput, out decimal vol) ? vol : 0m;

            var request = new CalculateRebateRequest
            {
                RebateIdentifier = rebateIdentifier,
                ProductIdentifier = productIdentifier,
                Volume = volume
            };

            Console.WriteLine();
            Console.WriteLine($"Rebate Identifier: {request.RebateIdentifier}");
            Console.WriteLine($"Product Identifier: {request.ProductIdentifier}");
            Console.WriteLine($"Volume: {request.Volume}");
            Console.WriteLine();

            CalculateRebateResult result = rebateService.Calculate(request);
            DisplayResult(result);
            Console.WriteLine();
        }
    }

    static void DisplayResult(CalculateRebateResult result)
    {
        if (result.IsSuccess)
        {
            Console.WriteLine($"Rebate Amount: {result.RebateAmount:C}");
        }
        else
        {
            Console.WriteLine($"Failed to calculate rebate.");
        }
    }
}
