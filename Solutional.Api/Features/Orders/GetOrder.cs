using Solutional.Api.Features.Orders.Shared;

namespace Solutional.Api.Features.Orders;

public static class GetOrder
{
    public static WebApplication MapGetOrderEndpoint(this WebApplication app)
    {
        app.MapGet("api/orders/{id}", async (int id, AppDbContext dbContext) =>
        {
            var orderModel = await dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new OrderModel(
                    o.Id,
                    o.Status,
                    new OrderModel.AmountModel(
                        o.Discount.ToString("F2"), 
                        o.Paid.ToString("F2"), 
                        o.Returns.ToString("F2"), 
                        o.Total.ToString("F2")),
                    o.Products
                        .Where(op => !op.IsReplacement)
                        .Select(op => new OrderProductModel(
                            op.Id, 
                            op.Name, 
                            op.Price.ToString("F2"), 
                            op.ProductId, 
                            op.Quantity,
                            op.ReplacedWith != null 
                                ? new OrderProductModel(
                                    op.ReplacedWith.Id,
                                    op.ReplacedWith.Name,
                                    op.ReplacedWith.Price.ToString("F2"),
                                    op.ReplacedWith.ProductId,
                                    op.ReplacedWith.Quantity,
                                    null)
                                : null
                        ))))
                .FirstOrDefaultAsync();

            if (orderModel is null) return Results.NotFound("Not found");

            return Results.Ok(orderModel);
        })
        .Produces<OrderModel>(200)
        .Produces(404);

        return app;
    }
}
