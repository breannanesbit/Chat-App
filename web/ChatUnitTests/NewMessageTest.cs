using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatAppWeb.classes;

namespace ChatUnitTests;
[TestFixture]
public class NewMessageTest
{
    [Test]
    public void Add_Message()
    {
        int messageCount = ChatBox.Messages.Count;
        ChatBox.NewMessage = new ChatBox { Time = "1/11/2024 1:34:55 PM", User = "Riddle", Message = "About Time!" };

        ChatBox.SendMessage();

        Assert.Greater(ChatBox.Messages.Count,messageCount);
        Assert.AreNotEqual(ChatBox.Messages.Count,messageCount);
    }
}
