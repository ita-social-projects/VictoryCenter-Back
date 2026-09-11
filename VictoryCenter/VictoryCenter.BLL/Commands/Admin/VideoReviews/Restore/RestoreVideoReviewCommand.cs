using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.VideoReviews.Restore;

public record RestoreVideoReviewCommand(long Id) : IRequest<Result<long>>;
