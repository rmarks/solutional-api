using Microsoft.AspNetCore.Mvc;

namespace Solutional.Api.Features.Orders;

public static class ReplaceOrderProduct
{
    // only if order PAID
    // OrderNotFound => NotFound
    // OrderProductNotFound => NotFound
    // ReplaceModelIsNull(OrEmpty?) => BadRequest
    // ProductNotFound => BadRequest("Invalid parameters")
    // võib olla sama või erinev toode
    // saab lõputult üle kopeerida
    // hinnavahe läheb discount-i või return-i
    // õnnestumisel => Ok("OK")

    public static WebApplication MapReplaceOrderProductEndpoint(this WebApplication app)
    {
        app.MapPatch("api/orders/{orderId}/products/{orderProductId}", async (
            int orderId,
            int orderProductId,
            [FromBody] ReplacedWithModel? replaceModel,
            [FromServices] AppDbContext dbContext) =>
        {
            var order = await dbContext.Orders.FindAsync(orderId);
            if (order is null) return Results.NotFound("Not found");

            if (order.Status != "PAID") return Results.BadRequest("Invalid parameters");

            var orderProduct = await dbContext.OrderProducts
                .FirstOrDefaultAsync(x => x.OrderId == orderId && x.Id == orderProductId);
            if (orderProduct is null) return Results.NotFound("Not found");

            if (replaceModel is null) return Results.BadRequest("Invalid parameters");

            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == replaceModel.Replaced_with.Product_id);
            if (product is null) return Results.BadRequest("Invalid parameters");

            var newOrderProduct = new OrderProduct
            {
                OrderId = orderId,
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = replaceModel.Replaced_with.Quantity,
                IsReplacement = true,
            };

            orderProduct.ReplacedWith = newOrderProduct;

            var newTotal = newOrderProduct.Quantity * newOrderProduct.Price;
            var totalDiff = newTotal - order.Total;
            order.Discount = totalDiff > 0 ? totalDiff : 0;
            order.Returns = totalDiff < 0 ? Math.Abs(totalDiff) : 0;

            await dbContext.SaveChangesAsync();

            return Results.Ok("OK");
        });
        
        return app;
    }
}

public record ReplacedWithModel(ShortProductModel Replaced_with);

public record ShortProductModel(int Product_id, int Quantity);