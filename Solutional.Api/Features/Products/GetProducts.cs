namespace Solutional.Api.Features.Products;

public static class GetProducts
{
    public static WebApplication MapGetProductsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/products", async (AppDbContext dbContext) =>
        {
            return await dbContext.Products
                .AsNoTracking()
                .Select(p => new ProductModel(p.Id, p.Name, p.Price.ToString("F2")))
                .ToListAsync();
        });

        return app;
    }
}

public record ProductModel(int Id, string Name, string Price);
