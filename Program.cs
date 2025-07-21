using System.Collections;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Nanobin.Components;
using Nanobin.Components.Services;
using Nanobin.Config;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var logMap = new Dictionary<string, LogEventLevel>()
{
    { "Debug", LogEventLevel.Debug },
    { "Warning", LogEventLevel.Warning },
};

var builder = WebApplication.CreateBuilder(args);

var appDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var configPath = builder.Configuration["ConfigPath"] ?? Path.Combine(appDir, "nanobin");
var dbPath = Path.Combine(configPath, "db", "nanobin.db");
var keysPath = Path.Combine(configPath, "keys");
var logPath = Path.Combine(configPath, "logs");

var logConfig = builder.Configuration["LogLevel"] ?? "Warning";

builder.Services.AddDbContext<NanobinDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

builder.Host.UseSerilog((context, _, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Host", logMap[logConfig])
        .MinimumLevel.Override("Microsoft", logMap[logConfig])
        .MinimumLevel.Override("System", logMap[logConfig])
        .MinimumLevel.Override("Microsoft.AspNetCore", logMap[logConfig])
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", logMap[logConfig])
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database", LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Error)
        .Enrich.FromLogContext();
    
    
    configuration.WriteTo.Console(
        theme: ConsoleTheme.None,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}"
    );

    // Configure file sink with the correct path
    configuration.WriteTo.File(
        path: Path.Combine(logPath, "nanobin_.log"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}",
        fileSizeLimitBytes: 10485760,
        retainedFileCountLimit: 7,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1)
    );
});


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<PasteService>();
builder.Services.AddHostedService<PasteCleanupService>();

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