using AppStore.Api.Response;
using AppStore.Core.DTOs;
using AppStore.Core.Entities;
using AppStore.Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStore.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productProduct;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _mapper = mapper;
            _productProduct = productService;
        }

        [HttpGet]
        public IActionResult List()
        {
            var products =  _productProduct.List();
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            var response = new ApiResponse<IEnumerable<ProductDto>>(productsDto);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productProduct.Get(id);
            var productDto = _mapper.Map<ProductDto>(product);
            var response = new ApiResponse<ProductDto>(productDto);
            
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productProduct.Create(product);
            productDto = _mapper.Map<ProductDto>(product);
            var response = new ApiResponse<ProductDto>(productDto);
            
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            product.Id = id;
            var resutl = await _productProduct.Edit(product);
            var response = new ApiResponse<bool>(resutl);
           
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resutl = await _productProduct.Remove(id);
            var response = new ApiResponse<bool>(resutl);
            return Ok(response);
        }
    }
}
