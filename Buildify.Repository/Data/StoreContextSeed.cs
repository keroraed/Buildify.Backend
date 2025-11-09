namespace Buildify.Repository.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        // Add seed data for your business entities here
        await context.SaveChangesAsync();
    }
}
