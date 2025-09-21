
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;

namespace APILayer.Endpoints;

public static class ProductAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        var productEndpoints = app.MapGroup("/api/v1/products");

        productEndpoints.MapGet("/", GetAllProducts);
        productEndpoints.MapGet("/{id}", GetProduct);
        productEndpoints.MapPost("/", CreateProduct);
        productEndpoints.MapPut("/{id}", UpdateProduct);
        productEndpoints.MapDelete("/{id}", DeleteProduct);

        return app;
    }

    static async Task<IResult> GetAllProducts(IProductService productService, string? searchString)
    {
        List<ProductResponse?> products;

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            products = await productService.GetProductsByCondition(temp =>
                                (temp.ProductName != null && temp.ProductName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                                (temp.Category != null && temp.Category.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
        }
        else
        {
            products = await productService.GetProducts();
        }

        return TypedResults.Ok(products);
    }
    static async Task<IResult> GetProduct(IProductService productService, Guid id)
    {
        var product = await productService.GetProductByCondition(temp => temp.ProductID == id);
        if (product == null)
        {
            return TypedResults.NotFound();
        }
        else
            return TypedResults.Ok(product);
    }

    static async Task<IResult> CreateProduct(IProductService productService, ProductAddRequest productAddRequest, IValidator<ProductAddRequest> productAddRequestValidator)
    {
        // Validation
        ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);

        if (!validationResult.IsValid)
        {
            Dictionary<string, string[]> errors = validationResult.Errors
                                    .GroupBy(temp => temp.PropertyName)
                                    .ToDictionary(grp => grp.Key, grp => grp
                                    .Select(err => err.ErrorMessage).ToArray());

            return TypedResults.ValidationProblem(errors);
        }

        var insertProduct = await productService.AddProduct(productAddRequest);
        if (insertProduct != null)
            return TypedResults.Created($"/api/v1/products/{insertProduct.ProductID}", insertProduct);
        else
            return TypedResults.Problem("Error in adding product");
    }
    static async Task<IResult> UpdateProduct(IProductService productService, Guid id, ProductUpdateRequest productUpdateRequest, IValidator<ProductUpdateRequest> productUpdateRequestValidator)
    {
        // Validation
        if (id != productUpdateRequest.ProductID)
            return TypedResults.BadRequest("ProductID in the URL doesn't match with the ProductID in the Request body");

        ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

        if (!validationResult.IsValid)
        {
            Dictionary<string, string[]> errors = validationResult.Errors
                                    .GroupBy(temp => temp.PropertyName)
                                    .ToDictionary(grp => grp.Key, grp => grp
                                    .Select(err => err.ErrorMessage).ToArray());

            return TypedResults.ValidationProblem(errors);
        }

        var updatedProduct = await productService.UpdateProduct(productUpdateRequest);

        if (updatedProduct != null)
            return TypedResults.Ok(updatedProduct);
        else
            return TypedResults.Problem("Error in updating product");
    }

    static async Task<IResult> DeleteProduct(IProductService productService, Guid id)
    {
        var IsDeleted = await productService.DeleteProduct(id);
        if (IsDeleted == true)
            return TypedResults.Ok(IsDeleted);
        else
            return TypedResults.Problem("Error in deleting product");
    }
}
