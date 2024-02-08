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
    /// Will be accessed by the user role or simply say customer
    /// </summary>
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IGenericRepository<Product> _genericRepository = null!;
        public IGenericRepository<AddToCart> _genericRepositoryForAddToCart = null!;
        public ProjectDbContext _dbContext;
        private readonly ICacheService _cacheService;

        public UserController(IGenericRepository<Product> genericRepository, IGenericRepository<AddToCart> genericRepositoryForAddToCart, ProjectDbContext dbContext, ICacheService cacheService)
        {
            _genericRepository = genericRepository;
            _genericRepositoryForAddToCart = genericRepositoryForAddToCart;
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        private static Expression<Func<Product, object>> GetPropertyExpression(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(Product), "x");
            var property = Expression.Property(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<Product, object>>(convert, parameter);
            return lambda;
        }

        //Get all the product list applying sever side pagination and searching
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(int page = 1, int pageSize = 5, string? search = "", string? sort = "", string? column = "")
         {
            var query = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search) || x.Description.Contains(search) || x.Price.ToString().Contains(search) || x.Id.ToString().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize);

            if (string.IsNullOrEmpty(column))
            {
                column = "Name";
            }
            
            switch (sort)
            {
                case "asc":
                    query = query.OrderByDescending(GetPropertyExpression(column));
                    sort = "asc";
                    break;
                default:
                    query = query.OrderBy(GetPropertyExpression(column));
                    sort = "desc";
                    break;
            }

            var productPerPage = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                TotalCount = totalCount,
                TotalPage = totalPage,
                ProductPerPage = productPerPage
            };

            return Ok(response);
        }

        //Add to cart option in which user can create it's own cart
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCart addToCart)
        {
            var userEmail = _cacheService.GetData<string>("UserEmail");
            var userId = _cacheService.GetData<string>("UserId");
            var product = await _genericRepository.GetById(addToCart.ProductId);

            if (product != null)
            {
                var existingCartItem = await _genericRepositoryForAddToCart.FirstOrDefaultAsync(x => x.ProductId == product.Id);
                if (existingCartItem != null)
                {
                    return Ok();
                }
                else
                {
                    addToCart.ProductName = product.Name;
                    addToCart.ProductPrice = product.Price;
                    addToCart.ProductQuantity = product.Quantity;
                    addToCart.CustomerId = new Guid(userId);
                    addToCart.CustomerEmail = userEmail;
                    addToCart.IsCheckOut = false;
                    await _genericRepositoryForAddToCart.Insert(addToCart);
                    return Ok(product);
                }
            }
            else
            {
                return NotFound();
            }
        }

        //Checkout page, clicking this will delete the item from cart
        [HttpGet("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var allProductsInCart = await _genericRepositoryForAddToCart.GetAll();

            if (allProductsInCart.Any())
            {
                await _genericRepositoryForAddToCart.Delete(allProductsInCart);
                return Ok(allProductsInCart);
            }

            return NotFound();
        }
    }
}
