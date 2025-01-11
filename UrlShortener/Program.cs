using FastEndpoints;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UrlShortener.Common.Exceptions;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
// Add services to the container.
builder.Host.UseSerilog((context, configuration)
    => configuration.ReadFrom.Configuration(context.Configuration), writeToProviders: true);
builder.Services.RegisterInfrastructure(config);
builder.Logging.ConfigureInfrastructureLogging();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await using var serviceScope = app.Services.CreateAsyncScope();
    await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<UrlShortenerContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseExceptionHandler();

var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

app.UseFastEndpoints(options => options.Endpoints.RoutePrefix = "api");

app.Run();

