using DeliverCom.Core.Context.Infrastructure;
using DeliverCom.Core.Operation.Model;

namespace DeliverCom.Core.UseCase.Infrastructure
{
    public interface IUseCase
    {
        void Validate();
        Task<OperationResult> Execute();
        void SetExecutionContext(IExecutionContext context);
    }

    public interface IUseCase<TIn> : IUseCase
    {
        TIn Input { get; }
        void SetInput(TIn input);
    }
}