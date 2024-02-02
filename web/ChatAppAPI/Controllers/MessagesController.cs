using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

//api needs to have a depencey injection of the other api http client 

namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly MessageContext _context;
        private readonly HttpClient imageClient;

        public MessagesController(ILogger<MessagesController> logger, MessageContext context, HttpClient imageClient)
        {
            _logger = logger;
            _context = context;
            this.imageClient = imageClient;
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
                _logger.LogWarning($"{ex.Message} failed to get messages");
                return new List<Message>();
            }
        }

        [HttpPost("newMessage")]
        public async Task<ActionResult<Message>> PostMessageWithImage([FromBody] MessageWithImageDto messageDto)
        {
            try
            {
                // Handle the uploaded image
                var imagePath = await imageClient.PatchAsJsonAsync("api/Image/SaveImage", messageDto.Image);

                // Save the message with the image path in the database
                var message = new Message
                {
                    MessageText = messageDto.message.MessageText,
                    Sender = messageDto.message.Sender,
                    Timestamp = messageDto.message.Timestamp,
                    ImagePath = await imagePath.Content.ReadAsStringAsync()
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
    }
}

//private async Task<string> SaveBase64ImageToVolume(string base64Image)
//{
//    var volumePath = "/app/Images";
//    var filePath = Path.Combine(volumePath, "uploaded_image.txt");
//    Directory.CreateDirectory(volumePath);
//    await System.IO.File.WriteAllTextAsync(filePath, base64Image);
//    return filePath;
//}
