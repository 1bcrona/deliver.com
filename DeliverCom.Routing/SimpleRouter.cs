using DeliverCom.Core.Container.Infrastructure;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Resolving.Infrastructure;
using DeliverCom.Core.UseCase.Infrastructure;
using DeliverCom.Routing.Infrastructure;

namespace DeliverCom.Routing
{
    public class SimpleRouter : BaseRouter
    {
        private readonly IContainer _container;

        public SimpleRouter(IContainer container, IResolver resolver, IExecutionContext context) : base(resolver,
            context)
        {
            _container = container;
        }

        public override IUseCase Route(string path, object parameters)
        {
            var useCase = RouteInternal(path);
            FillParameters(useCase, parameters);
            return useCase;
        }

        public override IUseCase Route(string path)
        {
            return RouteInternal(path);
        }

        private IUseCase RouteInternal(string path)
        {
            var useCase = _container.ResolveNamed<IUseCase>(path);

            if (useCase == null)
                throw new NotFoundException("USE_CASE_NOT_FOUND");

            return useCase;
        }
    }
}