using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;



namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<MessagesController> _logger;
        private readonly MessageContext _context;

        public MessagesController(IWebHostEnvironment webHostEnvironment, ILogger<MessagesController> logger, MessageContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _context = context;
        }

        [HttpGet("AllMessage")]
        public async Task<List<Message>> GetMessages()
        {
            _logger.LogInformation("Getting messages in API at {time}", DateTime.Now);
            try
            {
                var response = await _context.Messages.ToListAsync();
                if (response != null)
                {
                    Metrics.SuccessCalls.Add(1);
                    return response;
                }
                else
                {
                    _logger.LogInformation("nothing in Messages");
                    return new List<Message>();
                }
            }
            catch (Exception ex)
            {
                Metrics.FailedCalls.Add(1);
                _logger.LogInformation($"{ex.Message} failed to get messages");
                return new List<Message>();
            }
        }

        [HttpPost("newMessage")]
        public async Task<ActionResult<Message>> PostMessageWithImage([FromForm] MessageWithImageDto messageDto)
        {
            try
            {
                // Handle the uploaded image
                var imagePath = await SaveBase64ImageToVolume(messageDto.Image);

                // Save the message with the image path in the database
                var message = new Message
                {
                    MessageText = messageDto.message.MessageText,
                    Sender = messageDto.message.Sender,
                    Timestamp = messageDto.message.Timestamp,
                    ImagePath = imagePath
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Adding a message with image in the API at {time}", DateTime.Now);
                Metrics.SuccessCalls.Add(1);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't send messages with image in API at {time}", DateTime.Now);
                Metrics.FailedCalls.Add(1);
                Metrics.ApiCalls.Add(1);
                return StatusCode(500, "Internal server error");
            }
        }

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

        //private async Task<string> SaveBase64ImageToVolume(string base64Image)
        //{
        //    var volumePath = "/app/Images";
        //    var filePath = Path.Combine(volumePath, "uploaded_image.txt");
        //    Directory.CreateDirectory(volumePath);
        //    await System.IO.File.WriteAllTextAsync(filePath, base64Image);
        //    return filePath;
        //}

    }
}
