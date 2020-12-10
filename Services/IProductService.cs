using CartModuleApi.Models;
using System.Collections.Generic;

namespace CartModuleApi.Services
{
    public interface IProductService
    {
        IList<Product> Get();        
        Product Get(int productId);
    }
}
