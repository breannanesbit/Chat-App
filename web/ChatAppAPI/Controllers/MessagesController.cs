﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Data;

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

        public MessagesController(ILogger<MessagesController> logger, MessageContext context, IHttpClientFactory imageClientFactory)
        {
            _logger = logger;
            _context = context;
            this.imageClientFactory = imageClientFactory;
        }

        [HttpGet("AllMessage")]
        public async Task<List<MessageWithImageDto>> GetMessages()
        {
            _logger.LogInformation("Getting messages in API at {time}", DateTime.Now);
            try
            {
                List<MessageWithImageDto> imageDtosList = new();
                var response = await _context.Messages.Include((m) => m.MessageContainerLocations).ToListAsync();
                if (response != null)
                {
                    Metrics.SuccessCalls.Add(1);
                    foreach (var m in response)
                    {
                        string image = "";
                        if (m.ImagePath != null)
                        {
                            switch (m.MessageContainerLocations.FirstOrDefault().ContainerLocationId)
                            {
                                case 1:
                                    var imageClient = imageClientFactory.CreateClient("ImageApi1");
                                    image = await imageClient.GetFromJsonAsync<string>($"api/Image/getimage/{m.ImagePath}");
                                    break;
                                case 2:
                                    var imageClient2 = imageClientFactory.CreateClient("ImageApi2");
                                    image = await imageClient2.GetFromJsonAsync<string>($"api/Image/getimage/{m.ImagePath}");
                                    break;
                                case 3:
                                    var imageClient3 = imageClientFactory.CreateClient("ImageApi3");
                                    image = await imageClient3.GetFromJsonAsync<string>($"api/Image/getimage/{m.ImagePath}");
                                    break;

                            }

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
                var imageClient = imageClientFactory.CreateClient("apiImage");
                var imagePath = await imageClient.PostAsJsonAsync("api/Image/SaveImage", messageDto.Image);
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
                    ClientId = messageDto.message.ClientId,
                    LamportCounter = messageDto.message.LamportCounter,
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                var messageId = GetAMessage(message);
                await NewMessageContainerLocation(parsedContainerId, messageId);

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

        [HttpGet("GetMessageByTimestamp/{timestap}")]
        private async Task<List<Message>> MessagesByTimestamp(DateTime timestamp)
        {
            var messages = await _context.Messages.Where((m) => m.Timestamp >= timestamp).ToListAsync();
            return messages;
        }

        [HttpPost("postANewMessageContainerLocation")]
        private async Task NewMessageContainerLocation(int parsedContainerId, Task<Message> messageId)
        {
            var messageContainerLocation = new MessageContainerLocation
            {
                MessageId = messageId.Id,
                ContainerLocationId = parsedContainerId
            };

            _context.MessageContainerLocations.Add(messageContainerLocation);
            await _context.SaveChangesAsync();
        }

        [HttpGet("AMessage")]
        public async Task<Message> GetAMessage(Message message)
        {
            return await _context.Messages.Where((m) => m.MessageText == message.MessageText && m.Timestamp == message.Timestamp).FirstOrDefaultAsync();
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
