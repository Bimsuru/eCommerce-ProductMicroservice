using System.Linq.Expressions;
using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Product?> AddProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;

    }
    public async Task<IEnumerable<Product?>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<bool> DeleteProduct(Guid productID)
    {
        var exittingProduct = await _context.Products.FirstOrDefaultAsync(temp => temp.ProductID == productID);

        if (exittingProduct != null)
        {
            _context.Products.Remove(exittingProduct);
            int affectedRowsCount = await _context.SaveChangesAsync();
            return affectedRowsCount > 0;
        }
        else
            return false;
    }

    public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _context.Products.FirstOrDefaultAsync(conditionExpression);
    }

    public async Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _context.Products.Where(conditionExpression).ToListAsync();
    }

    public async Task<Product?> UpdateProduct(Product product)
    {
        var exittingProduct = await _context.Products.FirstOrDefaultAsync(temp => temp.ProductID == product.ProductID);

        if (exittingProduct != null)
        {
            exittingProduct.ProductName = product.ProductName;
            exittingProduct.Category = product.Category;
            exittingProduct.UnitPrice = product.UnitPrice;
            exittingProduct.QuantityInStock = product.QuantityInStock;

             // Use AutoMapper to map new values onto the existing entity
            // _mapper.Map(product, existingProduct);

            _context.Products.Update(exittingProduct);
            await _context.SaveChangesAsync();
            return exittingProduct;
        }
        else
            return null;
    }
}
