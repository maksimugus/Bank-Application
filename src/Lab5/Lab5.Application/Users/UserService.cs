using Abstractions.Repositories;
using Contracts.Results;
using Contracts.Users;
using Models.Results;
using Models.Users;

#pragma warning disable CA2007

namespace Application.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task<Result<User>> Login(string login, string password)
    {
        User? user = await _userRepository.FindUserByLogin(login);

        if (user is null)
        {
            return new Result<User>(ResultType.Failure, new NullUser());
        }

        return user.Password != password
            ? new Result<User>(ResultType.Failure, new NullUser())
            : new Result<User>(ResultType.Success, user);
    }

    public async Task<Result<string>> Register(string login, string password, string role)
    {
        User? user = await _userRepository.FindUserByLogin(login);

        if (user is not null)
        {
            return new Result<string>(ResultType.Failure, $"User with name {login} already exists");
        }

        await _userRepository.AddUser(login, password, role);
        return new Result<string>(ResultType.Success, "Sign up successful");
    }
}