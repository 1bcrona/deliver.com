using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Castle.DynamicProxy;
using DeliverCom.Core.Container.Infrastructure;
using DeliverCom.Core.InvocationPropertiesGenerator.Impl;
using DeliverCom.Core.InvocationPropertiesGenerator.Infrastructure;
using DeliverCom.Core.Resolving.Attribute;
using DeliverCom.Core.UseCase.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using IContainer = DeliverCom.Core.Container.Infrastructure.IContainer;

namespace DeliverCom.Container.Autofac
{
    public class AutofacContainerBuilder : IContainerBuilder
    {
        private readonly ContainerBuilder _containerBuilder;

        public AutofacContainerBuilder()
        {
            _containerBuilder = new ContainerBuilder();
        }

        public IContainer Build()
        {
            var container = new AutofacContainer();
            _containerBuilder.RegisterInstance<IContainer>(container).PropertiesAutowired();
            var c = _containerBuilder.Build();
            container.Container = c;
            return container;
        }

        public void RegisterScoped<TImpl, TService>() where TImpl : TService where TService : notnull
        {
            _containerBuilder.RegisterType<TImpl>().As<TService>().InstancePerLifetimeScope();
        }

        public void RegisterSingleton<TImpl, TService>() where TImpl : TService where TService : notnull
        {
            _containerBuilder.RegisterType<TImpl>().As<TService>().SingleInstance();
        }

        public void RegisterTransient<TImpl, TService>() where TImpl : TService where TService : notnull
        {
            _containerBuilder.RegisterType<TImpl>().As<TService>().InstancePerDependency();
        }

        public void RegisterSelfTransient<TImpl>() where TImpl : notnull
        {
            _containerBuilder.RegisterType<TImpl>().AsSelf().InstancePerDependency();
        }

        public void RegisterSelfSingleton<TImpl>() where TImpl : notnull
        {
            _containerBuilder.RegisterType<TImpl>().AsSelf().SingleInstance();
        }

        public void RegisterSelfScoped<TImpl>() where TImpl : notnull
        {
            _containerBuilder.RegisterType<TImpl>().AsSelf().InstancePerLifetimeScope();
        }

        public void RegisterGenericScope<TService>(Type implementer)
        {
            _containerBuilder.RegisterGeneric(implementer).As(typeof(TService)).InstancePerLifetimeScope();
        }

        public void RegisterGenericSingleton<TService>(Type implementer)
        {
            _containerBuilder.RegisterGeneric(implementer).As(typeof(TService)).SingleInstance();
        }

        public void RegisterGenericTransient<TService>(Type implementer)
        {
            _containerBuilder.RegisterGeneric(implementer).As(typeof(TService)).InstancePerDependency();
        }

        public void RegisterUseCases()
        {
            var useCaseAssembly = Assembly.Load("DeliverCom.UseCase");

            var useCases = useCaseAssembly.GetTypes()
                .Where(w => typeof(IUseCase).IsAssignableFrom(w) && w.IsClass && !w.IsAbstract);

            foreach (var type in useCases)
            {
                _containerBuilder.RegisterType(type).AsSelf().InstancePerDependency().OnActivated(args =>
                {
                    var properties = args.Instance.GetType().GetProperties();
                    foreach (var p in properties)
                    {
                        var value = p.GetValue(args.Instance);
                        if (!p.CanWrite || value != null) continue;
                        var attribute = p.GetCustomAttribute<NamedResolveAttribute>();
                        object o = null;
                        if (attribute != null) args.Context.TryResolveNamed(attribute.Name, p.PropertyType, out o);

                        if (o != null) p.SetValue(args.Instance, o);
                    }
                }).PropertiesAutowired();


                foreach (var serviceType in type.GetInterfaces())
                    _containerBuilder.Register(context =>
                        {
                            var proxyGenerator = context.Resolve<ILifetimeScope>().Resolve<ProxyGenerator>();
                            var actual = context.Resolve<ILifetimeScope>().Resolve(type);
                            var interceptor = context.Resolve<IAsyncInterceptor>(
                                new ResolvedParameter(
                                    (pi, _) => pi.ParameterType == typeof(IInvocationPropertiesGenerator) &&
                                               pi.Name == "invocationPropertiesGenerator",
                                    (_, _) => new UseCaseInvocationPropertiesGenerator(actual as IUseCase)));
                            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, actual, interceptor);
                            return proxy;
                        })
                        .As(serviceType)
                        .Keyed(type.FullName ?? string.Empty, serviceType)
                        .InstancePerDependency()
                        .PropertiesAutowired();
            }
        }

        internal void CrossWireServices(IServiceCollection serviceCollection)
        {
            _containerBuilder.Populate(serviceCollection);
        }


        public void Register(Action<ContainerBuilder> action)
        {
            action(_containerBuilder);
        }
    }
}