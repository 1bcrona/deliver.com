using System.Data;
using System.Reflection;
using DeliverCom.Core.InvocationPropertiesGenerator.Infrastructure;
using DeliverCom.Core.InvocationPropertiesGenerator.Model;
using DeliverCom.Core.Request.Model;

namespace DeliverCom.Core.InvocationPropertiesGenerator.Impl
{
    public class DefaultInvocationPropertiesGenerator : IInvocationPropertiesGenerator
    {
        public InvocationProperties Generate(MethodInfo methodInfo, object[] args)
        {
            var generate = new InvocationProperties
            {
                RequestParameters = GetParameters(GetParameterDefinition(methodInfo), args)
            };
            return generate;
        }

        private Dictionary<RequestParameter, object> GetParameters(List<RequestParameter> parameterDefinitions,
            IReadOnlyList<object> args)
        {
            var parameterDictionary = new Dictionary<RequestParameter, object>();
            var inputs = parameterDefinitions.Where(w => w.ParameterDirection == ParameterDirection.Input).ToArray();

            for (var i = 0; i < args.Count; i++) parameterDictionary[inputs[i]] = args[i];

            return parameterDictionary;
        }

        private List<RequestParameter> GetParameterDefinition(MethodInfo method)
        {
            var parameterDefinitions = new List<RequestParameter>();
            var @params = method.GetParameters();

            foreach (var param in @params)
            {
                var parameterDefinition = new RequestParameter
                {
                    ParameterName = param.Name,
                    ParameterType = param.ParameterType,
                    ParameterDirection = ParameterDirection.Input
                };
                if (param.ParameterType.Name.Contains("Nullable")) parameterDefinition.ParameterValueType = "Nullable";

                if (param.ParameterType.IsGenericType &&
                    param.ParameterType.GetGenericTypeDefinition() == typeof(List<>))
                    parameterDefinition.ParameterValueType = "List";


                parameterDefinitions.Add(parameterDefinition);
            }

            var returnParameter = new RequestParameter
            {
                ParameterName = "Return",
                ParameterType = method.ReturnType,
                ParameterDirection = ParameterDirection.ReturnValue
            };
            parameterDefinitions.Add(returnParameter);
            return parameterDefinitions;
        }
    }
}