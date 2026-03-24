using VictoryCenter.WebAPI.Controllers.Common;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using Microsoft.AspNetCore.Mvc;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class CompanyProfileController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCompanyProfile([FromBody] CreateCompanyProfileDto createCompanyProfileDto)
    {
        return HandleResult(await Mediator.Send(new CreateCompanyProfileCommand(createCompanyProfileDto)));
    }
}