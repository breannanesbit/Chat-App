using ChatAppAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace ChatIntegrationTests;

[TestFixture]
public class Tests
{
    private MessageContext _context { get; set; }
    private MessagesController api { get; set; }

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MessageContext>()
            .UseInMemoryDatabase(databaseName: "Test_Db")
            .Options;

        _context = new MessageContext(options);
        api = new MessagesController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task TestAddAndGetMessageAsync()
    {
        var message = new Shared.Message()
        {
            Id = 0,
            Sender = "Toby Tester",
            MessageText = "This is a Test Message",
            Timestamp = DateTime.Now,
        };

        var addMessageResponse = await api.PostMessage(message);
        //Assert.AreEqual(message.MessageText, addMessageResponse.Value.MessageText);

        var getMessages = await api.GetMessages();
        Assert.IsNotEmpty(getMessages);
        Assert.AreEqual(message.MessageText, getMessages[0].MessageText);
    }
}