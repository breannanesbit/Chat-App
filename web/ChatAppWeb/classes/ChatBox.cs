using ChatAppWeb.Services;
using Shared;
using Shared.Data;

namespace ChatAppWeb.classes;

public class ChatBox
{
    private readonly MessageService service;

    public ChatBox(MessageService service)
    {
        this.service = service;
    }

    public async Task SendMessageAsync(MessageWithImageDto newMessage)
    {
        await service.SendMessage(newMessage);
    }

    public async Task<List<Message>> GetMessagesAsync()
    {
        return await service.GetMessages();
    }

    //public String Time { get; set; }
    //public String User { get; set; }
    //public String Message { get; set; }


    //static ChatBox()
    //{
    //    //Messages = new List<ChatBox>
    //    //{
    //    //        new ChatBox{Time = "1/11/2024 1:33:55 PM", User= "Tom", Message= "I have returned!"},
    //    //        new ChatBox{Time = "1/11/2024 1:34:55 PM", User="Riddle", Message="About Time!"},
    //    //};
    //}



}
