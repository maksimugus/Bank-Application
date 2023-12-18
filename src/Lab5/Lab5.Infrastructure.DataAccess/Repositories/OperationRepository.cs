using Abstractions.Connection;
using Abstractions.Repositories;
using Models.Operations;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Repositories;

public class OperationRepository : IOperationRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public OperationRepository(IPostgresConnectionProvider connectionProvider)
    {
        ArgumentNullException.ThrowIfNull(connectionProvider);
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<Operation>> GetAccountOperations(long accountId)
    {
        const string query = "SELECT * FROM operations WHERE account_id = ($1)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(accountId);
        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        List<Operation> operations = new();
        while (await reader.ReadAsync())
        {
            operations.Add(new Operation(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2)));
        }

        return operations;
    }

    public async Task AddOperation(long accountId, int balanceChange)
    {
        const string query = "INSERT INTO operations (account_id, amount) VALUES (($1), ($2))";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(accountId);
        cmd.Parameters.AddWithValue(balanceChange);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateOperation(Operation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        const string query = "UPDATE operations SET account_id = ($1), balance_change = ($2) WHERE id = ($3)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(operation.AccountId);
        cmd.Parameters.AddWithValue(operation.BalanceChange);
        cmd.Parameters.AddWithValue(operation.Id);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteOperationById(long id)
    {
        const string query = "DELETE FROM operations WHERE id = ($1)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(id);

        await cmd.ExecuteNonQueryAsync();
    }

    private NpgsqlCommand CreateCommand(string query) =>
        _connectionProvider.DataSource.CreateCommand(query);
}