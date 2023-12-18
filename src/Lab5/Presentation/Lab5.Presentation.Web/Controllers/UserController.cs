using System.Security.Claims;
using Console.Models;
using Contracts.Results;
using Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Results;
using Models.Users;

#pragma warning disable CA2007

namespace Console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(userService);
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        ArgumentNullException.ThrowIfNull(loginModel);
        Result<User> result = await _userService.Login(loginModel.Login, loginModel.Password);
        if (result.Type is ResultType.Failure) return BadRequest();

        var claims = new List<Claim>
        {
            new("id", $"{result.Data.Id}", ClaimValueTypes.Integer64),
            new(ClaimTypes.Role, result.Data.Role),
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
            });

        return RedirectToAction();
    }

    [Authorize(Roles = "admin")]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] LoginModel loginModel)
    {
        ArgumentNullException.ThrowIfNull(loginModel);
        Result<string> result = await _userService.Register(loginModel.Login, loginModel.Password);
        if (result.Type is ResultType.Failure) return BadRequest(result.Data);
        return Ok();
    }
}