using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace ImageMircoServiceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> _logger;
    private readonly ConnectionMultiplexer redisConnection;
    private readonly RedisClient redisClient;

     public ImageController(ILogger<ImageController> logger, ConnectionMultiplexer redisConnection)
        {
            _logger = logger;
            this.redisConnection = redisConnection;
            this.redisClient = new RedisClient(redisConnection);
        }

        [HttpGet("getimage/{path}")]
        public async Task<string> GetImage(string path)
        {
            var cachedImageBase64 = redisClient.Get<string>(path);

            if (cachedImageBase64 != null)
            {
                _logger.LogInformation("Image found in Redis cache");
                return cachedImageBase64;
            }

            var intervalTime = Environment.GetEnvironmentVariable("TIME_INTERVAL");
            var parsedIntervalTime = int.Parse(intervalTime);

            Thread.Sleep(parsedIntervalTime);
            var imagePath = Path.Combine("/app/Images", path);

            // Load image data from file
            var byteArray = await System.IO.File.ReadAllBytesAsync(imagePath);
            var imageBase64 = Convert.ToBase64String(byteArray);

            // Cache the image data in Redis with a key as the image path
            redisClient.Set(path, imageBase64);

            return imageBase64;
        }


    [HttpPost("SaveImage")]
    public async Task<string> SaveBase64ImageToVolume(string base64Image)
    {
        _logger.LogInformation("made it to base 64");
        // Check if image compression is enabled via environment variable
        bool isCompressionEnabled = Environment.GetEnvironmentVariable("IMAGE_COMPRESSION_ENABLED") == "true";
        var intervalTime = Environment.GetEnvironmentVariable("TIME_INTERVAL");
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
        return filePath;
    }

}