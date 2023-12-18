using Application.Extensions;
using DataAccess.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;

#pragma warning disable CA2007

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddDataAccess(builder.Configuration.GetConnectionString("connection") ?? string.Empty)
    .AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
scope.ResetDataBase();
scope.SetUpDataBase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();