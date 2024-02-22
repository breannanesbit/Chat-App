using ChatAppWeb;
using ChatAppWeb.classes;
using ChatAppWeb.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddHttpClient("ServerAPI",
//      client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
//    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

//builder.Services.AddHttpClient<PublicClient>(client => client.BaseAddress = new Uri("https://localhost:5000"));

//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
//  .CreateClient("ServerAPI"));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:4002") });

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<LocalStorageAccessor>();

await builder.Build().RunAsync();