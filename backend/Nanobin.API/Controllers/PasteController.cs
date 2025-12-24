using Microsoft.AspNetCore.Mvc;
using Nanobin.API.Data;

namespace Nanobin.Api.Controllers;

[ApiController]
[Route("api/pastes")]
public sealed class PasteController(PasteRepository repo, IConfiguration config) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePasteRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.ContentType))
            return BadRequest("contentType required");

        byte[] ciphertext, iv, salt;
        try
        {
            ciphertext = Convert.FromBase64String(req.CiphertextBase64);
            iv = Convert.FromBase64String(req.IvBase64);
            salt = Convert.FromBase64String(req.SaltBase64);
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
            salt,
            req.ContentType,
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
            paste.ContentType,
            CiphertextBase64 = Convert.ToBase64String(paste.Ciphertext),
            IvBase64 = Convert.ToBase64String(paste.Iv),
            SaltBase64 = Convert.ToBase64String(paste.Salt)
        });
    }
}

public record CreatePasteRequest(
    string ContentType,
    string CiphertextBase64,
    string IvBase64,
    string SaltBase64
);
