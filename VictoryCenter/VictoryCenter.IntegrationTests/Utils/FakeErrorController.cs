using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace VictoryCenter.IntegrationTests.Utils;

[ApiController]
[Route("api/Test")]
public class FakeErrorController : ControllerBase
{
    [HttpGet("Get500Response")]
    public IActionResult Get500Response()
        => StatusCode(500);

    [HttpGet("ThrowException")]
    public IActionResult ThrowException()
        => throw new InvalidOperationException("Test Exception");

    [HttpGet("ThrowValidationException")]
    public IActionResult ThrowValidationException()
        => throw new ValidationException([new ValidationFailure("Name", "Name is required")]);
}
