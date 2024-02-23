using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs;

public class ChatHub : Hub
{
    public async Task StartTyping(string user)
    {
        await Clients.All.SendAsync("ReceiveTyping", user);
    }

    public async Task NewMessage(DateTime timestamp, Guid guid)
    {
        await Clients.All.SendAsync("ReceiveMessageTimestamp", timestamp, guid);
    }
}