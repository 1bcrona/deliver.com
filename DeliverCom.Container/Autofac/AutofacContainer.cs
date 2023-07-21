using Autofac;
using IContainer = DeliverCom.Core.Container.Infrastructure.IContainer;

namespace DeliverCom.Container.Autofac
{
    public class AutofacContainer : IContainer, IDisposable
    {
        public ILifetimeScope Container { get; internal set; }

        public T Resolve<T>() where T : class
        {
            return Container.ResolveOptional<T>();
        }


        public T ResolveNamed<T>(string name) where T : class
        {
            return Container.ResolveOptionalKeyed<T>(name);
        }

        public T ResolveRequired<T>() where T : notnull
        {
            return Container.Resolve<T>();
        }

        public T ResolveNamedRequired<T>(string name) where T : notnull
        {
            return Container.ResolveKeyed<T>(name);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Container?.Dispose();
        }

        ~AutofacContainer()
        {
            Dispose(false);
        }
    }
}