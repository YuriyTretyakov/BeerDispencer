using BeerDispenser.WebUi;
using BeerDispenser.WebUi.Abstractions;
using BeerDispenser.WebUi.Implementation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using BeerDispenser.WebUi.Implementation.ExternalLogin.Google;
using Microsoft.AspNetCore.Components.Authorization;
using BeerDispenser.WebUi.Implementation.Spinner;
internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        Uri webApiHost;

        Console.WriteLine(builder.HostEnvironment.Environment);
        if (builder.HostEnvironment.Environment == "Development")
        {
            webApiHost = new Uri("http://localhost:5268");
        }
        else
        {
            webApiHost = new Uri("https://ugly-coyote.azurewebsites.net");
        }

        Console.WriteLine($"Web api host: {webApiHost}");

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<DialogService>();

        builder.Services.AddScoped<SpinnerService>();

        builder.Services.AddScoped<UserNotificationService>();
        builder.Services.AddScoped<HttpRequestMessageHandler>();

        builder.Services.AddSingleton<TimeZoneService>();
        builder.Services.AddSingleton<ILocalStorage, LocalStorage>();
        builder.Services.AddSingleton<AccountService>();
     
        builder.Services.AddScoped<ExternalLoginCallbackHandler>();
     
        builder.Services.AddAuthorizationCore();
        

        builder.Services.AddHttpClient("ServerAPI", client => client.BaseAddress = webApiHost)
            .AddHttpMessageHandler<HttpRequestMessageHandler>();

        builder.Services.AddRadzenComponents();
        
        var host = builder.Build();

        await host.Services.GetRequiredService<AccountService>().InitializeAsync();

        await host.RunAsync();
    }
}