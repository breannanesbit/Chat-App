using ChatAppAPI;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Loki;
using Shared;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();

        // Add services to the container.

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        // MessageContext registration
        builder.Services.AddDbContext<MessageContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // HttpClient registration
        builder.Services.AddHttpClient("ImageApi1", client =>
        {
            client.BaseAddress = new Uri("http://image-api-1:4003");
        });

        builder.Services.AddHttpClient("ImageApi2", client =>
        {
            client.BaseAddress = new Uri("http://image-api-2:4003");
        });

        builder.Services.AddHttpClient("apiImage", client =>
        {
            client.BaseAddress = new Uri("http://api/image");
        });
        // Controller registration
        builder.Services.AddControllers();
        builder.Services.AddScoped<MessageContext>();


        // ... other registrations


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
                .AddOtlpExporter()
                .AddMeter(Metrics.m.Name)
                .AddMeter(Metrics.FailedCalls.Name)
                .AddMeter(Metrics.SuccessCalls.Name));

        //Logs--------------------------------------------------------------------
        /* ILogger Logger = new LoggerConfiguration()
             .WriteTo.GrafanaLoki("http://loki:3100")
             .CreateLogger();*/
        builder.Host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.WriteTo.Console()
            .Enrich.WithProperty("job", "your-api-job")
            .Enrich.WithExceptionDetails()
            .WriteTo.LokiHttp("http://loki:3100");


            /*.WriteTo.Sink(new GrafanaLokiSink("http://loki:3100"));*/
        });

        //------------------------------------------------------------------------

        var app = builder.Build();
        // Configure the HTTP request pipeline.

        app.UseSwagger();
        app.UseSwaggerUI();



        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}