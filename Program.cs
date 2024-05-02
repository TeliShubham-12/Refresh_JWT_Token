using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Helpers;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddCors();
services.AddControllers();
services
    .AddOptions<AppSettings>()
    .BindConfiguration("AppSettings")
    .ValidateDataAnnotations()
    .ValidateOnStart();

services.AddDbContext<DataContext>(
    options =>
        options
            .UseMySQL(
                builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new ArgumentNullException("DefaultConnection")
            )
            .LogTo(Console.WriteLine, LogLevel.Information)
);

services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "http://localhost:5235",
            ValidAudience = "http://localhost:5235",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["AppSettings:Secret"]
                        ?? throw new ArgumentNullException("Secret")
                )
            )
        };
    });

services.AddScoped<IJwtUtils, JwtUtils>();
services.AddScoped<IUserService, UserService>();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(
    x => x.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials()
);
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
