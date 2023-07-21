namespace DeliverCom.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UseCaseRouteAttribute : Attribute
    {
        public UseCaseRouteAttribute(string name)
        {
            _name = name;
        }

        public string Name => _name;
        private string _name { get; }
    }
}