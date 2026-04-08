using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.MainPages;

namespace VictoryCenter.BLL.Queries.Admin.MainPage.GetMainPage;

public record GetMainPageQuery : IRequest<Result<MainPageDto>>;
