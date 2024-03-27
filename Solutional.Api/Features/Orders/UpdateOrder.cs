using Microsoft.AspNetCore.Mvc;

namespace Solutional.Api.Features.Orders;

public static class UpdateOrder
{
    public static WebApplication MapUpdateOrderEndpoint(this WebApplication app)
    {
        app.MapPatch("api/orders/{id}", async (Guid id, [FromBody] OrderUpdateModel? model, [FromServices] AppDbContext dbContext) =>
        {
            var order = await dbContext.Orders.FindAsync(id);
            if (order is null) return Results.NotFound("Not found");

            if (model is null || model.Status != "PAID") return Results.BadRequest("Invalid order status");

            order.Status = model.Status;
            order.Paid = order.Total;
            await dbContext.SaveChangesAsync();

            return Results.Ok("OK");
        })
        .Produces(200)
        .Produces(404)
        .Produces(400);
        
        return app;
    }
}

public record OrderUpdateModel(string Status);
