using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.VideoReviews;

namespace VictoryCenter.BLL.Commands.Admin.Localization.VideoReviews.Delete;

public record DeleteVideoReviewLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeleteVideoReviewLocalizationDto>>;
