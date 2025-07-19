using MediatR;

namespace VictoryCenter.BLL.Commands.Donation.Common;

public interface IDonationCommandHandler<in TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
}
