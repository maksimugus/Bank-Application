using Abstractions.Connection;
using Abstractions.Repositories;
using Models.Accounts;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public AccountRepository(IPostgresConnectionProvider connectionProvider)
    {
        ArgumentNullException.ThrowIfNull(connectionProvider);
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<Account>> GetUserAccounts(long userId)
    {
        const string query = "SELECT * FROM accounts WHERE user_id = ($1)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(userId);
        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        List<Account> accounts = new();
        while (await reader.ReadAsync())
        {
            accounts.Add(new Account(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2),
                reader.GetInt32(3)));
        }

        return accounts;
    }

    public async Task<Account?> FindAccountById(long id)
    {
        const string query = "SELECT * FROM accounts WHERE id = ($1)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(id);

        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Account(
            Id: reader.GetInt64(0),
            UserId: reader.GetInt64(1),
            Balance: reader.GetInt32(2),
            Pin: reader.GetInt32(3));
    }

    public async Task AddAccount(long userId, int pin)
    {
        const string query = "INSERT INTO accounts (user_id, balance, pin) VALUES (($1), 0, ($2))";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(userId);
        cmd.Parameters.AddWithValue(pin);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAccount(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        const string query = "UPDATE accounts SET (user_id, balance, pin) = (($1), ($2), ($3)) WHERE id = ($4)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(account.UserId);
        cmd.Parameters.AddWithValue(account.Balance);
        cmd.Parameters.AddWithValue(account.Pin);
        cmd.Parameters.AddWithValue(account.Id);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAccountById(long id)
    {
        const string query = "DELETE FROM accounts WHERE id = ($1)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(id);

        await cmd.ExecuteNonQueryAsync();
    }

    private NpgsqlCommand CreateCommand(string query) =>
        _connectionProvider.DataSource.CreateCommand(query);
}