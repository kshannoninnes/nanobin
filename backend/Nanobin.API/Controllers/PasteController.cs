using Microsoft.AspNetCore.Mvc;
using Nanobin.API.Model;
using Nanobin.API.Services;

namespace Nanobin.Api.Controllers;

[ApiController]
[Route("api/pastes")]
public sealed class PasteController(SQLiteService repo, IConfiguration config) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePasteRequest req)
    {
        byte[] ciphertext, iv;
        try
        {
            ciphertext = Convert.FromBase64String(req.CiphertextBase64);
            iv = Convert.FromBase64String(req.IvBase64);
        }
        catch
        {
            return BadRequest("invalid base64");
        }

        var now = DateTimeOffset.UtcNow;
        var expires = now.AddDays(config.GetValue("Nanobin:DefaultTtlDays", 7));
        var id = Guid.NewGuid().ToString("N")[..12];

        await repo.InsertAsync(new Paste(
            id,
            ciphertext,
            iv,
            now,
            expires
        ));

        return Created($"/api/pastes/{id}", new { id, expires });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var paste = await repo.GetAsync(id);
        if (paste is null)
            return NotFound();

        if (paste.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            await repo.DeleteAsync(id);
            return NotFound();
        }

        return Ok(new
        {
            CiphertextBase64 = Convert.ToBase64String(paste.Ciphertext),
            IvBase64 = Convert.ToBase64String(paste.Iv)
        });
    }
}

public record CreatePasteRequest(
    string CiphertextBase64,
    string IvBase64
);
