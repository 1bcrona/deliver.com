namespace DeliverCom.Core.Resolving.Infrastructure
{
    public interface IResolver
    {
        T Resolve<T>(object input, bool throwsExceptionIfNotResolve = false);
        object Resolve(object input, Type type, bool throwsExceptionIfNotResolve = false);
    }
}