using Microsoft.AspNetCore.Mvc;
using WebProject.Config;
using WebProject.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using DotNetEnv;


Env.Load();
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

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("Fixed", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(5);
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.Configure<EmailSettings>(options =>
{
    options.Host = Environment.GetEnvironmentVariable("SMTP_HOST");
    options.Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
    options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
    options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_ENABLESSL") ?? "true");
    options.FromEmail = Environment.GetEnvironmentVariable("SMTP_FROMEMAIL");
    options.Sender = Environment.GetEnvironmentVariable("SMTP_SENDER");
});

builder.Services.Configure<JwtSettings>(options =>
{
    options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRETKEY");
    options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
    options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
    options.ExpiryInMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRYINMINUTES") ?? "60");
});

builder.Services.AddCustomServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
builder.Services.AddCustomSwagger();
builder.Services.AddMemoryCache();
builder.Services.AddLazyCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRateLimiter();
app.UseDefaultFiles(); 
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("Fixed"); ;


app.Run();
public partial class Program { }
