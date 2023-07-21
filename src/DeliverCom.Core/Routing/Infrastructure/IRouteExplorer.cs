using DeliverCom.Core.Routing.Settings;

namespace DeliverCom.Core.Routing.Infrastructure
{
    public interface IRoutingExplorer
    {
        void Explore(RouteSettings settings);
        string GetRoute(string path);
    }
}