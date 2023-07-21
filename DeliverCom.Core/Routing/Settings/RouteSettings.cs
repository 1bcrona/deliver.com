namespace DeliverCom.Core.Routing.Settings
{
    public class RouteSettings
    {
        public List<RouteSetting> Routes { get; set; }
    }

    public class RouteSetting
    {
        public string Route { get; set; }
        public string Path { get; set; }
    }
}