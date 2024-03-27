namespace Solutional.Api.Features.Orders.Shared;

public record OrderModel(
    Guid Id, 
    string Status, 
    OrderModel.AmountModel Amount, 
    IEnumerable<OrderProductModel> Products)
{
    public record AmountModel(
        string Discount,
        string Paid,
        string Returns,
        string Total);
}

public record OrderProductModel(
        Guid Id,
        string Name,
        string Price,
        int Product_id,
        int Quantity,
        OrderProductModel? Replaced_with);