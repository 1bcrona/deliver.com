using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DeliverCom.Core.Helper;
using DeliverCom.Core.InvocationPropertiesGenerator.Infrastructure;
using DeliverCom.Core.InvocationPropertiesGenerator.Model;
using DeliverCom.Core.Request.Attribute;
using DeliverCom.Core.Request.Model;
using DeliverCom.Core.UseCase.Infrastructure;

namespace DeliverCom.Core.InvocationPropertiesGenerator.Impl
{
    public class UseCaseInvocationPropertiesGenerator : IInvocationPropertiesGenerator
    {
        private const string StrNull = "null";
        private const int MaxKeyLength = 512;
        private readonly IUseCase _useCase;

        public UseCaseInvocationPropertiesGenerator(IUseCase useCase)
        {
            _useCase = useCase;
        }


        public InvocationProperties Generate(MethodInfo methodInfo, object[] args)
        {
            object input = null;
            if (!_useCase.GetType().GetInterfaces()
                    .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IUseCase<>))) return null;

            var invocationProperties = new InvocationProperties();
            if (methodInfo.Name == "Execute")
            {
                input = _useCase.GetType().GetProperty("Input", BindingFlags.Public | BindingFlags.Instance)
                    ?.GetValue(_useCase);
                if (input != null)
                {
                    var requestParameters = GetParameterDefinition(input, methodInfo);
                    invocationProperties.RequestParameters = requestParameters;
                }
            }

            if (methodInfo.GetCustomAttribute<CachingAttribute>() == null) return invocationProperties;
            var prefix = $"UC:{methodInfo.ReflectedType?.Name}:{methodInfo.Name}:";
            var cachingAttribute = methodInfo.GetCustomAttribute<CachingAttribute>();
            var cacheKey = input == null ? string.Empty : GetCacheKey(prefix, input);
            var duration = cachingAttribute?.DurationInSeconds;
            invocationProperties.CachingProperties = new CachingProperties
            {
                CacheKey = cacheKey,
                CacheDuration = duration ?? -1
            };
            return invocationProperties;
        }

        private IDictionary<RequestParameter, object> GetParameterDefinition(object input, MethodInfo method)
        {
            var dictionary = new Dictionary<RequestParameter, object>();
            var @params = input.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var param in @params)
            {
                var parameterDefinition = new RequestParameter
                {
                    ParameterName = param.Name,
                    ParameterType = param.PropertyType,
                    ParameterDirection = ParameterDirection.Input
                };
                if (param.PropertyType.Name.Contains("Nullable")) parameterDefinition.ParameterValueType = "Nullable";

                if (param.PropertyType.IsGenericType &&
                    param.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    parameterDefinition.ParameterValueType = "List";
                dictionary.Add(parameterDefinition, param.GetValue(input));
            }

            var returnParameter = new RequestParameter
            {
                ParameterName = "Return",
                ParameterType = method.ReturnType,
                ParameterDirection = ParameterDirection.ReturnValue
            };
            dictionary.Add(returnParameter, null);


            return dictionary;
        }

        private string GetCacheKey(string prefix, object input)
        {
            var sb = new StringBuilder();

            sb.Append(prefix);
            sb.Append("|");
            var paramCacheKey = GetParamString(GetKvpFromInput(input));
            var keyLen = MaxKeyLength - sb.Length;

            if (paramCacheKey.Length <= keyLen)
            {
                sb.Append(paramCacheKey);
            }
            else
            {
                var hash = GetHash(paramCacheKey);
                if (string.IsNullOrEmpty(hash)) hash = StrNull;

                if (keyLen > 0 && paramCacheKey.Length < keyLen)
                {
                    sb.Append("|" + paramCacheKey.Substring(0, keyLen - hash.Length));
                    sb.Append('~');
                }

                sb.Append(hash);
            }


            return sb.ToString();
        }

        private Dictionary<string, object> GetKvpFromInput(object input)
        {
            var dictionary = input.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(
                    propertyInfo => propertyInfo.Name,
                    propertyInfo => propertyInfo.GetValue(input));

            return dictionary;
        }

        private string GetParamString(Dictionary<string, object> eArgs)
        {
            var paramBuilder = new StringBuilder(40);
            if (eArgs == null || eArgs.Count == 0)
            {
                paramBuilder.Append("ACnt=0|Args=null");
                return paramBuilder.ToString();
            }

            var parameterKeys = new List<string>
            {
                string.Join("=", "PCount", eArgs.Count)
            };

            foreach (var eArg in eArgs)
            {
                var value = eArg.Value;

                if (value == null)
                {
                    parameterKeys.Add(string.Join("=", eArg.Key, StrNull));
                    continue;
                }

                string parameterString;

                var type = value.GetType();
                var tc = Type.GetTypeCode(type);

                switch (tc)
                {
                    case TypeCode.String:
                    {
                        parameterString = (string)value;
                        parameterString = parameterString.Length < 32 ? parameterString : GetHash(parameterString);

                        break;
                    }
                    case TypeCode.DateTime:
                        parameterString = ((DateTime)value).Ticks.ToString();
                        break;
                    default:
                    {
                        if (type == typeof(DateTime?))
                        {
                            parameterString = ((DateTime?)value).Value.Ticks.ToString();
                        }
                        else if (type == typeof(DateTimeOffset))
                        {
                            parameterString = ((DateTimeOffset)value).Ticks.ToString();
                        }
                        else if (tc != TypeCode.Object)
                        {
                            parameterString = value.ToString();
                        }
                        else
                        {
                            parameterString = SerializationHelper.Serialize(value);
                            if (!string.IsNullOrEmpty(parameterString)) parameterString = GetHash(parameterString);
                        }

                        break;
                    }
                }

                var paramString = parameterString ?? StrNull;


                var cacheParamKey = string.Join("=", eArg.Key, paramString);
                parameterKeys.Add(cacheParamKey);
            }

            paramBuilder.Append(string.Join("|", parameterKeys));


            return paramBuilder.ToString();
        }

        private string FormHash(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var strLen = str.Length;
            var sbuilder = new StringBuilder(strLen);

            for (var i = 0; i < strLen; i++)
            {
                var ch = str[i];
                if (char.IsWhiteSpace(ch) || ch is '\\' or '/' or '=')
                    sbuilder.Append($@"{ch:x2}");
                else
                    sbuilder.Append(ch);
            }

            return sbuilder.Length != strLen ? sbuilder.ToString() : str;
        }

        private string MD5Hash(string input)
        {
            using var md5 = MD5.Create();
            var retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();
            foreach (var t in retVal) sBuilder.Append(t.ToString("X2"));

            return sBuilder.ToString();
        }

        private string GetHash(string str)
        {
            if (!string.IsNullOrEmpty(str)) str = FormHash(MD5Hash(str));

            return str ?? string.Empty;
        }
    }
}