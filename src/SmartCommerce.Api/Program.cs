using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartCommerce.Api.Endpoints;
using SmartCommerce.Api.Middlewares;
using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common;
using SmartCommerce.Application.Services;
using SmartCommerce.Infrastructure.Persistence;
using SmartCommerce.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// DbContext (SQLite)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// DI: Repositories
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();

// DI: Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health (ApiResponse format)
app.MapGet("/health", () =>
{
    var payload = new { status = "ok" };
    return Results.Ok(ApiResponse<object>.Ok(payload, "Service is running"));
});

// Endpoints
app.MapCategoryEndpoints();
app.MapProductEndpoints();

app.Run();
