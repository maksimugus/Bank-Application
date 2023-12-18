using Abstractions.Repositories;
using Contracts.Accounts;
using Contracts.Results;
using Models.Accounts;
using Models.Results;

#pragma warning disable CA2007

namespace Application.Accounts;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IOperationRepository _operationRepository;

    public AccountService(IAccountRepository accountRepository, IOperationRepository operationRepository)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(operationRepository);
        _accountRepository = accountRepository;
        _operationRepository = operationRepository;
    }

    public async Task<Result<IEnumerable<Account>>> GetUserAccounts(long userId)
    {
        IEnumerable<Account> accounts = await _accountRepository.GetUserAccounts(userId);
        return new Result<IEnumerable<Account>>(ResultType.Success, accounts);
    }

    public async Task<Result<string>> CreateAccount(long userId, int pin)
    {
        await _accountRepository.AddAccount(userId, pin);
        return new Result<string>(ResultType.Success, "Account created");
    }

    public async Task<Result<Account>> FindAccountById(long accountId)
    {
        Account? account = await _accountRepository.FindAccountById(accountId);
        return account is null
            ? new Result<Account>(ResultType.Failure, new NullAccount())
            : new Result<Account>(ResultType.Success, account);
    }

    public async Task<Result<string>> WithdrawMoneyFromAccount(long accountId, int pin, int amount)
    {
        Account? account = await _accountRepository.FindAccountById(accountId);

        if (account is null)
        {
            return new Result<string>(ResultType.Failure, "Account not found");
        }

        if (pin != account.Pin)
        {
            return new Result<string>(ResultType.Failure, "Wrong pin");
        }

        if (account.Balance < amount)
        {
            return new Result<string>(ResultType.Failure, "Not enough money");
        }

        Account updatedAccount = account with { Balance = account.Balance - amount };
        await _accountRepository.UpdateAccount(updatedAccount);
        await _operationRepository.AddOperation(accountId, -amount);

        return new Result<string>(ResultType.Success, string.Empty);
    }

    public async Task<Result<string>> DepositMoneyToAccount(long accountId, int amount)
    {
        Account? account = await _accountRepository.FindAccountById(accountId);

        if (account is null)
        {
            return new Result<string>(ResultType.Failure, "Account not found");
        }

        Account updatedAccount = account with { Balance = account.Balance + amount };
        await _accountRepository.UpdateAccount(updatedAccount);
        await _operationRepository.AddOperation(accountId, amount);

        return new Result<string>(ResultType.Success, string.Empty);
    }
}