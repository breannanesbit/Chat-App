using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MessageContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(
        serviceNamespace: "demo-namespace",
        serviceName: builder.Environment.ApplicationName,
        serviceVersion: Assembly.GetEntryAssembly()?.GetName().Version?.ToString(),
        serviceInstanceId: Environment.MachineName
    ).AddAttributes(new Dictionary<string, object>
    {
        { "deployment.environment", builder.Environment.EnvironmentName }
    }))
    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
