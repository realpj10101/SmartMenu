using System.ComponentModel.Design;
using api.Controllers.Helpers;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class MenuController(IMenuRepository menuRepository, IMenuRecommendationService menuRecommendationService) : BaseApiController
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
}