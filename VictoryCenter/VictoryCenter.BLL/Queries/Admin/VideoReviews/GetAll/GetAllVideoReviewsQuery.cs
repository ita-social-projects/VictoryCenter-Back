using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Enums;

namespace VictoryCenter.BLL.Queries.Admin.VideoReviews.GetAll;

public record GetAllVideoReviewsQuery(
    bool Archived = false,
    TranslationStatusFilter? TranslationStatusFilter = null)
    : IRequest<Result<List<VideoReviewDto>>>;
