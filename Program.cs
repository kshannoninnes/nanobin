using Microsoft.EntityFrameworkCore;
using Nanobin.Components;
using Nanobin.Components.Services;
using Nanobin.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NanobinDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
    options.UseSqlite(connectionString, sqliteOptions =>
    {
        // Configure SQLite to use Write-Ahead Logging
        sqliteOptions.CommandTimeout(60);
    });
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
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