using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.Core.UseCase.Infrastructure;
using MediatR;

namespace DeliverCom.Application.Infrastructure
{
    public abstract class BaseRequestHandler<TIn, TOut> : IRequestHandler<TIn, TOut> where TIn : IRequest<TOut>
    {
        protected readonly IRouter _router;
        protected IUseCase _useCase;

        protected BaseRequestHandler(IRouter router)
        {
            _router = router;
        }

        public abstract Task<TOut> Handle(TIn request, CancellationToken cancellationToken);
    }
}