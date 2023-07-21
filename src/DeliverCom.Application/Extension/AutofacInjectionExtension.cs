using System.Reflection;
using Autofac;
using DeliverCom.Application.Attributes;
using DeliverCom.Application.Helper;
using DeliverCom.Container.Autofac;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.Domain.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace DeliverCom.Application.Extension
{
    public static class AutofacInjectionExtension
    {
        private const string APPLICATION_ASSEMBLY_NAME = "DeliverCom.Application";

        public static void Bootstrap(this AutofacContainerBuilder builder, IConfiguration configuration)
        {
            ApplicationHelper.InjectCommonServices(builder);
            builder.Register(containerBuilder =>
            {
                containerBuilder.RegisterGeneric(typeof(RequestPipeline<,>)).As(typeof(IPipelineBehavior<,>))
                    .InstancePerLifetimeScope();
            });
            var applicationAssembly = Assembly.Load(APPLICATION_ASSEMBLY_NAME);
            builder.Register(containerBuilder => containerBuilder
                .RegisterType<DeliverComDbContext>()
                .WithParameter("options", new DbContextOptionsBuilder<DeliverComDbContext>()
                    .UseNpgsql(configuration.GetConnectionString("DeliverComDb"))
                    .UseLazyLoadingProxies()
                    .Options)
                .InstancePerLifetimeScope()
            );
            
            

            builder.Register(containerBuilder =>
            {
                containerBuilder
                    .RegisterAssemblyTypes(applicationAssembly)
                    .AsClosedTypesOf(typeof(IRequestHandler<,>))
                    .AsImplementedInterfaces().OnActivated(a =>
                    {
                        var type = a.Instance.GetType();
                        var attribute = type.GetCustomAttribute<UseCaseRouteAttribute>();
                        if (attribute == null) return;
                        var name = attribute.Name;
                        var router = a.Context.Resolve<IRouter>();
                        var useCase = router.Route(name);
                        var property = type.GetField("_useCase", BindingFlags.NonPublic | BindingFlags.Instance);
                        property?.SetValue(a.Instance, useCase);
                    });
            });
            builder.RegisterUseCases();
        }
    }
}