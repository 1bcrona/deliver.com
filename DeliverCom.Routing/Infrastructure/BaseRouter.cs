using System.Reflection;
using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Resolving.Infrastructure;
using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.Core.UseCase.Infrastructure;

namespace DeliverCom.Routing.Infrastructure
{
    public abstract class BaseRouter : IRouter
    {
        protected readonly IExecutionContext _context;
        private readonly IResolver _resolver;

        protected BaseRouter(IResolver resolver, IExecutionContext context)
        {
            _resolver = resolver;
            _context = context;
        }


        public abstract IUseCase Route(string path, object parameters);

        public abstract IUseCase Route(string path);

        public IUseCase Route<T, TP>(string path, TP parameters) where T : class, IUseCase<TP>
        {
            return Route(path, parameters);
        }

        public void FillParameters(IUseCase useCase, object parameter)
        {
            if (parameter == null) return;

            useCase.SetExecutionContext(_context);
            var useCaseType = useCase.GetType();

            var isGenericType = useCaseType.GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IUseCase<>));

            if (!isGenericType)
                return;

            var methodInfo = useCaseType.GetMethod("SetInput",
                BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance);

            if (methodInfo == null) return;
            var parameterOfMethods = methodInfo.GetParameters();
            var parameterCount = parameterOfMethods.Length;

            var inputParameter = parameterOfMethods.FirstOrDefault(w => w.Name == "input");

            if (inputParameter == null) throw new NotFoundException("INPUT_PARAMETER_NOT_FOUND");

            var inputParameterType = inputParameter.ParameterType;
            var inputParameterPosition = inputParameter.Position;

            var arguments = new object[parameterCount];


            var input = ResolveParameter(parameter, inputParameterType);
            arguments[inputParameterPosition] = input;
            methodInfo.Invoke(useCase, arguments);
        }

        private object ResolveParameter(object parameter, Type inputParameterType)
        {
            return _resolver.Resolve(parameter, inputParameterType, true);
        }
    }
}