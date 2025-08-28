using MediatR;

namespace VictoryCenter.BLL.Commands.Payment.Common;

public interface IPaymentCommandHandler<in TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
}
