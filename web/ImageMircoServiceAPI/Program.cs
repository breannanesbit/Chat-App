using ImageMircoServiceAPI;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var redisConfiguration = ConfigurationOptions.Parse("localhost:6379,abortConnect=false");
var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);


builder.Services.AddSingleton(connectionMultiplexer);

builder.Services.AddSingleton<RedisClient>();

builder.Services.AddControllers();
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
