using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Create;
using VictoryCenter.BLL.Commands.Admin.CompanyProfile.Update;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;
using VictoryCenter.WebAPI.Controllers.Common;

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

    [HttpPut]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompanyProfile([FromBody] UpdateCompanyProfileDto updateCompanyProfileDto)
    {
        return HandleResult(await Mediator.Send(new UpdateCompanyProfileCommand(updateCompanyProfileDto)));
    }
}