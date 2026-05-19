using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using TodoApi.Data;
using TodoApi.Extensions;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization: case-insensitive property matching and enum-as-string conversion.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Register FluentValidation validators, Swagger, and API explorer.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MySQL database connection with retry-on-failure (up to 3 retries).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
    }));

var app = builder.Build();

// Middleware pipeline: request body logging first, then global error handling.
app.UseRequestBodyLogging();
app.UseGlobalExceptionHandler();

// Enable Swagger UI only in development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto-discover and register all endpoint modules under the /api prefix.
app.MapGroup("/api").RegisterAllEndpoints();

app.Run();
