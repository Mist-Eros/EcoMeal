using EcoMeal.Components;
using EcoMeal.EcoMealBlazor.Services;
using EcoMeal.Site.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<AuthenticationHeaderHandler>();

var apiClientBuilder = builder.Services.AddHttpClient("EcoMealApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7173/");
}).AddHttpMessageHandler<AuthenticationHeaderHandler>();

if (builder.Environment.IsDevelopment())
{
    apiClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
}

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("EcoMealApi"));
builder.Services.AddScoped<EcoMeal.EcoMealBlazor.Services.BusinessService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddLocalization();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<DarkModeService>();
builder.Services.AddScoped<CurrencyService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

var supportedCultures = new[] { "en-US", "en-GB", "ro-RO", "de-DE", "fr-FR", "sr-RS", "fi-FI", "ja-JP", "ar-SA" };
app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures)
    .SetDefaultCulture("en-US"));

app.Use(async (context, next) =>
{
    var cookie = context.Request.Cookies["BlazorCulture"];
    if (!string.IsNullOrEmpty(cookie))
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(cookie);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        catch { }
    }
    await next();
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
