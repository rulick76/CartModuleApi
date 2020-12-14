using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CartModuleApi.Models;
using CartModuleApi.Services;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CartModuleApi.Controllers
{
    [Route("api/[controller]")]
    public class ShoppingCartController : Controller
    {
        private readonly ICartService _iCartService;
        public ShoppingCartController(ICartService CartService)
        {
            _iCartService = CartService;
        }

        // GET: api/ShoppingCart/GetCartItems
        [HttpGet("GetItems/{userId}")]
        public async Task<IActionResult> GetCartItems(int userId)
        {
            try
            {
                IList<CartItem> cartItems = await _iCartService.GetCartItemsAsync(userId);    
                if (cartItems == null || cartItems.Count==0)
                {
                    return NotFound("User not found");
                }       
                return Ok(cartItems);  
            }
            catch (System.Exception ex)
            {
                
               return BadRequest(ex.Message);
            }
         
        }


        // POST api/ShoppingCart
        [HttpPost]
        public async  Task<IActionResult> Post(CartItemPost cartItem)
        {       
            try
            {
                await _iCartService.AddProductToCartAsync(cartItem);
                return Created($"ShoppingCart", cartItem);
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(ex.Message);
            }

        }

        // PUT api/ShoppingCart/ChangeItemQuantity/5/4/3
        [HttpPut("ChangeQuantity/{userId}/{productId}/{quantity}")]
        public async Task<IActionResult> ChangeProductQuantity(int userId ,int productId, int quantity)
        {
            try
            {
                CartItem cartItem=await _iCartService.ChangeProductQuantityAsync(userId,productId,quantity);
                if (cartItem == null)
                {
                    return NotFound("User with this product not found");
                }  
                return Ok(cartItem);
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(ex.Message);
            }

        }

        // DELETE api/ShoppingCart/ClearCart/1
        [HttpDelete("ClearCart/{userId}")]
        public async Task<IActionResult> ClearCart (int userId)
        {
            try
            {
                bool bClear=  await _iCartService.ClearCartAsync(userId);
                if (!bClear)
                {
                    return NotFound("User not found");
                }
                return Ok("Cart cleared for user " + userId); 
            }
            catch (System.Exception ex) 
            {
                
                return BadRequest(ex.Message);
            }

        }

        // DELETE api/ShoppingCart/DeleteItem/5
        [HttpDelete("DeleteItem/{userId}/{productId}")]
        public  async Task<IActionResult> DeleteProduct(int userId,int productId)
        {
            try
            {
                CartItem cartItem=await _iCartService.DeleteProductAsync(userId,productId);
                if (cartItem == null)
                {
                    return NotFound("User with this product not found");
                }  
                return Ok("ProductId:" + productId + " Deleted  for user " + userId);
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(ex.Message);
            }

        }
        
                // DELETE api/ShoppingCart/DeleteItem/5
        [HttpPost("Checkout/userid")]
        public async Task<IActionResult> Checkout(int userId)
        {
            try
            {
                bool bQueued= await _iCartService.Checkout(userId);
                return Ok("Basket was queued"); 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        
    }
}
