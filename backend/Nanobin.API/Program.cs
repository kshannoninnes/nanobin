using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nanobin.API.Data;
using Nanobin.API.Services;

var builder = WebApplication.CreateBuilder(args);

var configuredPath = builder.Configuration["Nanobin:SqlitePath"]
                     ?? throw new InvalidOperationException("Nanobin:SqlitePath not configured");

var fullPath = Path.IsPathRooted(configuredPath)
    ? configuredPath
    : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, configuredPath));

var directoryPath = Path.GetDirectoryName(fullPath);
if (!string.IsNullOrWhiteSpace(directoryPath))
    Directory.CreateDirectory(directoryPath);

var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = fullPath,
    ForeignKeys = true
}.ToString();

var configuredJournalMode = builder.Configuration.GetValue<string>("Nanobin:SqliteJournalMode");

if (!string.IsNullOrWhiteSpace(configuredJournalMode))
{
    await using var connection = new SqliteConnection(connectionString);
    await connection.OpenAsync();

    await using var command = connection.CreateCommand();
    command.CommandText = $"PRAGMA journal_mode={configuredJournalMode};";
    await command.ExecuteNonQueryAsync();
}


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NanobinDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<PasteService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NanobinDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();