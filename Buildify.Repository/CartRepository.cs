using Buildify.Core.Entities;
using Buildify.Core.Repositories;
using Buildify.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Buildify.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly StoreContext _context;

        public CartRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartItem?> GetCartItemAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        public async Task<bool> DeleteCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
                return false;

            _context.CartItems.RemoveRange(cart.Items);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
