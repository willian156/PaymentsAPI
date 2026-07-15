using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaymentsAPI.Application.Payments;
using PaymentsAPI.Domain.Payments;

namespace PaymentsAPI.Infrastructure.DependencyInjection;

public static class PaymentsApiConfiguration
{
    public static IServiceCollection AddPaymentsApi(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddPaymentsApplication();
        services.AddPaymentsPersistence(configuration);
        services.AddPaymentsMessaging(configuration);
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerWithBearerAuthentication();
        return services;
    }

    public static WebApplication UsePaymentsApi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        return app;
    }

    public static async Task InitializePaymentsDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<PaymentsDbContext>().Database.EnsureCreatedAsync();
    }

    private static void AddPaymentsApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<ProcessOrderCommand>()
        );
    }

    private static void AddPaymentsPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database"))
        );
        services.AddScoped<IPaymentRepository, PaymentRepository>();
    }

    private static void AddPaymentsMessaging(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<OrderPlacedConsumer>();
            configurator.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(configuration["RabbitMq:Host"] ?? "localhost", "/", host =>
                {
                    host.Username(configuration["RabbitMq:Username"] ?? "guest");
                    host.Password(configuration["RabbitMq:Password"] ?? "guest");
                });
                rabbit.ReceiveEndpoint(
                    configuration["RabbitMq:OrderQueue"] ?? "payments-orders",
                    endpoint => endpoint.ConfigureConsumer<OrderPlacedConsumer>(context)
                );
            });
        });
    }

    private static void AddSwaggerWithBearerAuthentication(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Informe somente o token JWT. O prefixo Bearer será adicionado automaticamente.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            };

            options.AddSecurityDefinition("Bearer", bearerScheme);
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement { [bearerScheme] = Array.Empty<string>() }
            );
        });
    }
}
