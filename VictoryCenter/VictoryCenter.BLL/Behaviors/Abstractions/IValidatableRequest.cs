using MediatR;

namespace VictoryCenter.BLL.Behaviors.Abstractions;

public interface IBaseValidatableRequest
{
}

public interface IValidatableRequest : IRequest, IBaseValidatableRequest
{
}

public interface IValidatableRequest<out TResponse> : IRequest<TResponse>, IBaseValidatableRequest
{
}
