using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Delete;

public record DeleteVideoReviewCommand(long Id) : IRequest<Result<long>>;
