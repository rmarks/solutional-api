namespace Solutional.Api.Features.Orders.Shared;

public record OrderModel(
    int Id, 
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
        int Id,
        string Name,
        string Price,
        int Product_id,
        int Quantity,
        OrderProductModel? Replaced_with);