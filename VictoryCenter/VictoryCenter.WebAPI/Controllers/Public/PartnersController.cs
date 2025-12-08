using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.Partners;
using VictoryCenter.BLL.Queries.Public.Partners.GetPage;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class PartnersController : BaseApiController
{
    [HttpGet("page")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartnersPageDto))]
    public async Task<IActionResult> GetPartnersPage()
    {
        return HandleResult(await Mediator.Send(new GetPartnersPageQuery()));
    }
}
