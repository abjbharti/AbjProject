using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.DAL.Account;
using Project.DAL.CacheMemory;
using Project.Models;

namespace AbjProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository, ICacheService cacheService) 
        {
            _accountRepository = accountRepository;
        }

        //Create any new account
        [HttpPost("signup")]
        public IActionResult SignUp(Register Model)
        {
            if(ModelState.IsValid)
            {
                var result = _accountRepository.Signup(Model);

                if(result.Result != null)
                {
                    if(result.Result.IsSuccess)
                    {
                        return Ok(new { Message = "Registration successful" });
                    }
                    return BadRequest(new { Message = "Registration failed" });
                }
            }
            return BadRequest(new { Message = "Invalid model state" });
        }

        //Login your account
        [HttpPost("login")]
        public IActionResult Login(Login login)
        {
            var result = _accountRepository.Login(login);

            if (result != null)
            {
                if (result.Result.IsSuccess)
                {
                    return Ok(new
                    {
                        result.Result.Token,
                        result.Result.Username,
                        result.Result.Role
                    });
                }

                return Unauthorized(new { Message = "Authentication failed" });
            }
            return Unauthorized(new { Message = "Invalid login credentials" });
        }
    }
}
