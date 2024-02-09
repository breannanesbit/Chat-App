using ImageMircoServiceAPI;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var redisConfiguration = ConfigurationOptions.Parse("localhost:6379,abortConnect=false");
var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);


builder.Services.AddSingleton(connectionMultiplexer);

builder.Services.AddDbContext<MessageContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<RedisClient>();

builder.Services.AddControllers();

builder.Services.AddHttpClient("ImageApi1", client =>
{
    client.BaseAddress = new Uri("http://image-api-1:4003");
});

builder.Services.AddHttpClient("ImageApi2", client =>
{
    client.BaseAddress = new Uri("http://image-api-2:4003");
});

builder.Services.AddHttpClient("ImageApi3", client =>
{
    client.BaseAddress = new Uri("http://image-api-3:4003");
});

builder.Services.AddHttpClient("apiImage", client =>
{
    client.BaseAddress = new Uri("http://client.bre-aub-chatapp.duckdns.org/api/image");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration[WebHostDefaults.ServerUrlsKey] = "http://0.0.0.0:4003";


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();





app.UseAuthorization();

app.MapControllers();

app.Run();
