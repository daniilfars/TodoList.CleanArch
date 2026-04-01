using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace web_api_tests.Helpers;

public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Находим ВСЕ регистрации, которые касаются DbContextOptions
            // Это удалит и настройки Postgres, и саму регистрацию контекста
            var descriptors = services.Where(
                d => d.ServiceType.Name.Contains("DbContextOptions") ||
                     d.ServiceType == typeof(AppDbContext) ||
                     d.ServiceType == typeof(IAppDbContext)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // 2. Регистрируем БД в памяти С НУЛЯ
            // Используем ServiceCollectionServiceExtensions.AddDbContext, чтобы не было конфликтов
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
                // Это ВАЖНО: заставляем EF использовать новый провайдер сервисов
                options.UseInternalServiceProvider(null);
            });

            // 3. Снова привязываем интерфейс к новому контексту
            services.AddScoped<IAppDbContext>(provider =>
                provider.GetRequiredService<AppDbContext>());
        });
    }
}
