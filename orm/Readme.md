# ORM (ADO.NET vs Entity Framework)

## Описание

Сравнение двух подходов к работе с БД: чистое ADO.NET и Entity Framework 6. Проект реализует CRUD для сущности `Tasks`.

## Структура

- `AdoNet/TaskRepository.cs` — репозиторий на чистом ADO.NET (SqlConnection, SqlCommand)
- `EF/TaskRepository.cs` — репозиторий на Entity Framework 6 (DbContext, DbSet)
- `Entities/TaskEntity.cs` — сущность `Tasks` (Id, Name, Description, CreatedAt)
- `Data/TaskDbContext.cs` — контекст БД для EF
- `Migrations/` — миграции EF