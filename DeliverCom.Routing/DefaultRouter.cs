using Castle.DynamicProxy;
using DeliverCom.Core.Container.Infrastructure;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Resolving.Infrastructure;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.Core.UseCase.Infrastructure;
using DeliverCom.Routing.Infrastructure;

namespace DeliverCom.Routing
{
    public sealed class DefaultRouter : BaseRouter
    {
        private readonly IContainer _container;
        private readonly IRoutingExplorer _routingExplorer;

        public DefaultRouter(IRoutingExplorer routingExplorer, IContainer container, IResolver resolver,
            IExecutionContext context) :
            base(resolver, context)
        {
            _routingExplorer = routingExplorer;
            _container = container;
        }

        public override IUseCase Route(string path, object parameters)
        {
            var result = RouteInternal(path);

            FillParameters(result, parameters);

            return result;
        }

        public override IUseCase Route(string path)
        {
            return RouteInternal(path);
        }

        private IUseCase RouteInternal(string path)
        {
            var route = _routingExplorer.GetRoute(path);

            if (string.IsNullOrWhiteSpace(route)) throw new NotFoundException("ROUTE_NOT_FOUND");

            var useCase = _container.ResolveNamed<IUseCase>(route);
            if (useCase == null) throw new NotFoundException("NO_DEFINITION_FOUND_WITH_KEY");
            IUseCase result;
            if (useCase is IProxyTargetAccessor proxy)
            {
                var target = proxy.DynProxyGetTarget();
                result = (IUseCase)target;
            }
            else
            {
                result = useCase;
            }

            return result;
        }
    }
}