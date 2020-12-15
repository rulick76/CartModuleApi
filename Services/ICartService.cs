using System.Collections.Generic;
using System.Threading.Tasks;
using CartModuleApi.Models;

namespace CartModuleApi.Services
{
    public interface ICartService
    {
        Task<CartItem> AddProductToCartAsync(CartItemPost CartItem);
        Task<CartItem> ChangeProductQuantityAsync(int userId,int productId,int quantity);
        Task<bool> ClearCartAsync(int userId);
        Task<CartItem> DeleteProductAsync(int userId,int productId);
        Task<IList<CartItem>> GetCartItemsAsync(int userId);
        Task<IList<CartItem>> Checkout(int userid);
    }
}