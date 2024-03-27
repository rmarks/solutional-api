using Solutional.Api.Features.Orders.Shared;

namespace Solutional.Api.Features.Orders;

public static class GetOrderProducts
{
    public static WebApplication MapGetOrderProductsEndpoint(this WebApplication app)
    {
        app.MapGet("api/orders/{id}/products", async (Guid id, AppDbContext dbContext) =>
        {
            bool orderExists = await dbContext.Orders.AnyAsync(o => o.Id == id);
            if (!orderExists) return Results.NotFound("Not found");

            var products = await dbContext.OrderProducts
                .AsNoTracking()
                .Where(x => x.OrderId == id && !x.IsReplacement)
                .Select(x => new OrderProductModel(
                    x.Id, 
                    x.Name, 
                    x.Price.ToString("F2"), 
                    x.ProductId, 
                    x.Quantity,
                    x.ReplacedWith != null
                        ? new OrderProductModel(
                            x.ReplacedWith.Id,
                            x.Name,
                            x.ReplacedWith.Price.ToString("F2"),
                            x.ReplacedWith.ProductId,
                            x.ReplacedWith.Quantity,
                            null)
                        : null))
                .ToListAsync();

            return Results.Ok(products);
        })
        .Produces<IEnumerable<OrderProductModel>>(200)
        .Produces(404);

        return app;
    }
}
