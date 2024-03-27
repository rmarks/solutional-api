using Microsoft.AspNetCore.Mvc;

namespace Solutional.Api.Features.Orders;

public static class AddOrderProducts
{
    public static WebApplication MapAddOrderProductsEndpoint(this WebApplication app)
    {
        app.MapPost("api/orders/{id}/products", async (Guid id, [FromBody] int[]? productIds, [FromServices] AppDbContext dbContext) =>
        {
            var order = await dbContext.Orders.FindAsync(id);
            if (order is null) return Results.NotFound("Not found");

            if (order.Status == "PAID") return Results.BadRequest("Invalid parameters");

            if (productIds is null || productIds.Count() == 0 || productIds.GroupBy(x => x).Any(g => g.Count() > 1))
            {
                return Results.BadRequest("Invalid parameters");
            }

            var products = await dbContext.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != productIds.Count()) return Results.BadRequest("Invalid parameters");

            var orderProducts = await dbContext.OrderProducts
            .Where(op => op.OrderId == id)
            .ToListAsync();

            foreach (var p in products)
            {
                var op = orderProducts.FirstOrDefault(op => op.ProductId == p.Id);
                if (op is not null)
                {
                    op.Quantity += 1;
                    
                }
                else
                {
                    var newOp = new OrderProduct
                    {
                        Name = p.Name,
                        Price = p.Price,
                        ProductId = p.Id,
                        Quantity = 1,
                        OrderId = id,
                    };

                    await dbContext.AddAsync(newOp);
                }

                order.Total += p.Price;
            }

            await dbContext.SaveChangesAsync();

            return Results.Ok("OK");
        })
        .Produces(200)
        .Produces(404)
        .Produces(400);

        return app;
    }
}
