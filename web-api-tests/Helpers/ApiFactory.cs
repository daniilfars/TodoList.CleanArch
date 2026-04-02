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
    // Генерируем ОДНО имя базы для текущего экземпляра фабрики.
    // Это гарантирует, что внутри одного теста база общая, 
    // но разные классы тестов не будут мешать друг другу.
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Вычищаем старые регистрации
            var descriptors = services.Where(
                d => d.ServiceType.Name.Contains("DbContextOptions") ||
                     d.ServiceType == typeof(AppDbContext) ||
                     d.ServiceType == typeof(IAppDbContext)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // 2. Регистрируем БД с ФИКСИРОВАННЫМ именем базы
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName); // Используем поле класса
                options.UseInternalServiceProvider(null);
            });

            // 3. Привязываем интерфейс
            services.AddScoped<IAppDbContext>(provider =>
                provider.GetRequiredService<AppDbContext>());
        });
    }
}

