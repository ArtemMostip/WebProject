using WebProject.Datas;
using WebProject.Controllers;
using WebProject.Services;
using WebProject.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebProject.Config;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using WebProject.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("SmtpSetting"));
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Додаємо сервісні методи
builder.Services.AddCustomServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
builder.Services.AddCustomSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles(); 
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
