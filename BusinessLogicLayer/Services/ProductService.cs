using System.Linq.Expressions;
using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogicLayer.Services;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<ProductAddRequest> _productAddRequestvalidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequestvalidator;
    public ProductService(IMapper mapper, IProductRepository productRepository, IValidator<ProductAddRequest> productAddRequestvalidator, IValidator<ProductUpdateRequest> productUpdateRequestvalidator)
    {
        _mapper = mapper;
        _productRepository = productRepository;
        _productAddRequestvalidator = productAddRequestvalidator;
        _productUpdateRequestvalidator = productUpdateRequestvalidator;
    }
    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        if (productAddRequest == null)
        {
            throw new ArgumentNullException(nameof(productAddRequest));
        }

        // FluentValidation
        ValidationResult result = await _productAddRequestvalidator.ValidateAsync(productAddRequest);

        // check result valid or not
        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(temp => temp.ErrorMessage)); // error1, error2, ...
            throw new ArgumentException(errors);
        }

        // mapped productAddRequest into product
        var product = _mapper.Map<Product>(productAddRequest);

        var addedProduct = await _productRepository.AddProduct(product);

        if (addedProduct != null)
        {
            return _mapper.Map<ProductResponse>(addedProduct);
        }
        else
            return null;

    }

    public async Task<bool> DeleteProduct(Guid productid)
    {
        // check productid in db 
        Product? exittingProduct = await _productRepository.GetProductByCondition(temp => temp.ProductID == productid);

        if (exittingProduct == null)
            return false;

        bool IsDeleted = await _productRepository.DeleteProduct(exittingProduct.ProductID);

        return IsDeleted;
    }

    public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        Product? product = await _productRepository.GetProductByCondition(conditionExpression);

        if (product != null)
        {
            return _mapper.Map<ProductResponse>(product);
        }
        else
            return null;
    }

    public async Task<List<ProductResponse?>> GetProducts()
    {
        var products = await _productRepository.GetProducts();

        IEnumerable<ProductResponse?> productResponse = _mapper.Map<IEnumerable<ProductResponse>>(products);
        return productResponse.ToList();

    }

    public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        var products = await _productRepository.GetProductsByCondition(conditionExpression);
        IEnumerable<ProductResponse?> productResponse = _mapper.Map<IEnumerable<ProductResponse>>(products);
        return productResponse.ToList();
    }

    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        // Check exittingProduct object in db using productId
        var exittingProduct = _productRepository.GetProductByCondition(temp => temp.ProductID == productUpdateRequest.ProductID);

        if (exittingProduct == null)
        {
            throw new ArgumentException("Invalid product id");
        }

        // Fluent validation 
            ValidationResult validationResult = await _productUpdateRequestvalidator.ValidateAsync(productUpdateRequest);

        // Check IsValid or not
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage));
            throw new ArgumentException(errors);
        }

        // Mapped UpdateReq model into product 
        var product = _mapper.Map<Product>(productUpdateRequest);

        var updateProduct = await _productRepository.UpdateProduct(product);

        if (updateProduct != null)
        {
            return _mapper.Map<ProductResponse>(updateProduct);
        }
        else
            return null;
    }
}
