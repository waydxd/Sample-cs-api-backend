# TodoApi

Minimal ASP.NET Core API for managing todo items, backed by MySQL and Entity Framework Core.

## Features

- Minimal API endpoints under `/api/todos`
- MySQL persistence via EF Core (Pomelo provider)
- Swagger UI in Development
- Global exception handler returning RFC 7807 style responses

## Requirements

- .NET 10 SDK
- MySQL server

## Getting started

1. Update the connection string in `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=localhost;port=3306;database=todoapi;user=root;password=change_me"
     }
   }
   ```

2. Create the database (and run migrations if you add them later).

3. Run the API:

   ```bash
   dotnet run
   ```

4. Open Swagger UI in Development:

   ```
   http://localhost:5137/swagger
   ```

## API

Base path: `/api`

- `GET /todos` - list todo items
- `POST /todos` - create a todo item

### Example payload

```json
{
  "title": "Buy milk",
  "isCompleted": false
}
```

### Response model

```json
{
  "id": 1,
  "title": "Buy milk",
  "isCompleted": false,
  "createdAt": "2026-05-18T12:00:00Z"
}
```

## Notes

- `POST /todos` rejects empty titles with `400`.
- `CreatedAt` is set to UTC when the item is created.
