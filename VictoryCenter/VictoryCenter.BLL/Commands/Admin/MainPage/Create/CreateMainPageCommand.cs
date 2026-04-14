using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.MainPages;

namespace VictoryCenter.BLL.Commands.Admin.MainPage.Create;

public record CreateMainPageCommand(CreateMainPageDto CreateMainPageDto)
    : IRequest<Result<MainPageDto>>;
