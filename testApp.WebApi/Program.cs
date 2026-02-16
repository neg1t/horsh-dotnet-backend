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

            var origins = builder.Configuration
                .GetSection("CorsPolicy:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

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
