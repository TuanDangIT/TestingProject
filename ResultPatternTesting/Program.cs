
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ResultPatternTesting.Behaviors;
using ResultPatternTesting.Caching;
using ResultPatternTesting.Entity;
using ResultPatternTesting.Interceptors;
using ResultPatternTesting.Services;
using Serilog;
using System.Reflection;

namespace ResultPatternTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddSingleton<UpdateAuditableInterceptor>();
            builder.Services.AddDbContext<DataDbContext>
                ((sp, options) =>
                {
                    var auditableInterceptor = sp.GetRequiredService<UpdateAuditableInterceptor>();    
                    options.UseNpgsql(builder.Configuration.GetConnectionString("PSQL"))
                    .AddInterceptors(auditableInterceptor);
                });
            builder.Services.AddMediatR(cfg =>
            {
                cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            builder.Services.AddStackExchangeRedisCache(cfg =>
            {
                cfg.Configuration = builder.Configuration.GetConnectionString("Redis");
            });
            builder.Host.UseSerilog((context, cfg) =>
            {
                cfg
                    .WriteTo
                    .Console();
                    //.WriteTo
                    //.Seq("https://localhost:5341");
                    
            });
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            FluentValidationSettings.UnableLanguageManager();
            builder.Services.AddScoped<DataService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}