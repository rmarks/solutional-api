using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solutional.Api.Data.Entities;

public class OrderProduct
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(9, 2)")]
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }

    public int? ReplacedWithId { get; set; }
    public OrderProduct? ReplacedWith { get; set; }

    public bool IsReplacement { get; set; }
}
