using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs;

public class ChatHub : Hub
{
    public async Task StartTyping(string user)
    {
        await Clients.All.SendAsync("ReceiveTyping", user);
    }
}