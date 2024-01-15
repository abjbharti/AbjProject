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
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        public IGenericRepository<Product> _genericRepository;
        public ProjectDbContext _dbContext;
        private readonly ICacheService _cacheService;

        public AdminController(IGenericRepository<Product> genericRepository, ProjectDbContext dbContext, ICacheService cacheService)
        {
            _genericRepository = genericRepository;
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

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

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(Product model)
        {
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price
            };

            await _genericRepository.Insert(product);
            return Ok();
        }

        [HttpPut("{id:guid}", Name = "EditProduct")]
        public async Task<IActionResult> EditProduct(Guid id, Product model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var product = await _dbContext.Products.FindAsync(id);

            if (product != null)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                await _dbContext.SaveChangesAsync();
                return Ok(product);
            }

            return NotFound();
        }

        [HttpDelete("{id:guid}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);

            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
                return Ok(product);
            }

            return NotFound();
        }
    }
}
