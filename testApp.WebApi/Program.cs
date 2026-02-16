using Scalar.AspNetCore;
using testApp.Data;
using System;


namespace testApp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var isDev = builder.Environment.IsDevelopment();

            string[] origins;

            // Пробуем получить как массив из конфигурации
            var originsFromConfig = builder.Configuration
                .GetSection("CorsPolicy:AllowedOrigins")
                .Get<string[]>();

            if (originsFromConfig != null && originsFromConfig.Length > 0)
            {
                origins = originsFromConfig;
            }
            else
            {
                // Если не получилось, пробуем прочитать как строку и разделить
                var originsString = builder.Configuration["CorsPolicy:AllowedOrigins"];
                if (!string.IsNullOrEmpty(originsString))
                {
                    // Убираем квадратные скобки и разделяем по запятой
                    originsString = originsString.Trim('[', ']');
                    origins = originsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(o => o.Trim())
                                          .ToArray();
                }
                else
                {
                    origins = Array.Empty<string>();
                }
            }

            var hasCors = origins.Length != 0;

            if (hasCors)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("DefaultCors", policy =>
                    {
                        policy.WithOrigins(origins)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                });

                // Для отладки - выведите разрешенные origins
                Console.WriteLine($"CORS enabled for: {string.Join(", ", origins)}");
            }
            else
            {
                Console.WriteLine("CORS is disabled - no origins configured");
            }


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.RegisterDataAccessServices(builder.Configuration, builder.Environment.IsDevelopment());

            var app = builder.Build();

            app.MapOpenApi();

            app.MapScalarApiReference();

            app.UseHttpsRedirection();

            if (hasCors)
            {
                app.UseCors("DefaultCors");
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Services.MigrateDb();

            app.Run();
        }
    }
}
