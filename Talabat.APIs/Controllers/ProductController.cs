using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Specifications;

namespace Talabat.APIs.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IGenericRepository<Product> ProductRepository;
        private readonly IGenericRepository<ProductType> ProductTypeRepository;
        private readonly IGenericRepository<ProductBrand> ProductBrandRepository;
        private readonly IMapper mapper;

        public ProductController(IGenericRepository<Product> ProductRepository, IGenericRepository<ProductType> ProductTypeRepository, IGenericRepository<ProductBrand> ProductBrandRepository, IMapper mapper)
        {
            this.ProductRepository = ProductRepository;
            this.ProductTypeRepository = ProductTypeRepository;
            this.ProductBrandRepository = ProductBrandRepository;
            this.mapper = mapper;
        }


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
        //    => Ok(await ProductRepository.GetAllAsync());

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Product>> GetProductAsync(int id)
        //    => Ok(await ProductRepository.GetById(id));

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProductsAsync([FromQuery] ProductParamsSpecification Params)
        {
            if (Params.PageIndex < 1 || Params.PageSize < 1)
                return BadRequest("PageIndex and PageSize must be greater than 0.");

            var specification = new ProductWithTypeAndBrandSpecification(Params);
            var products = await ProductRepository.GetAllAsyncWithSpec(specification);
            var MappedProducts = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDTO>>(products);
            return Ok(MappedProducts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductAsync(int id)
        {
            var specification = new ProductWithTypeAndBrandSpecification(id);
            var product = await ProductRepository.GetByIdWithSpec(specification);
            if (product == null)
                return NotFound(new ApiResponse(404, "Product doesn't exist."));
            var MappedProduct = mapper.Map<Product, ProductDTO>(product);
            return Ok(MappedProduct);
        }

        [HttpGet("ProductTypes")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypesAsync()
        {
            var ProductTypes = await ProductTypeRepository.GetAllAsync();
            return Ok(ProductTypes);
        }

        [HttpGet("ProductBrands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrandsAsync()
        {
            var ProductBrands = await ProductBrandRepository.GetAllAsync();
            return Ok(ProductBrands);
        }
    }
}
