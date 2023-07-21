namespace DeliverCom.Core.Resolving.Attribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class NamedResolveAttribute : System.Attribute
    {
        public NamedResolveAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}