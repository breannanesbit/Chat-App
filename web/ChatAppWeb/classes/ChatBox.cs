namespace ChatAppWeb.classes;

public class ChatBox
{
    public String Time { get; set; }
    public String User { get; set; }
    public String Message { get; set; }

    public static List<ChatBox> Messages { get; set; } = new List<ChatBox>();
    public  static ChatBox NewMessage { get; set; } = new ChatBox();

    static ChatBox()
    {
        Messages = new List<ChatBox>
        {
                new ChatBox{Time = "1/11/2024 1:33:55 PM", User= "Tom", Message= "I have returned!"},
                new ChatBox{Time = "1/11/2024 1:34:55 PM", User="Riddle", Message="About Time!"},
        };
    }


    public static void SendMessage()
    {
        NewMessage.Time = DateTime.Now.ToString();
        Messages.Add(NewMessage);
        NewMessage = new ChatBox();
    }

    public static ChatBox[] GetMessages()
    {
        return Messages.ToArray();
    }
}
