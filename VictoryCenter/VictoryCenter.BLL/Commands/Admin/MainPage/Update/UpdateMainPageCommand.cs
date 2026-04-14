using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.MainPages;

namespace VictoryCenter.BLL.Commands.Admin.MainPage.Update;

public record UpdateMainPageCommand(UpdateMainPageDto UpdateMainPageDto)
    : IRequest<Result<MainPageDto>>;