using ImageMagick;
using Microsoft.AspNetCore.Mvc;

namespace ImageMircoServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {


        [HttpPost("SaveImage")]
        private async Task<string> SaveBase64ImageToVolume(string base64Image)
        {
            // Check if image compression is enabled via environment variable
            bool isCompressionEnabled = Environment.GetEnvironmentVariable("IMAGE_COMPRESSION_ENABLED") == "true";

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

            return filePath;
        }
    }
}
