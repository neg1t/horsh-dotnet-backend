using Scalar.AspNetCore;
using testApp.Data;


namespace testApp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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


            app.MapControllers();

            app.Services.MigrateDb();

            app.Run();
        }
    }
}
