using Buildify.Core.Entities;

namespace Buildify.Core.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart?> GetCartWithItemsByUserIdAsync(string userId);
        Task<CartItem?> GetCartItemAsync(int cartItemId);
        Task<bool> DeleteCartItemAsync(int cartItemId);
        Task<bool> ClearCartAsync(string userId);
    }
}
