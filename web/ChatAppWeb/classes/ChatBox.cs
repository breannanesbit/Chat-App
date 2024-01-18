using ChatAppWeb.Services;
using Shared;

namespace ChatAppWeb.classes;

public class ChatBox
{
    private readonly MessageService service;

    public ChatBox(MessageService service)
    {
        this.service = service;
    }
    public String Time { get; set; }
    public String User { get; set; }
    public String Message { get; set; }

    public static List<ChatBox> Messages { get; set; } = new List<ChatBox>();
    public static Message NewMessage { get; set; } = new Message();

    static ChatBox()
    {
        //Messages = new List<ChatBox>
        //{
        //        new ChatBox{Time = "1/11/2024 1:33:55 PM", User= "Tom", Message= "I have returned!"},
        //        new ChatBox{Time = "1/11/2024 1:34:55 PM", User="Riddle", Message="About Time!"},
        //};
    }


    public async Task SendMessageAsync()
    {
        NewMessage.Timestamp = DateTime.Now;
        await service.SendMessage(NewMessage);


    }

    public async Task<List<Message>> GetMessagesAsync()
    {
        return await service.GetMessages();
    }
}
