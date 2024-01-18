using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;


namespace ChatAppAPI.Controllers
{
    [ApiController]
    [Route("/chatRoom")]
    public class MessagesController : Controller
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

            return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
        }
    }
}
