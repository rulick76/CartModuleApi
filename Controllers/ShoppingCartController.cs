using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CartModuleApi.Models;
using CartModuleApi.Services;

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
        public  IActionResult GetCartItems(int userId)
        {
            IList<CartItem> cartItems =  _iCartService.GetCartItems(userId);           
            return Ok(cartItems);           
        }


        // POST api/ShoppingCart
        [HttpPost]
        public IActionResult Post(CartItem cartItem)
        {       
             _iCartService.AddProductToCart(cartItem);
            return Created($"ShoppingCart", cartItem);
        }

        // PUT api/ShoppingCart/ChangeItemQuantity/5/4/3
        [HttpPut("ChangeQuantity/{userId}/{productId}/{quantity}")]
        public IActionResult ChangeProductQuantity(int userId ,int productId, int quantity)
        {
            IList<CartItem> cartItems = _iCartService.ChangeProductQuantity(userId,productId,quantity);
            if (cartItems == null)
                return NotFound("Item not found in the cart, please check the productId"); 
            return Ok(cartItems);
        }

        // DELETE api/ShoppingCart/ClearCart/1
        [HttpDelete("ClearCart/{userId}")]
        public IActionResult ClearCart (int userId)
        {
            IList<CartItem> cartItems =  _iCartService.ClearCart(userId);
            return Ok(cartItems);
        }

        // DELETE api/ShoppingCart/DeleteItem/5
        [HttpDelete("DeleteItem/{userId}/{productId}")]
        public  IActionResult DeleteProduct(int userId,int productId)
        {
            IList<CartItem> cartItems =  _iCartService.DeleteProduct(userId,productId);
            if (cartItems == null)
                return NotFound("Item not found in the Cart, please check the productId");
            return Ok(cartItems);
        }
        

        
    }
}
