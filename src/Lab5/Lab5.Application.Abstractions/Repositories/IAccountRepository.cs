using Models.Accounts;

namespace Abstractions.Repositories;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetUserAccounts(long userId);
    Task<Account?> FindAccountById(long id);
    Task AddAccount(long userId, int pin);
    Task UpdateAccount(Account account);
    Task DeleteAccountById(long id);
}