using System.Collections.Generic;
using CartModuleApi.Models;

namespace CartModuleApi.Services
{
    public interface ICartService
    {
        CartItem AddProductToCart(CartItem CartItem);
        IList<CartItem> ChangeProductQuantity(int userId,int productId,int quantity);
        IList<CartItem> ClearCart(int userId);
        IList<CartItem> DeleteProduct(int userId,int productId);
        IList<CartItem> GetCartItems(int userId);
    }
}