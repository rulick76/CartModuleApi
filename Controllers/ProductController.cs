using System;
using System.Threading.Tasks;
using CartModuleApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartModuleApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _iProductService;
        public ProductController(IProductService iProductService)
        {
            _iProductService = iProductService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var products =  _iProductService.Get();
                return Ok(products);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int productId)
        {
            try
            {
                var product =  await _iProductService.GetAsync(productId);

                if (product == null)
                {
                    return NotFound("Product not exist");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
