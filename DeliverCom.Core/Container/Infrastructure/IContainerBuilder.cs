namespace DeliverCom.Core.Container.Infrastructure
{
    public interface IContainerBuilder
    {
        IContainer Build();
        void RegisterScoped<TImpl, TService>() where TImpl : TService where TService : notnull;
        void RegisterSingleton<TImpl, TService>() where TImpl : TService where TService : notnull;
        void RegisterTransient<TImpl, TService>() where TImpl : TService where TService : notnull;
        void RegisterSelfTransient<TImpl>() where TImpl : notnull;
        void RegisterSelfSingleton<TImpl>() where TImpl : notnull;

        void RegisterSelfScoped<TImpl>() where TImpl : notnull;

        void RegisterGenericScope<TService>(Type implementer);

        void RegisterGenericSingleton<TService>(Type implementer);

        void RegisterGenericTransient<TService>(Type implementer);
        void RegisterUseCases();
    }
}