using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VictoryCenter.BLL.Commands.Public.ContactUs;
using VictoryCenter.BLL.DTOs.Public.ContactUs;
using VictoryCenter.WebAPI.Controllers.Common;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class ContactUsController : BaseApiController
{
    [HttpPost]
    [EnableRateLimiting(RateLimitingPolicyNameConstants.SubmitContactUsForm)]
    [ProducesResponseType(typeof(ContactUsFormDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> SubmitContactFormAsync([FromBody] SubmitContactUsFormDto dto)
    {
        return HandleResult(await Mediator.Send(new SubmitContactFormCommand(dto)));
    }
}
