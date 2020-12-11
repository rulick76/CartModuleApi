using CartModuleApi.Models;
using Microsoft.EntityFrameworkCore;
using CartModuleApi.EntityFrameWork;
using System.Collections.Generic;
using System.Linq;

namespace CartModuleApi.Services
{
    public class CartService : ICartService
    {
        private readonly EnityFramWorkDbContext _context;
        public CartService(EnityFramWorkDbContext context)
        {
            _context=context;
        }

        public CartItem AddProductToCart(CartItem cartItem)
        {
            CartItem realItem = _context.Set<CartItem>().Where(c=>c.UserId == cartItem.UserId  &&
                c.ProductId==cartItem.ProductId).FirstOrDefault();
            //User post new item to the cart
            //If the cart is empty we are generating the first item for this user with nextId
            //If this product exist in the cart we are just updating the Quantity
            if (realItem!=null)
            {
                // var bExistingItem = realItem.ProductId==cartItem.ProductId ? true :false;
                // if (bExistingItem)     
                    //Update Quantity
                    return UpdateExistingItem(cartItem, realItem);
            }

            int nextId =  _context.Set<CartItem>()
                .Max(ci => ci.Id) +1;
            
            realItem = CreateNewCartItem(cartItem, nextId);
            _context.Set<CartItem>().Add(realItem);
            Save();
            return realItem;
        }

        private static CartItem CreateNewCartItem(CartItem cartItem, int nextId)
        {
            CartItem realItem = new CartItem();
            realItem.Id = nextId;
            realItem.ProductId = cartItem.ProductId;
            realItem.Quantity = cartItem.Quantity;
            realItem.UserId = cartItem.UserId;
            return realItem;
        }

        private CartItem UpdateExistingItem(CartItem cartItem, CartItem existingItem)
        {
            existingItem.Quantity += cartItem.Quantity;
            Update(existingItem);
            Save();
            return existingItem;
        }

        public IList<CartItem> GetCartItems(int userId)
        {
            var cartItems =  _context.Set<CartItem>().Where(c => c.UserId == userId);
            return PopulateItems(cartItems.ToList());
        }

        public IList<CartItem> ClearCart(int userId)
        {
            var cartItems = _context.Set<CartItem>().Where(b => b.UserId == userId);

            foreach (var cartItem in cartItems)
            {
                Delete(cartItem);
            }
            Save();
            return GetCartItems(userId);
        }

        public IList<CartItem> DeleteProduct(int userId,int productId)
        {
            var cartItem =_context.Set<CartItem>().Where(ci=>ci.UserId==userId && 
            ci.ProductId==productId).FirstOrDefault();
            if (cartItem != null)
            {
               Delete(cartItem);
               Save();
            }
            return GetCartItems(cartItem.UserId);
        }

        public IList<CartItem> ChangeProductQuantity(int userId,int productId, int quantity)
        {
            var cartItem =_context.Set<CartItem>().Where(ci=>ci.UserId==userId && 
            ci.ProductId==productId).FirstOrDefault();

            if (cartItem == null)
                return null;

            cartItem.Quantity = quantity;

            Update<CartItem>(cartItem);
            Save();
            return GetCartItems(cartItem.UserId);
        }

        private List<CartItem> PopulateItems(List<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                cartItem.Product = _context.Set<Product>().Find(cartItem.ProductId);
            }
            return cartItems;
        }

        public virtual void Save()
        {
            try
            {
                 _context.SaveChanges();//asy
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }

        public virtual void Update<CartItem>(CartItem entity, string modifiedBy = null)
            where CartItem : class
        {
            _context.Set<CartItem>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete<CartItem>(CartItem entity)
            where CartItem : class
        {
            var dbSet = _context.Set<CartItem>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }
    }
}
