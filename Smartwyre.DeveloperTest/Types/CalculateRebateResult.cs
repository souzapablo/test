namespace Smartwyre.DeveloperTest.Types;

public class CalculateRebateResult
{
    public bool IsSuccess { get; set; }

    public static CalculateRebateResult Failure() => new() { IsSuccess = false };
}
