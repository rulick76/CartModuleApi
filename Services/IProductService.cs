using CartModuleApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartModuleApi.Services
{
    public interface IProductService
    {
        IList<Product> Get();        
        Task<Product> GetAsync(int productId);
    }
}
