
using Shared;
using Shared.Data;
using System.Net.Http.Json;

namespace ChatAppWeb.Services
{
    public class MessageService
    {
        private readonly HttpClient client;

        public MessageService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<bool> SendMessage(MessageWithImageDto message)
        {
            var response = await client.PostAsJsonAsync("/api/messages/newMessage", message);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else { return false; }
        }

        public async Task<List<Message>> GetMessages()
        {
            var response = await client.GetFromJsonAsync<List<Message>>("/api/Messages/AllMessage");
            if (response != null)
            {
                return response;
            }
            else
            {
                return [];
            }
        }
    }
}
