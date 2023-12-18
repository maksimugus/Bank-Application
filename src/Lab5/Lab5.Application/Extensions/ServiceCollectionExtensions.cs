using Application.Accounts;
using Application.Operations;
using Application.Users;
using Contracts.Accounts;
using Contracts.Operations;
using Contracts.Users;
using Microsoft.Extensions.DependencyInjection;
namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddSingleton<IUserService, UserService>();
        collection.AddSingleton<IAccountService, AccountService>();
        collection.AddSingleton<IOperationService, OperationService>();
        return collection;
    }
}