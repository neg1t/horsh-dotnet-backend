using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace testApp.Data
{
    public static class ServiceRegistar
    {
        public static IServiceCollection RegisterDataAccessServices(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgressConnection"));
                if (isDevelopment)
                {
                    options.EnableSensitiveDataLogging();
                }
            });
            return services;
        }

        public static IServiceProvider MigrateDb(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
            return serviceProvider;
        }


    }
}
