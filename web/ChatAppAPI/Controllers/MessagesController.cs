using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;


namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageContext _context;

        public MessagesController(MessageContext context)
        {
            _context = context;
        }

        [HttpGet("AllMessage")]
        public async Task<List<Message>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        [HttpPost("newMessage")]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            Metrics.ApiCalls.Add(1);


            return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
        }
    }
}
