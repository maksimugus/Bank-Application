using Npgsql;

namespace Abstractions.Connection;

public interface IPostgresConnectionProvider
{
    NpgsqlDataSource DataSource { get; }
}