using System.Linq.Expressions;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.ServiceContracts;

public interface IProductService
{
    Task<List<ProductResponse?>> GetProducts();
    Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);
    Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);
    Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest);
    Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest);
    Task<bool> DeleteProduct(Guid productid);
}
