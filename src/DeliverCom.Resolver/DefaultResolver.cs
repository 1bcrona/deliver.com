using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Resolving.Infrastructure;
using DeliverCom.Core.Serialization;

namespace DeliverCom.Resolver
{
    public class DefaultResolver : IResolver
    {
        public T Resolve<T>(object input, bool throwsExceptionIfNotResolve = false)
        {
            return (T)Resolve(input, typeof(T), throwsExceptionIfNotResolve);
        }

        public object Resolve(object input, Type type, bool throwsExceptionIfNotResolve = false)
        {
            var resolved = GetDefault(type);

            type = Nullable.GetUnderlyingType(type) ?? type;
            try
            {
                resolved = Convert.ChangeType(input, type);
            }
            catch
            {
                try
                {
                    var inputString = JsonSerializer.Serialize(input);
                    resolved = JsonSerializer.Deserialize(inputString, type);
                }
                catch
                {
                    if (throwsExceptionIfNotResolve)
                    {
                        var typeName = input == null ? "null" : input.GetType().Name;
                        throw new ArgumentNotValidException(
                            $@"Input Parameter not compatible => Expected: {type.Name},  Actual: {typeName}");
                    }
                }
            }

            return resolved;
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}