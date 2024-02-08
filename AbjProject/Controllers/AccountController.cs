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
        public async Task<IActionResult> SignUp(Register Model)
        {
            if(ModelState.IsValid)
            {
                var result = await _accountRepository.Signup(Model);

                if(result != null)
                {
                    if(result.IsSuccess)
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
        public async Task<IActionResult> Login(Login login)
        {
            var result = await _accountRepository.Login(login);

            if (result != null)
            {
                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        result.Token,
                        result.Username,
                        result.Role
                    });
                }

                return Unauthorized(new { Message = "Authentication failed" });
            }
            return Unauthorized(new { Message = "Invalid login credentials" });
        }
    }
}
