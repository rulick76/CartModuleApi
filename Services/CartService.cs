using CartModuleApi.Models;
using Microsoft.EntityFrameworkCore;
using CartModuleApi.EntityFrameWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartModuleApi.Services
{
    public class CartService : ICartService
    {
        private readonly EnityFramWorkDbContext _context;
        public CartService(EnityFramWorkDbContext context)
        {
            _context=context;
        }

        public async Task<CartItem> AddProductToCartAsync(CartItemPost cartItem)
        {
            try
            {
                CartItem realItem = await _context.Set<CartItem>().Where(c=>c.UserId == cartItem.UserId  &&
                c.ProductId==cartItem.ProductId && c.Active).FirstOrDefaultAsync();
                //User post new item to the cart
                //If the cart is empty we are generating the first item for this user with nextId
                //If this product exist in the cart we are just updating the Quantity
                if (realItem!=null)
                {
                    await UpdateExistingItemAsync(cartItem, realItem);
                    return realItem;
                }

                int nextId =  _context.Set<CartItem>()
                    .Max(ci => ci.Id) +1;
                
                realItem =  CreateNewCartItem(cartItem, nextId);
                await _context.Set<CartItem>().AddAsync(realItem);
                await SaveAsync();
                return realItem;
            }
            catch (System.Exception ex)
            {   
                throw ex;
            }
        }

        private static CartItem CreateNewCartItem(CartItemPost cartItem, int nextId)
        {
            CartItem realItem = new CartItem();
            realItem.Id = nextId;
            realItem.ProductId = cartItem.ProductId;
            realItem.Quantity = cartItem.Quantity;
            realItem.UserId = cartItem.UserId;
            realItem.Active=true;
            return realItem;
        }

        private async Task<CartItem> UpdateExistingItemAsync(CartItemPost cartItem, CartItem existingItem)
        {
            existingItem.Quantity += cartItem.Quantity;
            Update(existingItem);
            await SaveAsync();
            return existingItem;
        }

        public async Task<IList<CartItem>> GetCartItemsAsync(int userId)
        {
            try
            {
                var cartItems =  await(from c in _context.CartItems
                                    where c.UserId==userId &&
                                    c.Active
                                    orderby c.ProductId
                                    select c).ToListAsync();
                PopulateItemsAsync(cartItems);
                return cartItems;   
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                var cartItems = _context.Set<CartItem>().Where(ci => ci.UserId == userId && ci.Active );
                bool bExist=false;
                if(cartItems.Count() >0)
                {
                    bExist=true;
                    _context.RemoveRange(cartItems);
                    await SaveAsync();
                }
                return bExist;
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
        }

        public async Task<CartItem> DeleteProductAsync(int userId,int productId)
        {
            try
            {
                var cartItem =await _context.Set<CartItem>().Where(ci=>ci.UserId==userId && 
                ci.ProductId==productId && ci.Active).FirstOrDefaultAsync();
                if (cartItem != null)
                {
                Delete(cartItem);
                await SaveAsync();
                }
                return cartItem; 
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }
        }

        public async Task<CartItem> ChangeProductQuantityAsync(int userId,int productId, int quantity)
        {
            try
            {
                var cartItem =await _context.Set<CartItem>().Where(ci=>ci.UserId==userId && 
                ci.ProductId==productId && ci.Active).FirstOrDefaultAsync();

                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    Update<CartItem>(cartItem);
                    await SaveAsync();
                }
                return cartItem;
            }
            catch (System.Exception ex)
            {   
                throw ex;
            }
        }
        private void PopulateItemsAsync(List<CartItem> cartItems)
        {
            var parallelAttach=new List<Task>();
            foreach (var cartItem in cartItems)
            {
                  parallelAttach.Add(AttachProduct(cartItem)) ;
            }
            Task.WaitAll(parallelAttach.ToArray());
        }

        public async Task SaveAsync()
        {
            try
            {
                await  _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }
        }


        private async Task AttachProduct(CartItem cart)
        {
            cart.Product = await _context.Set<Product>().FindAsync(cart.ProductId);
        }

        private void Update<CartItem>(CartItem entity, string modifiedBy = null)
            where CartItem : class
        {
            _context.Set<CartItem>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        private void Delete<CartItem>(CartItem entity)
            where CartItem : class
        {
            var dbSet = _context.Set<CartItem>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public async Task<bool> Checkout(int userId)
        {
            try
            {
                //Publish the basket to a broker queue for the payment service subscriber 
                var cartItems = await _context.Set<CartItem>().Where(ci => ci.UserId == userId && ci.Active).ToListAsync();
                cartItems.ForEach(ci =>
                        {
                            ci.Active=false;
                        });
                await SaveAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }

        }
    }
}
