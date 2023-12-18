using Abstractions.Connection;
using Npgsql;

namespace DataAccess.Connection;

public class PostgresConnectionProvider : IPostgresConnectionProvider
{
    public PostgresConnectionProvider(string connectionString)
    {
        DataSource = NpgsqlDataSource.Create(connectionString);
    }

    public NpgsqlDataSource DataSource { get; }
}