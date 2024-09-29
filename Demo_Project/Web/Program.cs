using Microsoft.AspNetCore.Localization;
using Shared.Services;
using TranslatorDemo.Components;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

//------------------------------------------------------
// Register the Translation service as Scoped
builder.Services.AddScoped(typeof(TranslationService<>));


// Add the built-in localization
builder.Services.AddLocalization();

// Add controllers so the CultureController can work
builder.Services.AddControllers();
//------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//------------------------------------------------------
// Also map the controllers
app.MapControllers();

// Configuring localization so it works with the server
var supportedCultures = new[] { "en-US", "es-ES", "hu-HU" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures)
    .AddInitialRequestCultureProvider(new CookieRequestCultureProvider
    {
        CookieName = "UserLanguage"
    });

app.UseRequestLocalization(localizationOptions);
//------------------------------------------------------

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Web.Client.Components.Pages.Counter).Assembly);
    

app.Run();