using Castle.DynamicProxy;
using DeliverCom.Core.Container.Infrastructure;
using DeliverCom.Core.Context.Impl;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.InvocationPropertiesGenerator.Impl;
using DeliverCom.Core.InvocationPropertiesGenerator.Infrastructure;
using DeliverCom.Core.Resolving.Infrastructure;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.Resolver;
using DeliverCom.Routing;
using MediatR;

namespace DeliverCom.Application.Helper
{
    public static class ApplicationHelper
    {
        public static void InjectCommonServices(IContainerBuilder builder)
        {
            builder.RegisterScoped<Mediator, IMediator>();

            builder.RegisterScoped<AmbientExecutionContext, IExecutionContext>();
            builder.RegisterSingleton<DefaultRouteExplorer, IRoutingExplorer>();
            builder.RegisterSingleton<DefaultResolver, IResolver>();
            builder.RegisterSelfSingleton<ProxyGenerator>();
            builder.RegisterTransient<ServiceInterceptor, IAsyncInterceptor>();
            builder.RegisterScoped<DefaultRouter, IRouter>();
            builder.RegisterScoped<DefaultInvocationPropertiesGenerator, IInvocationPropertiesGenerator>();
        }
    }
}