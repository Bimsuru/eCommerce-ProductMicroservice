namespace BusinessLogicLayer.DTO;

public record ProductResponse(
    Guid ProductID,
    string? ProductName,
    CategoryOptions Category,
    double UnitPrice,
    int QuantityInStock
)
{
    // Parameterless constructor
    // Needed for AutoMapper & model binding
    public ProductResponse() : this(default, default, default, default, default)
    {
        
    }
}
