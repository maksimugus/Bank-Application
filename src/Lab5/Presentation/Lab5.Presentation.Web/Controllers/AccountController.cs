using System.Globalization;
using Contracts.Accounts;
using Contracts.Operations;
using Contracts.Results;
using Lab5.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Accounts;
using Models.Operations;
using Models.Results;

#pragma warning disable CA2007

namespace Console.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IOperationService _operationService;

    public AccountController(IAccountService accountService, IOperationService operationService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        ArgumentNullException.ThrowIfNull(operationService);
        _accountService = accountService;
        _operationService = operationService;
    }

    [HttpPost("create-account")]
    public async Task<ActionResult> CreateAccount(int pin)
    {
        await _accountService.CreateAccount(GetUserId(), pin);
        return Ok();
    }

    [HttpGet("user-accounts")]
    public async Task<ActionResult<IEnumerable<long>>> GetUserAccounts()
    {
        return Ok((await _accountService.GetUserAccounts(GetUserId()))
            .Data.Select(account => account.Id));
    }

    [HttpGet("account-balance")]
    public async Task<ActionResult<string>> ShowAccountBalance(long accountId)
    {
        Result<Account> result = await _accountService.FindAccountById(accountId);
        if (result.Type is ResultType.Failure) return BadRequest();
        if (GetUserId() != result.Data.UserId) return Forbid();
        return Ok(result.Data.Balance);
    }

    [HttpPatch("withdraw-money")]
    public async Task<ActionResult<string>> WithdrawMoneyFromAccount(int accountId, int pin, int amount)
    {
        Result<Account> result = await _accountService.FindAccountById(accountId);
        if (result.Type is ResultType.Failure) return BadRequest();
        if (GetUserId() != result.Data.UserId) return Forbid();
        return Ok((await _accountService.WithdrawMoneyFromAccount(accountId, pin, amount)).Data);
    }

    [HttpPatch("deposit-money")]
    public async Task<ActionResult<string>> DepositMoneyToAccount(int accountId, int amount)
    {
        Result<Account> result = await _accountService.FindAccountById(accountId);
        if (result.Type is ResultType.Failure) return BadRequest();
        if (GetUserId() != result.Data.UserId) return Forbid();
        return Ok((await _accountService.DepositMoneyToAccount(accountId, amount)).Data);
    }

    [HttpGet("account-operations")]
    public async Task<ActionResult<Operation>> GetAccountOperations(long accountId)
    {
        Result<IEnumerable<Operation>> result = await _operationService.GetAccountOperations(accountId);
        if (result.Type is ResultType.Failure) return BadRequest();
        return Ok(result.Data);
    }

    private long GetUserId()
    {
        string id = HttpContext.User.FindFirst("id")?.Value ??
                    throw new NotAuthorizedException();
        return long.Parse(id, CultureInfo.CurrentCulture);
    }
}