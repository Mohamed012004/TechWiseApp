using Microsoft.EntityFrameworkCore;

namespace TechWise.Persistence.Store
{
    public static class StoreDbInitializer
    {

        public static async Task SeedAsync(StoreDbContext context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
