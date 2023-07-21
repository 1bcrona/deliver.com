using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DeliverCom.Container.Autofac
{
    public class AutofacServiceProviderFactory : IServiceProviderFactory<AutofacContainerBuilder>
    {
        private readonly Action<AutofacContainerBuilder> _configurationAction;

        public AutofacServiceProviderFactory(Action<AutofacContainerBuilder> configurationAction = null)
        {
            _configurationAction = configurationAction ?? (_ => { });
        }

        public AutofacContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new AutofacContainerBuilder();
            builder.CrossWireServices(services);
            _configurationAction(builder);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(AutofacContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            var container = containerBuilder.Build() as AutofacContainer;
            return new AutofacServiceProvider(container?.Container);
        }
    }
}