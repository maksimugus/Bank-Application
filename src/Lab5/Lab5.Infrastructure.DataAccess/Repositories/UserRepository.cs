using Abstractions.Connection;
using Abstractions.Repositories;
using Models.Users;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IPostgresConnectionProvider _connectionProvider;

    public UserRepository(IPostgresConnectionProvider connectionProvider)
    {
        ArgumentNullException.ThrowIfNull(connectionProvider);
        _connectionProvider = connectionProvider;
    }

    public async Task<User?> FindUserByLogin(string login)
    {
        const string query = "SELECT * FROM users WHERE login = ($1)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(login);

        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User(
            Id: reader.GetInt64(0),
            Login: reader.GetString(1),
            Password: reader.GetString(2),
            Role: reader.GetString(3));
    }

    public async Task AddUser(string login, string password, string role)
    {
        const string query = "INSERT INTO users (login, password, role) VALUES (($1), ($2), ($3))";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(login);
        cmd.Parameters.AddWithValue(password);
        cmd.Parameters.AddWithValue(role);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        const string query = "UPDATE users SET login = ($1), password = ($2) WHERE id = ($3)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(user.Login);
        cmd.Parameters.AddWithValue(user.Password);
        cmd.Parameters.AddWithValue(user.Id);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteUserById(long id)
    {
        const string query = "DELETE FROM users WHERE id = ($1)";

        await using NpgsqlCommand cmd = CreateCommand(query);
        cmd.Parameters.AddWithValue(id);

        await cmd.ExecuteNonQueryAsync();
    }

    private NpgsqlCommand CreateCommand(string query) =>
        _connectionProvider.DataSource.CreateCommand(query);
}