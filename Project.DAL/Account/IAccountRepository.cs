using Project.Models;

namespace Project.DAL.Account
{
    public interface IAccountRepository
    {
        Task<LoginModel> Signup(Register model);
        Task<LoginModel> Login(Login login);
    }
}
