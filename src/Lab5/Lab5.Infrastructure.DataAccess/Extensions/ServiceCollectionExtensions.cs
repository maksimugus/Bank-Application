using Abstractions.Connection;
using Abstractions.Repositories;
using DataAccess.Connection;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection collection,
        string connectionString)
    {
        collection.AddSingleton<IPostgresConnectionProvider>(new PostgresConnectionProvider(connectionString));
        collection.AddSingleton<IUserRepository, UserRepository>();
        collection.AddSingleton<IAccountRepository, AccountRepository>();
        collection.AddSingleton<IOperationRepository, OperationRepository>();

        return collection;
    }
}