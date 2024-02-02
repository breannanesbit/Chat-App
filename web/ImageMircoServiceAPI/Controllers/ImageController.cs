using ImageMagick;
using Microsoft.AspNetCore.Mvc;

namespace ImageMircoServiceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> logger;

    public ImageController(ILogger<ImageController> logger)
    {
        this.logger = logger;
    }

    [HttpGet("getimage/{path}")]
    public async Task<string> GetImage(string path)
    {
        var intervalTime = Environment.GetEnvironmentVariable("TIME_INTERVAL");
        var parsedIntervalTime = int.Parse(intervalTime);

        Thread.Sleep(parsedIntervalTime);
        var imagePath = Path.Combine("/app/Images", path);

        // Load image data from file
        var bytearray = await System.IO.File.ReadAllBytesAsync(imagePath);
        Thread.Sleep(parsedIntervalTime);

        return Convert.ToBase64String(bytearray);
    }

    [HttpPost("SaveImage")]
    public async Task<string> SaveBase64ImageToVolume(string base64Image)
    {
        logger.LogInformation("made it to base 64");
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
        logger.LogInformation($"{filePath}");
        return filePath;
    }
}
