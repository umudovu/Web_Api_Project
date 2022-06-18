using System.Text;
using API.Data;
using API.Extentions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;
// Add services to the container.

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddApplicationServices(config);

builder.Services.AddControllers();

builder.Services.AddIdentityServices(config);
builder.Services.AddEndpointsApiExplorer();
    
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
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
