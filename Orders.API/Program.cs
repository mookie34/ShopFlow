using FluentValidation;
using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Commands.CreateOrder;
using Orders.Application.Interfaces;
using Orders.Infrastructure.BackgroundJobs;
using Orders.Infrastructure.HttpClients;
using Orders.Infrastructure.Persistence;
using Orders.Infrastructure.Persistence.Repositories;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

// HTTP Clients
builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryUrl"]!);
})
    .AddTransientHttpErrorPolicy(policy =>
    policy.WaitAndRetryAsync(
        retryCount: 3,
       sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
       onRetry: (outcome, timespan, retryAttempt, context) =>
       {
           Console.WriteLine($"[RETRY] Attempt {retryAttempt} after {timespan.TotalSeconds}s. Reason: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
       }
       )
    )
    .AddTransientHttpErrorPolicy(policy =>
    policy.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, timespan) =>
        {
            Console.WriteLine($"[CIRCUIT BREAKER] Breaking the circuit for {timespan.TotalSeconds}s. Reason: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
        },
        onReset: () =>
        {
            Console.WriteLine("[CIRCUIT BREAKER] Circuit reset.");
        }
    ));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CreateOrderHandler).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]);
            h.Password(builder.Configuration["RabbitMQ:Password"]);
        });
    });
});

// Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("OrdersDb")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<OutboxPublisherJob>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Registra el job para correr cada 30 segundos
RecurringJob.AddOrUpdate<OutboxPublisherJob>(
    "outbox-publisher",
    job => job.ExecuteAsync(),
    "*/30 * * * * *"
);

// Aplica migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.Migrate();
}

app.Run();