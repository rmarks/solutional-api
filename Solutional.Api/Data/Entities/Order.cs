using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solutional.Api.Data.Entities;

public class Order
{
    public Guid Id { get; set; }

    [MaxLength(10)]
    public string Status { get; set; } = string.Empty;

    [Column(TypeName = "decimal(9,2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(9,2)")]
    public decimal Paid { get; set; }

    [Column(TypeName = "decimal(9,2)")]
    public decimal Returns { get; set; }

    [Column(TypeName = "decimal(9,2)")]
    public decimal Total { get; set; }

    public ICollection<OrderProduct> Products { get; set; } = new List<OrderProduct>();
}
