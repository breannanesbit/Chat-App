using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Data;
using StackExchange.Redis;

namespace ImageMircoServiceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> _logger;
    private readonly ConnectionMultiplexer redisConnection;
    private readonly MessageContext context;
    private readonly IHttpClientFactory imageClientFactory;
    private readonly RedisClient redisClient;

    public ImageController(ILogger<ImageController> logger, ConnectionMultiplexer redisConnection, MessageContext _context, IHttpClientFactory imageClientFactory)
    {
        _logger = logger;
        this.redisConnection = redisConnection;
        context = _context;
        this.imageClientFactory = imageClientFactory;
        this.redisClient = new RedisClient(redisConnection);
    }

    [HttpGet("getimage/{path}")]
    public async Task<IActionResult> GetImage(string path)
    {
        var cachedImageBase64 = redisClient.Get<string>(path);

        if (cachedImageBase64 != null)
        {
            _logger.LogInformation("Image found in Redis cache");
            return Ok(cachedImageBase64);
        }

        var intervalTime = Environment.GetEnvironmentVariable("TIME_INTERVAL");
        var parsedIntervalTime = int.Parse(intervalTime);

        //var imagePath = Path.Combine("/app/Images", path);
        if (System.IO.File.Exists(path))
        {
            var byteArray = await System.IO.File.ReadAllBytesAsync(path);
            var imageBase64 = Convert.ToBase64String(byteArray);

            // Cache the image data in Redis with a key as the image path
            redisClient.Set(path, imageBase64);

            return Ok(imageBase64);
        }
        else
        {
            var imageInDatabase = context.Messages.Include(m => m.MessageContainerLocations)
                                                .Where(m => m.ImagePath == path)
                                                .FirstOrDefault();
            if (imageInDatabase != null)
            {
                var containerId = imageInDatabase.MessageContainerLocations.FirstOrDefault();
                switch (containerId?.ContainerLocationId)
                {
                    case 1:
                        var imageClient = imageClientFactory.CreateClient("ImageApi1");
                        var image = await imageClient.GetFromJsonAsync<string>($"api/Image/getimage/{path}");
                        return Ok(image);
                    case 2:
                        var imageClient2 = imageClientFactory.CreateClient("ImageApi2");
                        var image1 = await imageClient2.GetFromJsonAsync<string>($"api/Image/getimage/{path}");
                        return Ok(image1);
                    case 3:
                        var imageClient3 = imageClientFactory.CreateClient("ImageApi3");
                        var image3 = await imageClient3.GetFromJsonAsync<string>($"api/Image/getimage/{path}");
                        return Ok(image3);

                }
            }
            return BadRequest();

        }

    }


    [HttpPost("SaveImage")]
    public async Task<Container_path> SaveBase64ImageToVolume(string base64Image)
    {
        _logger.LogInformation("made it to base 64");
        // Check if image compression is enabled via environment variable
        bool isCompressionEnabled = Environment.GetEnvironmentVariable("IMAGE_COMPRESSION_ENABLED") == "true";
        var intervalTime = Environment.GetEnvironmentVariable("TIME_INTERVAL");
        var container = Environment.GetEnvironmentVariable("OTHER_CONTAINER");
        string current_container;
        var parsedIntervalTime = int.Parse(intervalTime);
        Thread.Sleep(parsedIntervalTime);


        // Convert base64 string to byte array
        byte[] imageBytes = Convert.FromBase64String(base64Image);

        if (isCompressionEnabled)
        {
            // Create MagickImage from byte array
            using (var image = new MagickImage(imageBytes))
            {
                // Perform compression or other image processing if needed
                image.Quality = 80;

                // Convert the MagickImage back to a byte array
                imageBytes = image.ToByteArray();
            }
        }
        // Save the byte array to a file
        var volumePath = "/app/Images";
        var filePath = Path.Combine(volumePath, "uploaded_image.txt");
        Directory.CreateDirectory(volumePath);
        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
        Thread.Sleep(parsedIntervalTime);
        _logger.LogInformation($"{filePath}");



        var containerAndPath = new Container_path()
        {
            Container = container,
            FilePath = filePath,
        };

        return containerAndPath;
    }



    [HttpPost("SaveCompressedImage")]
    public async Task<Container_path> SaveCompressedImage(byte[] compressedImage)
    {
        _logger.LogInformation("save compressed image");

        // Save the byte array to a file
        var volumePath = "/app/Images";
        var filePath = Path.Combine(volumePath, "uploaded_image.txt");
        Directory.CreateDirectory(volumePath);
        await System.IO.File.WriteAllBytesAsync(filePath, compressedImage);
        _logger.LogInformation($"{filePath}");

        var container = Environment.GetEnvironmentVariable("OTHER_CONTAINER");
        string current_container;

        if (container.Contains("1"))
        {
            current_container = "2";
        }
        else
        {
            current_container = "1";
        }

        var containerAndPath = new Container_path()
        {
            Container = container,
            FilePath = filePath,
        };

        return containerAndPath;
    }

}
