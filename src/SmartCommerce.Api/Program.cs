using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartCommerce.Application.Common;
using SmartCommerce.Infrastructure.Persistence;

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

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Standart response formatıyla test endpoint
app.MapGet("/health", () =>
{
    var payload = new { status = "ok" };
    return Results.Ok(ApiResponse<object>.Ok(payload, "Service is running"));
});

app.Run();
