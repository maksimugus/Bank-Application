using Abstractions.Repositories;
using Application.Accounts;
using Contracts.Results;
using Models.Accounts;
using Models.Results;
using NSubstitute;
using Xunit;

#pragma warning disable CA2007

namespace Itmo.ObjectOrientedProgramming.Lab5.Tests;

public class BankApplicationTests
{
    private readonly IAccountRepository _accountRepository;
    private readonly AccountService _accountService;

    public BankApplicationTests()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        IOperationRepository operationRepository = Substitute.For<IOperationRepository>();
        _accountService = new AccountService(_accountRepository, operationRepository);
    }

    [Fact]
    public async void SuccessWithdrawingTest()
    {
        var account = new Account(1, 1, 100, 1);
        _accountRepository.FindAccountById(1).Returns(account);
        Result<string> result = await _accountService.WithdrawMoneyFromAccount(1, 1, 100);
        await _accountRepository.Received(1).UpdateAccount(account with { Balance = 0 });
        Assert.Equal(ResultType.Success, result.Type);
    }

    [Theory]
    [InlineData(100, 1)]
    [InlineData(1000, 2)]
    public async void FailureWithdrawingTest(int balance, int pin)
    {
        var account = new Account(1, 1, balance, pin);
        _accountRepository.FindAccountById(1).Returns(account);
        Result<string> result = await _accountService.WithdrawMoneyFromAccount(1, 1, 1000);
        await _accountRepository.DidNotReceiveWithAnyArgs().UpdateAccount(Arg.Any<Account>());
        Assert.Equal(ResultType.Failure, result.Type);
    }

    [Fact]
    public async void SuccessDepositingTest()
    {
        var account = new Account(1, 1, 0, 1);
        _accountRepository.FindAccountById(1).Returns(account);
        Result<string> result = await _accountService.DepositMoneyToAccount(1, 100);
        await _accountRepository.Received(1).UpdateAccount(account with { Balance = 100 });
        Assert.Equal(ResultType.Success, result.Type);
    }
}