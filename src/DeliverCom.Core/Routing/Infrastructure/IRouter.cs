using DeliverCom.Core.UseCase.Infrastructure;

namespace DeliverCom.Core.Routing.Infrastructure
{
    public interface IRouter
    {
        IUseCase Route(string path);
        void FillParameters(IUseCase useCase, object parameters);
        IUseCase Route(string path, object parameters);
        IUseCase Route<T, TP>(string path, TP parameters) where T : class, IUseCase<TP>;
    }
}