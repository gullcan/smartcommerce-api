
## Mimari Diagram

```mermaid
flowchart LR
  A[SmartCommerce.Api<br/>Minimal Endpoints] --> B[SmartCommerce.Application<br/>Services + DTOs + Validation]
  B --> C[SmartCommerce.Infrastructure<br/>EF Core + Repositories + Migrations + Seeding]
  C --> D[(SQLite DB)]
  B --> E[SmartCommerce.Domain<br/>Entities + Enums]
````

## Entities & Relations

Projede (minimum 4’ten fazla) şu entity’ler bulunur:

* **User**
* **Category**
* **Product**
* **Order**
* **OrderItem**
* **Review**

İlişkiler (örnek):

* Category **1 - N** Product
* User **1 - N** Order
* Order **1 - N** OrderItem
* Product **1 - N** Review
* User **1 - N** Review

Notlar:

* Tüm entity’lerde `CreatedAt`, `UpdatedAt` vardır.
* Soft delete için `IsDeleted` kullanılır.

## API Response Formatı

Tüm endpoint’ler standart bir response döndürür:

```json
{
  "success": true,
  "message": "Bilgi veya hata mesajı",
  "data": { }
}
```

> Validation hatalarında ayrıca `errors` alanı da dönebilir.

## Endpoint Listesi

### Auth

* `POST /auth/register`
* `POST /auth/login`

### Categories

* `GET /categories`
* `GET /categories/{id}`
* `POST /categories` (**AdminOnly**)
* `PUT /categories/{id}` (**AdminOnly**)
* `DELETE /categories/{id}` (**AdminOnly**)

### Products

* `GET /products`
* `GET /products/{id}`
* `POST /products` (**AdminOnly**)
* `PUT /products/{id}` (**AdminOnly**)
* `DELETE /products/{id}` (**AdminOnly**)

### Orders

* `GET /orders` (**AdminOnly**)
* `GET /orders/{id}` (Owner/Admin)
* `GET /orders/mine` (User)
* `POST /orders` (User)
* `PATCH /orders/{id}/status` (**AdminOnly**)

### Reviews (RESTful kuralına uygun)

* `GET /products/{productId}/reviews`
* `POST /products/{productId}/reviews` (Auth)
* `PUT /products/{productId}/reviews/{id}` (Owner/Admin)
* `DELETE /products/{productId}/reviews/{id}` (Owner/Admin)

## Status Code Örnekleri

### 200 OK

```json
{"success":true,"message":"OK","data":{ }}
```

### 201 Created

```json
{"success":true,"message":"Created","data":{ }}
```

### 400 Bad Request (Validation)

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": {
    "Name": ["Name is required."]
  }
}
```

### 401 Unauthorized

```json
{"success":false,"message":"Unauthorized","data":null}
```

### 404 Not Found

```json
{"success":false,"message":"Not found.","data":null}
```

### 409 Conflict

```json
{"success":false,"message":"... already exists.","data":null}
```

### 500 Internal Server Error

```json
{"success":false,"message":"Unexpected error.","data":null}
```

## Logging

* .NET built-in logging kullanılır.
* Docker ile logları izlemek için:

```bash
docker compose logs -f api
```

## Docker ile Çalıştırma (Opsiyonel)

```bash
docker compose up --build
```

Swagger: `http://localhost:5276/swagger`
Health: `GET /health`
