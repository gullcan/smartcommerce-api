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

## Endpoint Listesi (Özet)

- Auth: POST /auth/register, POST /auth/login
- Categories: GET /categories, GET /categories/{id}, POST/PUT/DELETE (AdminOnly)
- Products: GET /products, GET /products/{id}, POST/PUT/DELETE (AdminOnly)
- Orders: (varsa) GET /orders, GET /orders/{id}, POST /orders, PATCH /orders/{id}/status
- Reviews: (ödev kuralı) /products/{id}/reviews

## API Response Örnekleri

### 400 Validation failed
```json
{"success":false,"message":"Validation failed.","data":null,"errors":{"Name":["Name is required."]}}
```

### 409 Conflict
```json
{"success":false,"message":"... already exists.","data":null,"errors":null}
```


## Mimari (Kısa Diagram)

```text
Client (Swagger/Postman)
        |
        v
SmartCommerce.Api (Minimal API Endpoints)
        |
        v
SmartCommerce.Application (Services/DTOs/Validation)
        |
        v
SmartCommerce.Infrastructure (EF Core/Repositories/Migrations/Seeding)
        |
        v
SQLite

eof



ODODOCEOF
EOF

## Mimari (Kısa Diagram)

```text
Client (Swagger/Postman)
        |
        v
SmartCommerce.Api (Minimal API Endpoints)
        |
        v
SmartCommerce.Application (Services/DTOs/Validation)
        |
        v
SmartCommerce.Infrastructure (EF Core/Repositories/Migrations/Seeding)
        |
        v
SQLite

## Mimari Diagram

```mermaid
flowchart LR
  A[SmartCommerce.Api<br/>Minimal Endpoints] --> B[SmartCommerce.Application<br/>Services + DTOs + Validation]
  B --> C[SmartCommerce.Infrastructure<br/>EF Core + Repositories + Migrations + Seeding]
  C --> D[(SQLite DB)]
  B --> E[SmartCommerce.Domain<br/>Entities + Enums]

Validation hatalarında ayrıca errors alanı da dönebilir.
Endpoint Listesi
Auth
* POST /auth/register
* POST /auth/login
Categories
* GET /categories
* GET /categories/{id}
* POST /categories (AdminOnly)
* PUT /categories/{id} (AdminOnly)
* DELETE /categories/{id} (AdminOnly)
Products
* GET /products
* GET /products/{id}
* POST /products (AdminOnly)
* PUT /products/{id} (AdminOnly)
* DELETE /products/{id} (AdminOnly)
Orders
* GET /orders
* GET /orders/{id}
* GET /orders/mine
* POST /orders
* PATCH /orders/{id}/status

{
  "success": true,
  "message": "Bilgi veya hata mesajı",
  "data": { }
}

Validation hatalarında ayrıca errors alanı da dönebilir.

Endpoint Listesi
Auth
* POST /auth/register
* POST /auth/login
Categories
* GET /categories
* GET /categories/{id}
* POST /categories (AdminOnly)
* PUT /categories/{id} (AdminOnly)
* DELETE /categories/{id} (AdminOnly)
Products
* GET /products
* GET /products/{id}
* POST /products (AdminOnly)
* PUT /products/{id} (AdminOnly)
* DELETE /products/{id} (AdminOnly)
Orders
* GET /orders
* GET /orders/{id}
* GET /orders/mine
* POST /orders
* PATCH /orders/{id}/status
Reviews (RESTful kuralına uygun)
* GET /products/{productId}/reviews
* POST /products/{productId}/reviews
* PUT /products/{productId}/reviews/{id}
* DELETE /products/{productId}/reviews/{id}
* 
Status Code Örnekleri

200 OK

{"success":true,"message":"OK","data":{}}

201 Created

{"success":true,"message":"Created","data":{}}

400 Bad Request (Validation)

{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": {
    "Name": ["Name is required."]
  }
}

401 Unauthorized

{"success":false,"message":"Unauthorized","data":null}

404 Not Found

{"success":false,"message":"Not found.","data":null}

409 Conflict

{"success":false,"message":"... already exists.","data":null}

500 Internal Server Error

{"success":false,"message":"Unexpected error.","data":null}

Logging
* .NET built-in logging kullanılır.
* Docker ile logları izlemek için:

docker compose logs -f api

Docker ile Çalıştırma (Opsiyonel)

docker compose up --build
Swagger: http://localhost:5276/swagger Health: GET /health
