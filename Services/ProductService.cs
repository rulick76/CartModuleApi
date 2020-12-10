using CartModuleApi.Models;
using CartModuleApi.EntityFrameWork;
using System.Collections.Generic;
using System.Linq;

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
        public IList<Product> Get()
        {
            var products = _context.Set<Product>();
            return products.ToList();
        }

        /// <summary>
        /// Get Product from product by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>return single product</returns>
        public Product Get(int productId)
        {
            var product = _context.Set<Product>().Find(productId);
            return product;
        }
    }
}
