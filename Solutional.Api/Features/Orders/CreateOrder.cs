using Solutional.Api.Features.Orders.Shared;

namespace Solutional.Api.Features.Orders;

public static class CreateOrder
{
    public static WebApplication MapCreateOrderEndpoint(this WebApplication app)
    {
        app.MapPost("/api/orders", async (AppDbContext dbContext) =>
        {
            Order order = new() { Status = "NEW" };
            await dbContext.AddAsync(order);
            await dbContext.SaveChangesAsync();

            var orderModel = new OrderModel(
                order.Id, 
                order.Status,
                new OrderModel.AmountModel(
                    order.Discount.ToString("F2"),
                    order.Paid.ToString("F2"),
                    order.Returns.ToString("F2"),
                    order.Total.ToString("F2")), 
                Array.Empty<OrderProductModel>());

            return Results.Created($"/api/orders/{orderModel.Id}", orderModel);
        })
        .Produces<OrderModel>(201);

        return app;
    }
}
