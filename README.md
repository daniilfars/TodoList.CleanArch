# TodoList Clean Architecture API

RESTful API для управления задачами, написанное на .NET 10 с использованием принципов Чистой Архитектуры.

## 🏗 Архитектура проекта
Проект разделен на 4 независимых слоя:
- **Domain**: Сущности базы данных и бизнес-правила (чистые POCO классы).
- **Application**: Интерфейсы сервисов, DTO и логика приложения.
- **Infrastructure**: Реализация доступа к БД (EF Core, PostgreSQL), миграции и сервисы аутентификации (JWT).
- **WebAPI**: Контроллеры, обработка HTTP-запросов и конфигурация DI.

## 🛠 Технологический стек
- **Платформа**: .NET 10 (Preview)
- **БД**: PostgreSQL
- **ORM**: Entity Framework Core (Fluent API конфигурации)
- **Безопасность**: JWT Authentication
- **Инструменты**: Git, Swagger/OpenAPI

## 🚀 Запуск проекта
1. Склонируйте репозиторий: `git clone https://github.com`
2. Настройте строку подключения к PostgreSQL в `WebAPI/appsettings.json`.
3. Примените миграции:
   ```bash
   dotnet ef database update --project Infrastructure --startup-project WebAPI
