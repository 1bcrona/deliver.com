using DeliverCom.Core.Query.Infrastructure;
using DeliverCom.Domain.Delivery;

namespace DeliverCom.Domain.Query.Delivery
{
    public class DeliveryQueryContext : IQueryContext
    {
        public string CompanyId { get; set; }
        public string SenderCity { get; set; }
        public string SenderTown { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryTown { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryId { get; set; }
        public string DeliveryNumber { get; set; }

        public PaginationContext PaginationContext { get; set; }

        public void FillQueryContext(Dictionary<string, string> args)
        {
            /* Fill this context from args with corresponding keys */
            if (args.TryGetValue("CompanyId", out var arg))
                CompanyId = arg;

            if (args.TryGetValue("SenderCity", out arg))
                SenderCity = arg;

            if (args.TryGetValue("SenderTown", out arg))
                SenderTown = arg;

            if (args.TryGetValue("DeliveryCity", out arg))
                DeliveryCity = arg;

            if (args.TryGetValue("DeliveryTown", out arg))
                DeliveryTown = arg;

            if (args.TryGetValue("DeliveryStatus", out arg))
                DeliveryStatus = (DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), arg);

            if (args.TryGetValue("DeliveryId", out arg))
                DeliveryId = arg;

            if (args.TryGetValue("DeliveryNumber", out arg))
                DeliveryNumber = arg;
        }
    }
}