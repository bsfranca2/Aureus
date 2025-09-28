using MediatR;

namespace Aureus.Core;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}