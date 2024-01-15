using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.DAL.CacheMemory;
using Project.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Account
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public readonly ICacheService _cacheService;

        public AccountRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ICacheService cacheService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        public async Task<LoginModel> Login(Login login)
        {
            var user = await _userManager.FindByNameAsync(login.Email);

            if (user == null)
            {
                return new LoginModel()
                {
                    IsSuccess = false
                };
            }
            var expirationTime = DateTimeOffset.Now.AddHours(1.0);
            _cacheService.SetData<string>("UserEmail", login.Email, expirationTime);
            _cacheService.SetData<string>("UserId", user.Id, expirationTime);

            var PasswordValid = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!PasswordValid)
            {
                return new LoginModel()
                {
                    IsSuccess = false
                };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var tokenHendeler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._configuration.GetValue<string>("JWT:Key"));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = System.DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHendeler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHendeler.WriteToken(token);

            return new LoginModel()
            {
                IsSuccess = true,
                Token = encryptedToken,
                Username = user.UserName,
                Role = userRoles[0]
            };
        }

        public async Task<LoginModel> Signup(Register model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            var response = new LoginModel();

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                response.IsSuccess = true;
                return response;
            }

            response.IsSuccess = false;
            return response;
        }
    }
}
