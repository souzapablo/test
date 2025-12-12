using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore
{
    public Product GetProduct(string productIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        return new Product();
    }
}

public interface IProductDataStore
{
    Product GetProduct(string productIdentifier);
}