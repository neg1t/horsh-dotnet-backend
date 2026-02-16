using Scalar.AspNetCore;
using testApp.Data;


namespace testApp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var isDev = builder.Environment.IsDevelopment();
            var isProd = builder.Environment.IsProduction();

            // Список origin'ов в зависимости от окружения
            var allowedOrigins = isDev
                ? ["http://localhost:8000", "https://localhost:8000"]
                : new[] { "https://horsh-react-frontend.vercel.app" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                    // .AllowCredentials(); // включите только если действительно нужны credentials и тогда не используйте AllowAnyOrigin
                });
            });


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.RegisterDataAccessServices(builder.Configuration, builder.Environment.IsDevelopment());

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.MapOpenApi();


            app.MapScalarApiReference();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("DefaultCors");

            app.MapControllers();

            app.Services.MigrateDb();

            app.Run();
        }
    }
}
