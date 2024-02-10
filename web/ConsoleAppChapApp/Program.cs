using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Data;
using System.Net.Http.Json;
class Program
{
    static async Task Main(string[] args)
    {
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
                        var imageBytes = Convert.FromBase64String(image);
                        var response = await StoreCopy(imageBytes, containerId);
                        //update containers list
                        if (int.TryParse(response.Container, out int cId))
                        { }
                        var newContainerLocation = new MessageContainerLocation
                        {
                            ContainerLocationId = cId,
                        };
                        message.MessageContainerLocations.Add(newContainerLocation);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex) { }
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
    }

    static int GetSleepInterval()
    {
        bool isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        int sleepIntervalSeconds = isProduction ? 30 : 5;
        return sleepIntervalSeconds;
    }

    static async Task<Container_path> StoreCopy(byte[] image, int? containerId)
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

            }
        }
        return null;
    }

}