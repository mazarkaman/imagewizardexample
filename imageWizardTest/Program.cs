using System.Net;
using System.Security.Cryptography;
using System.Text.Unicode;
using ImageWizard;
using ImageWizard.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddConsole();


byte[] key = RandomNumberGenerator.GetBytes(64);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("default").ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, certChain, policyErrors) => true
    };
});


builder.Services.AddImageWizard(o =>
{
    o.AllowUnsafeUrl = true;
    o.Key = key;

})
    .AddHttpLoader()
    .AddYoutubeLoader()
    .AddGravatarLoader()
    .SetFileCache()
    .AddImageSharp()
    .AddFileLoader();

builder.Services.AddImageWizardClient(o =>
{
    o.Enabled = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseImageWizard();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
