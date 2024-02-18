using BeerDispenser.WebUi.Abstractions;
using BeerDispenser.WebUi.Implementation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

const string DevApiBaseAddress = "http://localhost:5268";

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Uri webApiHost;


if (builder.HostEnvironment.Environment == "Development")
{
    webApiHost = new Uri("http://localhost:5268");
}
else
{
    webApiHost = new Uri("https://ugly-coyote.azurewebsites.net");
}

builder.RootComponents.Add<BeerDispenser.WebUi.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();

builder.Services.AddScoped<UserNotificationService>();
builder.Services.AddScoped<HttpRequestMessageHandler>();

builder.Services.AddSingleton<TimeZoneService>();
builder.Services.AddSingleton<ILocalStorage, LocalStorage>();
builder.Services.AddSingleton<AccountService>();

builder.Services.AddHttpClient("ServerAPI", client => client.BaseAddress = webApiHost)
    .AddHttpMessageHandler<HttpRequestMessageHandler>();

builder.Services.AddRadzenComponents();

var host = builder.Build();
await host.Services.GetRequiredService<AccountService>().InitializeAsync();

await host.RunAsync();

