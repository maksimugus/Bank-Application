using Abstractions.Connection;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Extensions;

public static class ServiceScopeExtensions
{
    public static void SetUpDataBase(this IServiceScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);
        const string query = """
                             CREATE TABLE IF NOT EXISTS users
                             (
                                 id bigint PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                                 login text NOT NULL,
                                 password text NOT NULL,
                                 role text NOT NULL
                             );

                             CREATE TABLE IF NOT EXISTS accounts
                             (
                                 id bigint PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                                 user_id bigint NOT NULL REFERENCES users (id),
                                 balance integer NOT NULL,
                                 pin integer NOT NULL
                             );

                             CREATE TABLE IF NOT EXISTS operations
                             (
                                 id bigint PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
                                 account_id bigint NOT NULL REFERENCES accounts (id),
                                 amount integer NOT NULL
                             );
                             
                             INSERT INTO users (login, password, role) VALUES ('admin', 'admin', 'admin');
                             """;
        IPostgresConnectionProvider connectionProvider =
            scope.ServiceProvider.GetRequiredService<IPostgresConnectionProvider>();
        using NpgsqlCommand command = connectionProvider.DataSource.CreateCommand(query);
        command.ExecuteNonQuery();
    }

    public static void ResetDataBase(this IServiceScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);
        const string query = "DROP TABLE IF EXISTS operations, users, accounts";
        IPostgresConnectionProvider connectionProvider =
            scope.ServiceProvider.GetRequiredService<IPostgresConnectionProvider>();
        using NpgsqlCommand command = connectionProvider.DataSource.CreateCommand(query);
        command.ExecuteNonQuery();
    }
}