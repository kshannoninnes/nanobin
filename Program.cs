using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Nanobin.Components;
using Nanobin.Components.Services;
using Nanobin.Config;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var appDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var configPath = builder.Configuration["ConfigPath"] ?? Path.Combine(appDir, "nanobin");
var dbPath = Path.Combine(configPath, "db", "nanobin.db");
var keysPath = Path.Combine(configPath, "keys");
var logPath = Path.Combine(configPath, "logs");

builder.Services.AddDbContext<NanobinDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.WriteTo.File(path: logPath);
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<PasteService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<NanobinDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();