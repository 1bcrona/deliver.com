using System.Reflection;
using DeliverCom.Core.InvocationPropertiesGenerator.Model;

namespace DeliverCom.Core.InvocationPropertiesGenerator.Infrastructure
{
    public interface IInvocationPropertiesGenerator
    {
        InvocationProperties Generate(MethodInfo methodInfo, object[] args);
    }
}