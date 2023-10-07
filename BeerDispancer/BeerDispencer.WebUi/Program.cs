using BeerDispenser.WebUi.Abstractions;
using BeerDispenser.WebUi.Implementation;
using BeerDispenser.WebUi.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

const string DevApiBaseAddress = "http://localhost:5268";

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<BeerDispenser.WebUi.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();

builder.Services.AddScoped<UserNotificationService>();
builder.Services.AddScoped<HttpRequestMessageHandler>();

builder.Services.AddScoped<TimeZoneService>();
builder.Services.AddSingleton<ILocalStorage, LocalStorage>();

builder.Services.AddHttpClient("ServerAPI", client => client.BaseAddress = new Uri(DevApiBaseAddress))
    .AddHttpMessageHandler<HttpRequestMessageHandler>();

builder.Services.AddRadzenComponents();
await builder.Build().RunAsync();

