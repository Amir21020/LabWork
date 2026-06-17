# TimeTracker — веб-приложение для учёта рабочего времени

Приложение для учёта трудозатрат по проектам и задачам, разработанное в соответствии с техническим заданием (Вариант 1).

## Технологии

| Слой | Технологии |
|------|------------|
| **Backend** | ASP.NET Core 10 Web API (REST-архитектура) |
| **Frontend** | Vue 3 + Vite 8 + Tailwind CSS 4 |
| **ORM & База данных** | Entity Framework Core 10, PostgreSQL 16 |
| **Тестирование** | xUnit, Testcontainers (интеграция с СУБД в контейнере) |

## Архитектура

```
web/
├── api/ # .NET Backend
│ ├── TimeTrackingApp.Api/ # REST-контроллеры, Swagger, Middleware
│ ├── TimeTrackingApp.BL/ # Сервисы бизнес-логики, валидаторы, DTO
│ ├── TimeTrackingApp.DAL/ # DbContext, миграции EF Core, сущности БД
│ └── TimeTrackingApp.Tests/ # Модульные и интеграционные тесты
├── client/ # Vue 3 SPA (фронтенд)
├── compose.yml # конфигурация docker-compose
```

## Запуск

### Через Docker Compose

```bash
cd web
docker compose up --build
```

Сервисы:
- **client** — http://localhost (Nginx, SPA)
- **api** — http://localhost:5163 (Swagger: /swagger)

### Локальный запуск (без Docker)

**Backend:**

1. Укажите вашу строку подключения к PostgreSQL в `web/api/TimeTrackingApp.Api/appsettings.Development.json`
```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5435;Database=time_tracker_db;Username=postgres;Password=your_password"
     }
   }
```

2. Примените миграцию для создания БД:

```bash
cd web/api/TimeTrackingApp.Api
dotnet ef database update --project ../TimeTrackingApp.DAL
```

3. Запустите проект:

```bash
dotnet run
```


**Frontend:**

1. Перейдите в папку клиента создайте .env.local:
```bash
cd web/client
echo "VITE_API_URL=http://localhost:5163" > env.local
```

2. Установите зависимости и запустите локальный сервер:
```bash
npm install
npm run dev
```

По умолчанию запускается на http://localhost:5173

## Тестирование

```bash
cd web/api/TimeTrackingApp.Tests
dotnet test
```

Интеграционные тесты используют Testcontainers для поднятия PostgreSQL в контейнере.
