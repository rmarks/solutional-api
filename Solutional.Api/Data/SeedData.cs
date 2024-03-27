using Microsoft.EntityFrameworkCore;
using Solutional.Api.Data.Entities;

namespace Solutional.Api.Data;

public class SeedData
{
    public static async Task EnsurePopulated(AppDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        if (await dbContext.Products.AnyAsync()) return;

        await dbContext.Products.AddRangeAsync(
                new Product { Id = 123, Name = "Ketchup", Price = 0.45m },
                new Product { Id = 456, Name = "Beer", Price = 2.33m },
                new Product { Id = 879, Name = "Õllesnäkk", Price = 0.42m },
                new Product { Id = 999, Name = "75\" OLED TV", Price = 1333.37m }
            );

        await dbContext.SaveChangesAsync();
    }
}
