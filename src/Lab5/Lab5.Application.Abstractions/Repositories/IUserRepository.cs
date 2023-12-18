using Models.Users;

namespace Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> FindUserByLogin(string login);
    Task AddUser(string login, string password, string role);
    Task UpdateUser(User user);
    Task DeleteUserById(long id);
}