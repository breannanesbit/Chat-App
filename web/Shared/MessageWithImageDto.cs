using Microsoft.AspNetCore.Http;

namespace Shared
{
    public class MessageWithImageDto
    {
        public Message message { get; set; }
        public IFormFile Image { get; set; }
    }
}
