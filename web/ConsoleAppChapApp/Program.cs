using Microsoft.EntityFrameworkCore;
using Shared.Data;
class Program
{
    static void Main(string[] args)
    {
        using var dbContext = new MessageContext(new DbContextOptionsBuilder<MessageContext>()
            .UseNpgsql("your_connection_string_here")
            .Options);

        while (true)
        {   //check the shared database for image files that are only stored on one server.
            var files = dbContext.Messages.Where(message => dbContext.MessageContainerLocations
                    .Count(location => location.MessageId == message.Id) == 1 && !string.IsNullOrEmpty(message.ImagePath)).ToList();

            //for each instance file, select an image api instance to store a redudnatn copy (at random)
            foreach (var message in files)
            {
                StoreCopy(message.ImagePath);
                //sleep for an interval of time (env) 5 seconds for dev, 30 prod
                int sleepInterval = GetSleepInterval();
                Thread.Sleep(TimeSpan.FromSeconds(sleepInterval));
            }
        }
    }

    static int GetSleepInterval()
    {
        bool isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        int sleepIntervalSeconds = isProduction ? 30 : 5; 
        return sleepIntervalSeconds;
    }

    static void StoreCopy(string path)
    {

    }
    
}