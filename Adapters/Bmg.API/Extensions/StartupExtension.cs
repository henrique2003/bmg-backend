using Bmg.BuildingBlocks.Web.API.Validators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using Bmg.Repository.Contexts;
using Bmg.BuildingBlocks.Web.API.Conventions;
using Bmg.BuildingBlocks.Database;
using Bmg.API.Controllers;
using System.Reflection;
using Scrutor;
using Bmg.BuildingBlocks.Web.API.Pipelines;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Bmg.Domain.Clients.Entities;
using Bmg.Application.Clients.UseCases.Upsert;

namespace Bmg.API.Extensions
{

    internal static class StartupExtension
    {
        internal const string DEFAULT_CORS_POLICY_NAME = "Bmg";
        internal static IServiceCollection ConfigureFluentValidations(this IServiceCollection services)
        {
            return services.AddValidatorsFromAssemblyContaining(typeof(UpsertClientCommand));
        }

        internal static IServiceCollection ConfigureMediatr(this IServiceCollection services)
        {
            return services
                    .AddMediatR(cfg =>
                    {
                        cfg.RegisterServicesFromAssembly(typeof(UpsertClientCommand).Assembly);
                        cfg.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly);
                        cfg.RegisterServicesFromAssembly(typeof(ClientController).Assembly);
                    })
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }

        internal static IServiceCollection ConfigureDependecyInjection(this IServiceCollection services)
        {
            services.AddScoped<RequestStateValidator>();

            services.Scan(scan => scan
                          .FromAssemblies(typeof(RequestStateValidator).Assembly,
                                          typeof(BmgDbContext).Assembly,
                                          typeof(ClientController).Assembly,
                                          typeof(UpsertClientCommand).Assembly,
                                          typeof(IUnitOfWork).Assembly,
                                          typeof(Client).Assembly
                                          )
                          .AddClasses()
                          .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                          .AsMatchingInterface()
                          .WithScopedLifetime());

            services.AddHttpContextAccessor();

            return services;
        }

        internal static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(name: DEFAULT_CORS_POLICY_NAME,
                                  policy => policy
                                  .AllowAnyOrigin()
                                  .WithMethods("PUT", "POST", "PATCH", "DELETE", "GET", "OPTIONS")
                                  .AllowAnyHeader()));

            return services;
        }

        internal static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DbConnection") ?? string.Empty;
            var migrationsAssembly = typeof(BmgDbContext).GetTypeInfo().Assembly.GetName().Name;
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

            services.AddDbContext<BmgDbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion, options =>
                    {
                        options
                        .UseMicrosoftJson(MySqlCommonJsonChangeTrackingOptions.FullHierarchyOptimizedFast)
                        .MaxBatchSize(1000)
                        .EnableRetryOnFailure()
                        .MigrationsAssembly(migrationsAssembly)
                        .CommandTimeout(120);
                    })
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            return services;
        }

        public static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services
                .AddControllers(config =>
                {
                    config.Conventions.Add(new PluralizeKebabCaseControllerNamesConvention());

                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));

                })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

            return services;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "SeuIssuer",
                    ValidAudience = "SeuAudience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SECRET"))
                };
            });

            return services;
        }

        public static void RunMigrations(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<BmgDbContext>();
            context.Database.Migrate();
        }

        public static IServiceCollection ConfigureHttp(this IServiceCollection services)
        {
            return services.AddHttpClient();
        }

        internal static IServiceCollection ConfigureDataProtection(this IServiceCollection services)
        {
            services
                .AddDataProtection()
                .PersistKeysToDbContext<BmgDbContext>();

            return services;
        }
    }
}
