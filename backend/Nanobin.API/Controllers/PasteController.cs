using Microsoft.AspNetCore.Mvc;
using Nanobin.API.Services;

namespace Nanobin.Api.Controllers;

[ApiController]
[Route("api/pastes")]
public sealed class PasteController(PasteService repo) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePasteRequest req)
    {
        try
        {
            var (id, expires) = await repo.CreateAsync(req.CiphertextBase64, req.IvBase64);
            return Created($"/api/pastes/{id}", new { id, expires });
        }
        catch (InvalidPasteException)
        {
            return BadRequest("Error decoding paste contents. Did you visit the correct URL?");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            var (ciphertextBase64, ivBase64) = await repo.GetAsync(id);
            return Ok(new {ciphertextBase64, ivBase64});
        }
        catch (Exception ex) when (ex is PasteNotFoundException or PasteExpiredException)
        {
            return NotFound();
        }
    }
}

public record CreatePasteRequest(
    string CiphertextBase64,
    string IvBase64
);
