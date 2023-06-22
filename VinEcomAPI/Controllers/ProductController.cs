﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using VinEcomDomain.Model;
using VinEcomDomain.Resources;
using VinEcomInterface.IService;
using VinEcomViewModel.Product;

namespace VinEcomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpPost("StoreProducts")]
        public async Task<IActionResult> GetStoreProductPageAsync([FromBody] StoreProductFilterViewModel vm)
        {
            var validateResult = await productService.ValidateStoreProductFilterAsync(vm);
            if (!validateResult.IsValid)
            {
                var errors = validateResult.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage });
                return BadRequest(errors);
            }
            var result = await productService.GetStoreProductPageAsync(vm);
            return Ok(result);
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProductPage(int pageIndex = 0, int pageSize = 10)
        {
            if (pageIndex < 0) return BadRequest(new { Message = VinEcom.VINECOM_PAGE_INDEX_ERROR });
            if (pageSize <= 0) return BadRequest(new { Message = VinEcom.VINECOM_PAGE_SIZE_ERROR });
            var products = await productService.GetProductPagingAsync(pageIndex, pageSize);
            var ratings = await productService.GetProductRatingAsync(products.Items.Select(x => x.Id).ToList());
            return Ok(new
            {
                ProductPaging = products,
                ProductRatings = ratings
            });
        }

        [HttpPost("Filter")]
        public async Task<IActionResult> GetProductFilterAsync(ProductFilterModel filter, int pageIndex = 0, int pageSize = 10)
        {
            if (pageIndex < 0) return BadRequest();
            if (pageSize <= 0) return BadRequest();
            var products = await productService.GetProductFilterAsync(pageIndex, pageSize, filter);
            return Ok(products);
        }
        [HttpPost("AddAsync")]
        public async Task<IActionResult> AddProductAsync([FromBody] ProductCreateModel product)
        {
            var validateResult = await productService.ValidateCreateProductAsync(product);
            if (!validateResult.IsValid) return BadRequest(validateResult.Errors);
            //
            if (await productService.AddAsync(product)) return Ok(product);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = VinEcom.VINECOM_PRODUCT_CREATE_ERROR });
        }
    }
}
