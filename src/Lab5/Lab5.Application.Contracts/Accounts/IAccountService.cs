using Models.Accounts;
using Models.Results;

namespace Contracts.Accounts;

public interface IAccountService
{
    Task<Result<IEnumerable<Account>>> GetUserAccounts(long userId);
    Task<Result<string>> CreateAccount(long userId, int pin);
    Task<Result<Account>> FindAccountById(long accountId);
    Task<Result<string>> WithdrawMoneyFromAccount(long accountId, int pin, int amount);
    Task<Result<string>> DepositMoneyToAccount(long accountId, int amount);
}