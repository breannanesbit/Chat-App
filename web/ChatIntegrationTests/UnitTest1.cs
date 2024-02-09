using ChatAppAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using Shared.Data;

namespace ChatIntegrationTests;

[TestFixture]
public class Tests
{
    public MessageContext _context;
    public MessagesController api;

    [SetUp]
    public void Setup()
    {
        var logger = new Mock<ILogger<MessagesController>>().Object; // Using a mock logger
        var options = new DbContextOptionsBuilder<MessageContext>()
            .UseInMemoryDatabase(databaseName: "Test_Db")
            .Options;

        _context = new MessageContext(options);
        api = new MessagesController(logger, _context, null);
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
        var message = new Shared.Data.Message()
        {
            Id = 0,
            Sender = "Toby Tester",
            MessageText = "This is a Test Message",
            Timestamp = DateTime.Now,
            ImagePath = "",
        };

        var dtoMessage = new MessageWithImageDto()
        {
            Image = "",
            message = message,
        };


        var addMessageResponse = await api.PostMessageWithImage(dtoMessage);
        //Assert.AreEqual(message.MessageText, addMessageResponse.Value.MessageText);

        var getMessages = await api.GetMessages();
        Assert.AreNotEqual(0, getMessages.Count);
        Assert.AreEqual(message.MessageText, getMessages[0].message.MessageText);
    }
}