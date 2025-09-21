
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities;

public class Product
{
    [Key]
    public Guid ProductID { get; set; }
    public string ProductName { get; set; } = null!;
    public string Category { get; set; } = null!;
    public double? UnitPrice { get; set; }
    public int? QuantityInStock { get; set; }
}
