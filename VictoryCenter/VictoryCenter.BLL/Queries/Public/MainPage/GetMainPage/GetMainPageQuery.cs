using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.MainPages;

namespace VictoryCenter.BLL.Queries.Public.MainPage.GetMainPage;

public record GetMainPageQuery(bool IncludeHiddenMetrics = false) : IRequest<Result<MainPageDto>>;
