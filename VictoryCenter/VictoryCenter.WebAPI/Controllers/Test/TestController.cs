using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Test.CreateTestData;
using VictoryCenter.BLL.Commands.Test.DeleteTestData;
using VictoryCenter.BLL.Commands.Test.UpdateTestData;
using VictoryCenter.BLL.DTOs.Test;
using VictoryCenter.BLL.Queries.Test.GetAllTestData;
using VictoryCenter.BLL.Queries.Test.GetTestData;

namespace VictoryCenter.WebAPI.Controllers.Test;

[Route("api/test-data")]
public class TestController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllTestData()
    {
        return HandleResult(await Mediator.Send(new GetAllTestDataQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTestData(int id)
    {
        return HandleResult(await Mediator.Send(new GetTestDataQuery(id)));;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestData([FromBody] CreateTestDataDto createTestData)
    {
        return HandleResult(await Mediator.Send(new CreateTestDataCommand(createTestData)));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTestData([FromBody] UpdateTestDataDto updateTestData)
    {
        return HandleResult(await Mediator.Send(new UpdateTestDataCommand(updateTestData)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTestData(int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTestDataCommand(id)));
    }
}
