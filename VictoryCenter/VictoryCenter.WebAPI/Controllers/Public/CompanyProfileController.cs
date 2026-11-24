using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.BLL.Queries.Public.CompanyProfile.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class CompanyProfileController : PublicApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompanyProfile()
    {
        return HandleResult(await Mediator.Send(new GetCompanyProfileQuery()));
    }
}
