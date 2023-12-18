using Models.Results;
using Models.Users;

namespace Contracts.Users;

public interface IUserService
{
    Task<Result<User>> Login(string login, string password);
    Task<Result<string>> Register(string login, string password, string role = "customer");
}