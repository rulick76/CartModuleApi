using CartModuleApi.Models;
using CartModuleApi.EntityFrameWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartModuleApi.Services
{
    public class ProductService : IProductService
    {
      

        private readonly EnityFramWorkDbContext _context;
        public ProductService(EnityFramWorkDbContext context)
        {
            _context = context;;
        }

        /// <summary>
        /// Get All List of Products from Product Table
        /// </summary>
        /// <returns>return list of products</returns>
        public  IList<Product> Get()
        {
            try
            {
                var products =   _context.Set<Product>();
                return products.ToList();
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
 
        }

        /// <summary>
        /// Get Product from product by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>return single product</returns>
        public async Task<Product> GetAsync(int productId)
        {
            try
            {
                return await _context.Set<Product>().FindAsync(productId);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
