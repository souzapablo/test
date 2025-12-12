## Implementation Notes

### Design Decisions

- **Strategy Pattern**: Each incentive type has its own calculator class implementing `IRebateCalculator`, making it easy to add new incentive types without modifying existing code.
- **Factory Pattern**: `RebateCalculatorFactory` manages calculator instances and provides them based on incentive type.
- **Dependency Injection**: All dependencies are injected through constructors, making the code highly testable.
- **Single Responsibility**: Each calculator class is responsible for only one incentive type's calculation logic.

### Adding New Incentive Types

To add a new incentive type:

1. Add the new type to the `IncentiveType` enum
2. Add the corresponding flag to `SupportedIncentiveType` enum
3. Create a new calculator class implementing `IRebateCalculator`:
   ```csharp
   public class NewIncentiveCalculator : IRebateCalculator
   {
       public IncentiveType Incentive => IncentiveType.NewIncentive;
       
       public CalculateRebateResult Calculate(RebateCalculationContext context)
       {
           // Implementation
       }
   }
   ```
4. Register the calculator in the `RebateCalculatorFactory` (in `Program.cs` for the runner, or via DI container in production)

No changes are required to `RebateService`.

### Running the Application

**Command Line Arguments:**
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner/Smartwyre.DeveloperTest.Runner.csproj -- "REBATE-IDENTIFIER" "PRODUCT-IDENTIFIER" "VOLUME"
```

**Interactive Mode:**
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner/Smartwyre.DeveloperTest.Runner.csproj
```
The application will prompt for inputs and continue until you type `quit`.

### Test Data

The data stores contain sample data for testing:

**Rebates:**
- `FIXED-RATE-REBATE-001` - Fixed Rate Rebate (15% percentage)
- `AMOUNT-PER-UOM-001` - Amount Per UOM ($5.50 per unit)
- `FIXED-CASH-AMOUNT-001` - Fixed Cash Amount ($100.00)

**Products:**
- `PRODUCT-FIXED-RATE` - Supports FixedRateRebate
- `PRODUCT-AMOUNT-PER-UOM` - Supports AmountPerUom
- `PRODUCT-FIXED-CASH` - Supports FixedCashAmount

### Running Tests

```bash
dotnet test
```

The test suite includes comprehensive unit tests using xUnit and NSubstitute, covering:
- Success scenarios for all three incentive types
- Failure scenarios (missing rebate/product, invalid data, unsupported incentives)
- Edge cases and boundary conditions