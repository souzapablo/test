namespace Smartwyre.DeveloperTest.Types;

public class CalculateRebateResult
{
    public bool IsSuccess { get; set; }
    public decimal RebateAmount { get; set; }

    public static CalculateRebateResult Failure() => new() { IsSuccess = false };
    public static CalculateRebateResult Success(decimal rebateAmount) => 
        new() 
        {    
            IsSuccess = true, 
            RebateAmount = rebateAmount 
        };
}
