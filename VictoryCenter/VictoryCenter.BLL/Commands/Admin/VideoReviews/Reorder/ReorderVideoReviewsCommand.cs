using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Reorder;

public record ReorderVideoReviewsCommand(ReorderVideoReviewsDto ReorderVideoReviewsDto)
    : IRequest<Result<Unit>>;
