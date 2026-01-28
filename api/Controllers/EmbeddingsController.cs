using api.Controllers.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class EmbeddingsController(IMenuEmbeddingService menuEmbeddingService) : BaseApiController
{
    [HttpPost("menu-items")]
    public async Task<ActionResult> EmbedMenuItems(CancellationToken cancellationToken)
    {
        var (updated, skipped) = await menuEmbeddingService.EmbedAllAsync(cancellationToken);

        return Ok(new
        {
            Updated = updated,
            skipped = skipped
        });
    }
}