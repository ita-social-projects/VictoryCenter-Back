using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.MainPage;

namespace VictoryCenter.BLL.Queries.Public.MainPage.GetLocalizedMainPage;

public record GetLocalizedMainPageQuery(long? LanguageId)
    : IRequest<Result<LocalizedMainPageDto>>;
