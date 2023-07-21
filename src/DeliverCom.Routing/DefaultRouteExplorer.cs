using DeliverCom.Core.Routing.Infrastructure;
using DeliverCom.Core.Routing.Settings;
using Microsoft.Extensions.Options;

namespace DeliverCom.Routing
{
    public class DefaultRouteExplorer : IRoutingExplorer, IDisposable
    {
        private readonly IOptionsMonitor<RouteSettings> _settings;
        private IDictionary<string, string> _useCases = new Dictionary<string, string>();

        public DefaultRouteExplorer(IOptionsMonitor<RouteSettings> settings)
        {
            _settings = settings;
            Explore(settings.CurrentValue);
            _settings.OnChange(Explore);
        }

        public IDictionary<string, string> UseCases => _useCases;

        public void Dispose()
        {
            Dispose(true);
        }

        public string GetRoute(string path)
        {
            _useCases.TryGetValue(path, out var useCase);
            return useCase;
        }

        public void Explore(RouteSettings settings)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var route in settings.Routes) dictionary[route.Path] = route.Route;

            Interlocked.Exchange(ref _useCases, dictionary);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            var existingDictionary = Interlocked.Exchange(ref _useCases, null);
            existingDictionary.Clear();
        }
    }
}