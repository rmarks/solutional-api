using Microsoft.AspNetCore.Mvc;

namespace Solutional.Api.Features.Orders;

public static class UpdateOrderProduct
{
    public static WebApplication MapUpdateOrderProductEndpoint(this WebApplication app)
    {
        app.MapPatch("api/orders/{orderId}/products/{orderProductId}", async (
            int orderId,
            int orderProductId,
            [FromBody] OrderProductUpdateModel? updateModel,
            [FromServices] AppDbContext dbContext) =>
        {
            var order = await dbContext.Orders.FindAsync(orderId);
            
            if (order is null) return Results.NotFound("Not found");

            var orderProduct = await dbContext.OrderProducts
                .FirstOrDefaultAsync(x => x.OrderId == orderId && x.Id == orderProductId);
            
            if (orderProduct is null) return Results.NotFound("Not found");

            if (updateModel is null) return Results.BadRequest("Invalid parameters");

            if (order.Status == "NEW")
            {
                if (updateModel.Quantity is null || updateModel.Quantity < 0)
                {
                    return Results.BadRequest("Invalid parameters");
                }

                int newQuantity = (int)updateModel.Quantity;
                order.Total = order.Total - (orderProduct.Quantity * orderProduct.Price) + (newQuantity * orderProduct.Price);
                orderProduct.Quantity = newQuantity;
                await dbContext.SaveChangesAsync();

                return Results.Ok("OK");
            }
            else if (order.Status == "PAID")
            {
                if (updateModel.Replaced_with is null)
                {
                    return Results.BadRequest("Invalid parameters");
                }

                var product = await dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == updateModel.Replaced_with.Product_id);
                
                if (product is null) return Results.BadRequest("Invalid parameters");

                var newOrderProduct = new OrderProduct
                {
                    OrderId = orderId,
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = updateModel.Replaced_with.Quantity,
                    IsReplacement = true,
                };

                orderProduct.ReplacedWith = newOrderProduct;

                var newTotal = newOrderProduct.Quantity * newOrderProduct.Price;
                var totalDiff = newTotal - order.Total;
                order.Discount = totalDiff > 0 ? totalDiff : 0;
                order.Returns = totalDiff < 0 ? Math.Abs(totalDiff) : 0;

                await dbContext.SaveChangesAsync();

                return Results.Ok("OK");
            }

            return Results.BadRequest("Invalid parameters");
        })
        .Produces(200)
        .Produces(404)
        .Produces(400);

        return app;
    }
}

public record OrderProductUpdateModel(int? Quantity, OrderProductUpdateModel.ReplacedWith? Replaced_with)
{
    public record ReplacedWith(int Product_id, int Quantity);
}
