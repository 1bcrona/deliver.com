using DeliverCom.Core.Query.Infrastructure;

namespace DeliverCom.Domain.Query
{
    public class PaginationContext : IQueryContext
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public void FillQueryContext(Dictionary<string, string> args)
        {
            if (args.TryGetValue("PageNumber", out var arg))
                PageNumber = int.Parse(arg);

            if (args.TryGetValue("PageSize", out arg))
                PageSize = int.Parse(arg);
        }
    }
}