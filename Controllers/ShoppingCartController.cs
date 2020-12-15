using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CartModuleApi.Models;
using CartModuleApi.Services;
using System.Threading.Tasks;
using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

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
                var cartItems = await _iCartService.Checkout(userId);
                PublishToQueue(cartItems);
                return Ok("Basket was queued");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        private static void PublishToQueue(IList<CartItem> cartItems)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("ShoppingCartExchange", ExchangeType.Fanout, true);
            //In a Fanount Exchane each queue will receive messages when subscribing to the exchange
            //The Exchange should be declared as Fanount and in the BasicPublish empty routingKey 
            //his being passed 
            //Each subscriber/consumer signed to the Exchange with the QueueBind method and passing empty string to the routingKey parameter

            //In a Direct Exchane each queue will receive messages when subscribing to the exchange
            //The Exchange should be declared as Direct and in the BasicPublish real routingKey 
            //is being passed 
            //Each subscriber/consumer signed to the Exchange with the QueueBind method and passing a real routingKey parameter

            //In a Topic Exchane each queue will receive messages when subscribing to the exchange
            //The Exchange should be declared as Topic and in the BasicPublish real routingKey 
            //is being passed 
            //Each subscriber/consumer signed to the Exchange with the QueueBind method and passing a real routingKey parameter
            //Or BaseRoute.* to recieve all messages for the Topic which is the base route.

            //In a Header Exchane each queue will receive messages when subscribing to the exchange
            //The Exchange should be declared as Headers and in the BasicPublish an empty routingKey 
            //is being passed , instead a dictionary is being passed to the basic properties parameter.
            //The dictionary contains a subject and action for example Cart.Sent, Cart.Delete etc..
            //The subject is Cart and the action is sent,delete...
            //Each subscriber/consumer signed to the Exchange with the QueueBind method and passing an empty routingKey 
            //Instead, for the forth parameter to the QueueBind, each consumer have to declare a dictionart contains
            //a subject an action and x-match key that can be all or any
            //If it is being subscribed with all it behave like in Direct Exchange the subject and the action should both match.
            //If it is being subscribed with any it behave like in a Topic exchange, the subject is matched and this is enough.
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cartItems));
            channel.BasicPublish("ShoppingCartExchange", "", null, bytes);
            channel.Close();
            connection.Close();
        }

    }
}
