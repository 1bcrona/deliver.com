namespace DeliverCom.Core.Request.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CachingAttribute : System.Attribute
    {
        public CachingAttribute(int durationInSeconds)
        {
            DurationInSeconds = durationInSeconds;
        }

        public int DurationInSeconds { get; }
    }
}