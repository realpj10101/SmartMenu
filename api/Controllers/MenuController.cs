using System.ComponentModel.Design;
using api.Controllers.Helpers;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class MenuController(IMenuRepository menuRepository, IMenuRecommendationService menuRecommendationService, IMenuRecommendExplainService menuRecommendExplainService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItem>>> GetAllMenuItems(CancellationToken cancellationToken)
    {
        IEnumerable<MenuItem> menuItems = await menuRepository.GetAllMenuItemsAsync(cancellationToken);

        if (menuItems.Count() == 0)
            return NoContent();

        return Ok(menuItems);
    }

    [HttpPost("menu-items")]
    public async Task<ActionResult<MenuRecommendResponse>> RecommendMenuItems(MenuRecommendRequest request,
        CancellationToken cancellationToken)
    {
        MenuRecommendResponse res = await menuRecommendationService.GetTopCandidateAsync(request, cancellationToken);

        return Ok(res);
    }

    [HttpPost("menu-recommendation")]
    public async Task<ActionResult<MenuRecommendExplainResponse>> RecommendExplainMenuItems(
        MenuRecommendExplainRequest request, CancellationToken cancellationToken
    )
    {
        MenuRecommendExplainResponse res = await menuRecommendExplainService.RecommendAndExplainAsync(request, cancellationToken);

        return Ok(res);
    }

    [HttpPost("menu-talk")]
    public async Task<ActionResult<MenuRecommendTalkResponse>> RecommendTalk(
        MenuRecommendExplainRequest request, CancellationToken cancellationToken)
    {
        MenuRecommendTalkResponse res = await menuRecommendExplainService.RecommendAndTalkAsync(request, cancellationToken);

        return Ok(res);
    }
    
    [HttpGet("ollama-tags")]
    public async Task<IActionResult> Tags([FromServices] IHttpClientFactory factory, CancellationToken ct)
    {
        var http = factory.CreateClient("OllamaClient");
        var res = await http.GetAsync("/api/tags", ct);
        var txt = await res.Content.ReadAsStringAsync(ct);
        return Content(txt, "application/json");
    }
}