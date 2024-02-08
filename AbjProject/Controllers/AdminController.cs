using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DAL.CacheMemory;
using Project.DAL.Context;
using Project.DAL.Generic;
using Project.Models;
using System.Linq.Expressions;

namespace AbjProject.Controllers
{
    /// <summary>
    /// Will be only accessed by the admin role
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        public IGenericRepository<Product> _genericRepository;
        private readonly ICacheService _cacheService;

        public AdminController(IGenericRepository<Product> genericRepository, ICacheService cacheService)
        {
            _genericRepository = genericRepository;
            _cacheService = cacheService;
        }

        //Get all the porducts
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _genericRepository.GetAll();

            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        //Get any specific product details
        [HttpGet("{id:guid}", Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var result = await _genericRepository.GetById(id);

            if (result != null)
            {
                return new JsonResult((Product)result);
            }
            return NotFound();
        }

        //Add any products
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(Product model)
        {
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Rating = model.Rating,
                Category = model.Category,
                Quantity = model.Quantity
            };

            await _genericRepository.Insert(product);
            return Ok();
        }

        //Edit the product
        [HttpPut("{id:guid}", Name = "EditProduct")]
        public async Task<IActionResult> EditProduct(Guid id, Product model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var product = await _genericRepository.GetById(id);

            if (product != null)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                await _genericRepository.Update(product);
                return Ok(product);
            }

            return NotFound();
        }

        //Delete any product
        [HttpDelete("{id:guid}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _genericRepository.GetById(id);

            if (product != null)
            {
                await _genericRepository.Delete(id);
                return Ok(product);
            }

            return NotFound();
        }
    }
}
