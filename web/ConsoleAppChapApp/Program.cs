using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Shared;
using Shared.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http.Json;
class Program
{
    private static readonly ActivitySource MyActivitySource = new("MyCompany.MyProduct.MyLibrary");
    static async Task Main(string[] args)
    {
        //Metrics-----------------------------------------------
            var tracerProvider = Sdk.CreateTracerProviderBuilder()
               .AddSource("MyCompany.MyProduct.MyLibrary")
               .AddConsoleExporter()
               .Build();
            using (var activity = MyActivitySource.StartActivity("SayHello")) 
            {
                activity?.SetTag("foo", 1);
                activity?.SetTag("bar", "Hello, World!");
                activity?.SetTag("baz",new int[] {1,2,3});
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            //Logs
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddOpenTelemetry(options =>
                {
                    options.AddConsoleExporter();
                });
            });
            var logger = loggerFactory.CreateLogger<Program>();
            //Meter
            Meter myMeter = new("Console.Metrics", "1.0");
            Counter<long> RequestCounter = myMeter.CreateCounter<long>("RequestCounter");
            using var meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddMeter("Console.Metrics")
                .AddConsoleExporter()
                .Build();
        //------------------------------------------------------
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        using var dbContext = new MessageContext(new DbContextOptionsBuilder<MessageContext>()
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .Options);
        int sleepInterval = GetSleepInterval();

        while (true)
        {   //check the shared database for image files that are only stored on one server.
            logger.LogInformation("Console checking database for single save images files.");

            var files = dbContext.Messages.Where(message => dbContext.MessageContainerLocations
                    .Count(location => location.MessageId == message.Id) == 1 && !string.IsNullOrEmpty(message.ImagePath)).ToList();

            if (files != null)
            {
                foreach (var message in files)
                {
                    var containerId = message.MessageContainerLocations.First().ContainerLocationId;
                    var baseUri = $"http://image-api-{containerId}:4003/api/Image/getimage/{message.ImagePath}";

                    using var httpClient = new HttpClient();
                    try
                    {
                        var image = await httpClient.GetFromJsonAsync<string>(baseUri);
                        RequestCounter.Add(1, new KeyValuePair<string, object?>("Get Image", HttpMethod.Get));
                        var imageBytes = Convert.FromBase64String(image);
                        var response = await StoreCopy(imageBytes, containerId, logger);

                        RequestCounter.Add(1, new KeyValuePair<string, object?>("Post Image", HttpMethod.Post));
                        logger.LogInformation("Console: Stored image copy");

                        //update containers list
                        if (int.TryParse(response.Container, out int cId))
                        { }
                        var newContainerLocation = new MessageContainerLocation
                        {
                            ContainerLocationId = cId,
                        };
                        message.MessageContainerLocations.Add(newContainerLocation);
                        await dbContext.SaveChangesAsync();
                        logger.LogInformation("Console: Added new Container Location");

                    }
                    catch (Exception ex) { logger.LogError($"Console: Problem with Saving image. {ex.Message}"); }
                }
                //sleep for an interval of time (env) 5 seconds for dev, 30 prod
                Thread.Sleep(TimeSpan.FromSeconds(sleepInterval));

            }
            else
            {
                break;
            }
            //for each instance file, select an image api instance to store a redudnatn copy (at random)
        }
        tracerProvider.Dispose();
    }

    static int GetSleepInterval()
    {
        bool isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        int sleepIntervalSeconds = isProduction ? 30 : 5;
        return sleepIntervalSeconds;
    }

    static async Task<Container_path> StoreCopy(byte[] image, int? containerId, ILogger logger)
    {
        using var httpClient = new HttpClient();
        if (containerId.HasValue)
        {
            try
            {
                //randomly choose an api to send too.
                Random random = new Random();
                var apiChoice = random.Next(1, 3);
                var apiUrl = "";
                switch (containerId)
                {
                    case 1:
                        apiUrl = (apiChoice == 1) ? "http://image-api-2:4003" : "http://image-api-3:4003";
                        break;
                    case 2:
                        apiUrl = (apiChoice == 1) ? "http://image-api-3:4003" : "http://image-api-1:4003";
                        break;
                    case 3:
                        apiUrl = (apiChoice == 1) ? "http://image-api-2:4003" : "http://image-api-1:4003";
                        break;

                }
                // save image to container volume.
                httpClient.BaseAddress = new Uri(apiUrl);
                var volumePath = "/app/Images/SaveCompressedImage";
                var response = await httpClient.PostAsJsonAsync(volumePath, image);
                var containerPath = await response.Content.ReadFromJsonAsync<Container_path>();
                return containerPath;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while storing the image: {ex.Message}");
            }
        }
        return null;
    }

}