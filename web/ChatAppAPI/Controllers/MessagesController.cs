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
        private readonly IHttpClientFactory imageClientFactory;
        private readonly HttpClient _apiImageClient;

        public MessagesController(ILogger<MessagesController> logger, MessageContext context, IHttpClientFactory imageClientFactory, HttpClient apiImageClient)
        {
            _logger = logger;
            _context = context;
            this.imageClientFactory = imageClientFactory;
            _apiImageClient = apiImageClient;
        }

        [HttpGet("AllMessage")]
        public async Task<List<MessageWithImageDto>> GetMessages()
        {
            _logger.LogInformation("Getting messages in API at {time}", DateTime.Now);
            try
            {
                List<MessageWithImageDto> imageDtosList = new();
                var response = await _context.Messages.ToListAsync();
                if (response != null)
                {
                    Metrics.SuccessCalls.Add(1);
                    foreach (var m in response)
                    {
                        string image = "";
                        if (m.ContainerLocationId == 1)
                        {
                            var imageClient = imageClientFactory.CreateClient("ImageApi1");
                            image = await imageClient.GetFromJsonAsync<string>($"api/Image/getimage/{m.ImagePath}");
                        }
                        else
                        {
                            var imageClient = imageClientFactory.CreateClient("ImageApi2");
                            image = await imageClient.GetFromJsonAsync<string>($"api/Image/getimage/{m.ImagePath}");
                        }

                        var dto = new MessageWithImageDto
                        {
                            message = m,
                            Image = image,
                        };
                        imageDtosList.Add(dto);
                    }
                    return imageDtosList;
                }
                else
                {
                    _logger.LogInformation("nothing in Messages");
                    return new List<MessageWithImageDto>();
                }
            }
            catch (Exception ex)
            {
                Metrics.FailedCalls.Add(1);
                _logger.LogWarning($"{ex.Message} failed to get messages");
                return new List<MessageWithImageDto>();
            }
        }

        [HttpPost("newMessage")]
        public async Task<ActionResult<Message>> PostMessageWithImage([FromBody] MessageWithImageDto messageDto)
        {
            try
            {
                // Handle the uploaded image
                _logger.LogInformation(messageDto.Image);
               
                var imagePath = await _apiImageClient.PostAsJsonAsync("SaveImage", messageDto.Image);
                var containerPath = await imagePath.Content.ReadFromJsonAsync<Container_path>();
                _logger.LogInformation(containerPath.ToString());

                var parsedContainerId = int.Parse(containerPath.Container);

                // Save the message with the image path in the database
                var message = new Message
                {
                    MessageText = messageDto.message.MessageText,
                    Sender = messageDto.message.Sender,
                    Timestamp = messageDto.message.Timestamp,
                    ImagePath = containerPath.FilePath,
                    ContainerLocationId = parsedContainerId
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
                _logger.LogError($"{ex.Message}");
                Metrics.FailedCalls.Add(1);
                Metrics.ApiCalls.Add(1);
                return StatusCode(500, $"{ex.Message}");
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
