using Microsoft.Data.Sqlite;
using Nanobin.API.Model;

namespace Nanobin.API.Services;

public class SQLiteService
{
    private readonly string _sqliteConnectionString;

    public SQLiteService(IConfiguration configuration)
    {
        var configuredPath = configuration["Nanobin:SqlitePath"] ?? "nanobin.db";
        
        var baseDirectory = AppContext.BaseDirectory;
        var fullPath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(baseDirectory, configuredPath));

        // Create the directory for the database, if it doesn't already exist
        var directoryPath = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
            Directory.CreateDirectory(directoryPath);
    
        _sqliteConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            ForeignKeys = true
        }.ToString();
    }


    public async Task InitialiseAsync()
    {
        await using var connection = new SqliteConnection(_sqliteConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            DROP TABLE IF EXISTS pastes;
            CREATE TABLE IF NOT EXISTS pastes (
              id TEXT PRIMARY KEY,
              ciphertext BLOB NOT NULL,
              iv BLOB NOT NULL,
              created_at_utc TEXT NOT NULL,
              expires_at_utc TEXT NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_pastes_expires_at_utc ON pastes(expires_at_utc);
            """;

        await command.ExecuteNonQueryAsync();
    }

    public async Task InsertAsync(Paste record)
    {
        await using var connection = new SqliteConnection(_sqliteConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO pastes(id, ciphertext, iv, created_at_utc, expires_at_utc)
            VALUES($id, $ciphertext, $iv, $createdAtUtc, $expiresAtUtc);
            """;

        command.Parameters.AddWithValue("$id", record.Id);
        command.Parameters.Add("$ciphertext", SqliteType.Blob).Value = record.Ciphertext;
        command.Parameters.Add("$iv", SqliteType.Blob).Value = record.Iv;
        command.Parameters.AddWithValue("$createdAtUtc", record.CreatedAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$expiresAtUtc", record.ExpiresAtUtc.ToString("O"));

        await command.ExecuteNonQueryAsync();
    }

    public async Task<Paste?> GetAsync(string id)
    {
        await using var connection = new SqliteConnection(_sqliteConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT id, ciphertext, iv, created_at_utc, expires_at_utc
            FROM pastes
            WHERE id = $id
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var createdAtUtc = DateTimeOffset.Parse(reader.GetString(3));
        var expiresAtUtc = DateTimeOffset.Parse(reader.GetString(4));

        return new Paste(
            Id: reader.GetString(0),
            Ciphertext: (byte[])reader["ciphertext"],
            Iv: (byte[])reader["iv"],
            CreatedAtUtc: createdAtUtc,
            ExpiresAtUtc: expiresAtUtc
        );
    }

    public async Task DeleteAsync(string id)
    {
        await using var connection = new SqliteConnection(_sqliteConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM pastes WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        await command.ExecuteNonQueryAsync();
    }
}