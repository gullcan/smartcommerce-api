# SmartCommerce API

Clean Architecture yaklaşımıyla geliştirilmiş, JWT Authentication + Role-based Authorization (AdminOnly) içeren örnek bir e-ticaret API projesi.

## Proje Yapısı

- `src/SmartCommerce.Api`
  Minimal API endpoints, middleware, Swagger, DI configuration, startup.
- `src/SmartCommerce.Application`
  Use-case/service katmanı, DTOs, validation, exceptions, abstractions.
- `src/SmartCommerce.Infrastructure`
  EF Core, repository implementasyonları, migrations, security (password hasher), seeding.
- `src/SmartCommerce.Domain`
  Entities ve domain modelleri.

## Gereksinimler

- .NET SDK 9.x
- (Opsiyonel) `dotnet-ef` (migration işlemleri için)

## Çalıştırma

```bash
dotnet watch run --project src/SmartCommerce.Api
```

Swagger:
- http://localhost:5276/swagger

Health check:
- GET /health

## Authentication

### Seeded Admin (Development)
Uygulama ilk çalıştığında seeding ile admin kullanıcı eklenir (DB boşsa):

- Email: admin@smartcommerce.local
- Password: Admin123!
- Role: Admin

> Bu değerler `DbSeeder.cs` içinde bulunur.

### Swagger Authorization
1. `POST /auth/login` ile token al
2. Swagger’da **Authorize** butonuna tıkla
3. Açılan "Value" alanına **tokeni yapıştır**

> Not: Bazı Swagger kurulumlarında `Bearer ` prefix’ini elle yazman gerekir (yani `Bearer <token>`). Sende direkt token yapıştırmak yeterli.

## Authorization

- `AdminOnly` policy (örnek):
  - POST /categories
  - PUT /categories/{id}
  - DELETE /categories/{id}
  - POST /products (projedeki mevcut kuralına göre)

Normal kullanıcı bu endpointlerde **403 Forbidden** alır.

## Migrations (EF Core)

Global tool kurulumu:
```bash
dotnet tool install -g dotnet-ef
```

Migration ekleme (örnek):
```bash
dotnet ef migrations add InitialCreate \
  -p src/SmartCommerce.Infrastructure \
  -s src/SmartCommerce.Api \
  -c AppDbContext \
  -o Migrations
```

DB update:
```bash
dotnet ef database update \
  -p src/SmartCommerce.Infrastructure \
  -s src/SmartCommerce.Api \
  -c AppDbContext
```

## Notlar

- Local SQLite dosyaları repoya commit edilmez (`.gitignore`):
  - *.db, *.db-wal, *.db-shm, *.db.bak vb.

