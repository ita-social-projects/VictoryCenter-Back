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
}
