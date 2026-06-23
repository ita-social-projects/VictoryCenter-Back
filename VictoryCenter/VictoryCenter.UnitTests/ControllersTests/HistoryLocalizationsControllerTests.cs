using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;
using VictoryCenter.WebAPI.Controllers.Admin.Localization;

namespace VictoryCenter.UnitTests.ControllersTests;

public class HistoryLocalizationsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ProblemDetailsFactory> _problemDetailsFactoryMock;
    private readonly HistoryLocalizationsController _sut;

    public HistoryLocalizationsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _problemDetailsFactoryMock = new Mock<ProblemDetailsFactory>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(_mediatorMock.Object);
        serviceCollection.AddSingleton(_problemDetailsFactoryMock.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new HistoryLocalizationsController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestServices = serviceProvider }
            }
        };
    }

    [Fact]
    public async Task CreateHistoryLocalization_ReturnsOk()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateHistoryLocalizationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<HistorySectionLocalizationDto>()));

        var result = await _sut.CreateHistoryLocalization([]);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateHistoryLocalization_ReturnsOk()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateHistoryLocalizationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new List<HistorySectionLocalizationDto>()));

        var result = await _sut.UpdateHistoryLocalization([], 1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateHistorySectionLocalization_EntityIdMismatch_ReturnsBadRequest()
    {
        _problemDetailsFactoryMock.Setup(p => p.CreateProblemDetails(
            It.IsAny<HttpContext>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ProblemDetails { Status = 400, Detail = "Mismatch" });

        var dto = new UpdateHistorySectionLocalizationDto { EntityId = 1 };

        var result = await _sut.UpdateHistorySectionLocalization(dto, EntityId: 2, LanguageId: 1);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateHistorySectionLocalization_ValidIds_ReturnsOk()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateHistorySectionLocalizationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(new HistorySectionLocalizationDto()));

        var dto = new UpdateHistorySectionLocalizationDto { EntityId = 1 };

        var result = await _sut.UpdateHistorySectionLocalization(dto, EntityId: 1, LanguageId: 1);

        Assert.IsType<OkObjectResult>(result);
    }
}
