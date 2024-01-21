using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;


namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly MessageContext _context;

        public MessagesController(ILogger<MessagesController> logger, MessageContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("AllMessage")]
        public async Task<List<Message>> GetMessages()
        {
            _logger.LogInformation("Getting messages in API at {time}", DateTime.Now);
            var response = await _context.Messages.ToListAsync();
            if (response != null)
            {
                Metrics.SuccessCalls.Add(1);
                return response;
            }
            else
            {
                Metrics.FailedCalls.Add(1);
                return new List<Message>();
            }
        }

        [HttpPost("newMessage")]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            try
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Adding a message in the API at {time}", DateTime.Now);
                Metrics.SuccessCalls.Add(1);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't send messages in API at {time}", DateTime.Now);
                Metrics.FailedCalls.Add(1);
                Metrics.ApiCalls.Add(1);
            }

            return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
        }
    }
}
