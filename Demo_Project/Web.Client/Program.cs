using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Shared.Services;
using Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//------------------------------------------------------
// Don't forget to register the Translation service in the client as well, here as a Singleton
builder.Services.AddSingleton(typeof(TranslationService<>));

// Add localization here as well (Microsoft.Extensions.Localization on nuget)
builder.Services.AddLocalization();

// Set culture based on cookie
await SetCultureAsync(builder.Services);
//------------------------------------------------------

await builder.Build().RunAsync();



async Task SetCultureAsync(IServiceCollection services)
{
    const string defaultCulture = "en-US";
    var js = services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
    var result = await js.InvokeAsync<string>("getLanguageCookie");
    var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}