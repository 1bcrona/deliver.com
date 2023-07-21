using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Operation.Model;

namespace DeliverCom.Core.UseCase.Infrastructure
{
    public abstract class BaseUseCase<T> : IUseCase<T>
    {
        public IExecutionContext AmbientExecutionContext { get; private set; }

        public abstract void Validate();

        public virtual Task<OperationResult> Execute()
        {
            Validate();
            return ExecuteInternal();
        }

        public void SetExecutionContext(IExecutionContext context)
        {
            AmbientExecutionContext = context;
        }

        public T Input { get; private set; }

        public void SetInput(T input)
        {
            Input = input;
        }

        protected abstract Task<OperationResult> ExecuteInternal();
    }
}