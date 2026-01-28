using api.Controllers.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class MenuController(IMenuRepository menuRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItem>>> GetAllMenuItems(CancellationToken cancellationToken)
    {
        IEnumerable<MenuItem> menuItems = await menuRepository.GetAllMenuItemsAsync(cancellationToken);

        if (menuItems.Count() == 0)
            return NoContent();

        return Ok(menuItems);
    }
}