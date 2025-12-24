using Microsoft.Data.Sqlite;

namespace Nanobin.API.Data;

public class PasteRepository
{
    private readonly string _sqliteConnectionString;

    public PasteRepository(IConfiguration configuration)
    {
        var databasePath = configuration["Nanobin:SqlitePath"] ?? "nanobin.db";
        _sqliteConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
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
            CREATE TABLE IF NOT EXISTS pastes (
              id TEXT PRIMARY KEY,
              ciphertext BLOB NOT NULL,
              iv BLOB NOT NULL,
              salt BLOB NOT NULL,
              content_type TEXT NOT NULL,
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
            INSERT INTO pastes(id, ciphertext, iv, salt, content_type, created_at_utc, expires_at_utc)
            VALUES($id, $ciphertext, $iv, $salt, $contentType, $createdAtUtc, $expiresAtUtc);
            """;

        command.Parameters.AddWithValue("$id", record.Id);
        command.Parameters.Add("$ciphertext", SqliteType.Blob).Value = record.Ciphertext;
        command.Parameters.Add("$iv", SqliteType.Blob).Value = record.Iv;
        command.Parameters.Add("$salt", SqliteType.Blob).Value = record.Salt;
        command.Parameters.AddWithValue("$contentType", record.ContentType);
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
            SELECT id, ciphertext, iv, salt, content_type, created_at_utc, expires_at_utc
            FROM pastes
            WHERE id = $id
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var createdAtUtc = DateTimeOffset.Parse(reader.GetString(5));
        var expiresAtUtc = DateTimeOffset.Parse(reader.GetString(6));

        return new Paste(
            Id: reader.GetString(0),
            Ciphertext: (byte[])reader["ciphertext"],
            Iv: (byte[])reader["iv"],
            Salt: (byte[])reader["salt"],
            ContentType: reader.GetString(4),
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