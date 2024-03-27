using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solutional.Api.Data.Entities;

public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(9, 2)")]
    public decimal Price { get; set; }
}
