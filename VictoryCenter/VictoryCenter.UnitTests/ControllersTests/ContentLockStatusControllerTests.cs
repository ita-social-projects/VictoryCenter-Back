using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.WebAPI.Controllers.Admin;

namespace VictoryCenter.UnitTests.ControllersTests;

public class ContentLockStatusControllerTests
{
    [Fact]
    public void GetStatus_WhenResourceIsSupported_ReturnsCurrentStatus()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = new Mock<IContentLockResourcePolicy>();
        var resource = new ContentLockResourceDto { ResourceType = ContentLockConstants.EventNewsResourceType, ResourceId = 1 };
        var expected = new ContentLockStatusDto { IsLocked = true, ResourceType = resource.ResourceType, ResourceId = resource.ResourceId };
        policy.Setup(item => item.IsSupported(resource)).Returns(true);
        lockManager.Setup(item => item.GetStatus(resource)).Returns(expected);
        var controller = new ContentLockStatusController(lockManager.Object, policy.Object);

        var result = controller.GetStatus(resource.ResourceType, resource.ResourceId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, okResult.Value);
    }

    [Fact]
    public void GetStatus_WhenResourceIsUnsupported_ReturnsBadRequestWithoutAccessingLockManager()
    {
        var lockManager = new Mock<IContentLockManager>();
        var policy = new Mock<IContentLockResourcePolicy>();
        var problemDetailsFactory = new Mock<ProblemDetailsFactory>();
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Detail = ContentLockConstants.UnsupportedResourceMessage,
        };
        policy.Setup(item => item.IsSupported(It.IsAny<ContentLockResourceDto>())).Returns(false);
        problemDetailsFactory.Setup(item => item.CreateProblemDetails(
                It.IsAny<HttpContext>(),
                StatusCodes.Status400BadRequest,
                It.IsAny<string>(),
                It.IsAny<string>(),
                ContentLockConstants.UnsupportedResourceMessage,
                It.IsAny<string>()))
            .Returns(problemDetails);
        var services = new ServiceCollection();
        services.AddSingleton(problemDetailsFactory.Object);
        var controller = new ContentLockStatusController(lockManager.Object, policy.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() }
            }
        };

        var result = controller.GetStatus("UnknownResource", 1);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Same(problemDetails, badRequestResult.Value);
        lockManager.Verify(item => item.GetStatus(It.IsAny<ContentLockResourceDto>()), Times.Never);
    }
}
