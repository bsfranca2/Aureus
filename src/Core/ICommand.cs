using MediatR;

namespace Aureus.Core;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}