using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartCommerce.Application.Abstractions.Security;
using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher, CancellationToken ct = default)
    {
        // Dev için pratik: migration varsa otomatik uygula
        await db.Database.MigrateAsync(ct);

        // Admin user seed (yoksa ekle)
        var adminEmail = "admin@smartcommerce.local";
        var adminExists = await db.Users.AnyAsync(u => !u.IsDeleted && u.Email.ToLower() == adminEmail, ct);

        if (!adminExists)
        {
            var now = DateTime.UtcNow;

            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = adminEmail,
                PasswordHash = hasher.Hash("Admin123!"),
                Role = "Admin",
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            });

            await db.SaveChangesAsync(ct);
        }

        // Category + Product seed (DB boşsa)
        if (!await db.Categories.AnyAsync(c => !c.IsDeleted, ct))
        {
            var now = DateTime.UtcNow;

            var electronics = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                Description = "Devices and gadgets",
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            var books = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Books",
                Description = "Printed and digital books",
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            var home = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Home",
                Description = "Home & living products",
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            db.Categories.AddRange(electronics, books, home);
            await db.SaveChangesAsync(ct);

            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Wireless Mouse",
                    Description = "2.4G ergonomic mouse",
                    Price = 499.90m,
                    Stock = 50,
                    CategoryId = electronics.Id,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Clean Architecture",
                    Description = "Software architecture book",
                    Price = 699.00m,
                    Stock = 30,
                    CategoryId = books.Id,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Coffee Mug",
                    Description = "Ceramic 350ml",
                    Price = 199.00m,
                    Stock = 100,
                    CategoryId = home.Id,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                }
            );

            await db.SaveChangesAsync(ct);
        }
    }
}
