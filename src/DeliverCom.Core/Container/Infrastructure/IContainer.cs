namespace DeliverCom.Core.Container.Infrastructure
{
    public interface IContainer
    {
        T Resolve<T>() where T : class;
        T ResolveNamed<T>(string name) where T : class;

        T ResolveRequired<T>() where T : notnull;
        T ResolveNamedRequired<T>(string name) where T : notnull;
    }
}