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
                var imagePath = await SaveImage(messageDto.Image);

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

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return null; // No image uploaded
            }

            try
            {
                var uploads = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads");
                Directory.CreateDirectory(uploads); // Ensure the directory exists

                var imagePath = Path.Combine(uploads, image.FileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                return imagePath;
            }
            catch (Exception ex)
            {
                // Handle exception (log, return error response, etc.)
                return null;
            }
        }

    }
}
