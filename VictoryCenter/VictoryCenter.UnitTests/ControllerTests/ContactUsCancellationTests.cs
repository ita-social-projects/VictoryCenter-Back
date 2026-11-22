using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VictoryCenter.BLL.Commands.Public.ContactUs;
using VictoryCenter.BLL.DTOs.Public.ContactUs;
using VictoryCenter.WebAPI.Controllers.Public;

namespace VictoryCenter.UnitTests.ControllerTests;

public class ContactUsCancellationTests
{
    [Fact]
    public async Task SubmitContactFormAsync_ForwardsCancellationToMediator()
    {
        using var cancellation = new CancellationTokenSource();
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<SubmitContactFormCommand>(), cancellation.Token))
            .ThrowsAsync(new OperationCanceledException(cancellation.Token));
        using var services = new ServiceCollection().AddSingleton(mediator.Object).BuildServiceProvider();
        var controller = new ContactUsController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestServices = services, RequestAborted = cancellation.Token }
            }
        };
        var dto = new SubmitContactUsFormDto
        {
            CaptchaResponseToken = "token", FromEmail = "test@example.com", FromName = "Test", Subject = "Test", Message = "Test"
        };

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => controller.SubmitContactFormAsync(dto, cancellation.Token));
        mediator.Verify(m => m.Send(It.IsAny<SubmitContactFormCommand>(), cancellation.Token), Times.Once);
    }
}
