using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace DeliverCom.Test.Integration.Model
{
    public class IndexedProperty
    {
        private string[] strings =
        {
            "abc", "def", "ghi", "jkl"
        };

        public string this[int Index]
        {
            get => strings[Index];
            set => strings[Index] = value;
        }
    }

    public struct StructType
    {
        public int X { get; set; }
        public int Y { get; set; }

        public readonly string InitField = "Readonly field";

        public int? NullableInt { get; set; }

        public int IntField;
        public int[] IntArray { get; set; }

        public IndexedProperty IndexedProperty { get; set; }

        [IgnoreDataMember] public string StringField;

        [CompilerGenerated] public string CompileGeneratedField;

        public string NotReadPublicProperty
        {
            set => PrivateProperty = value;
        }

        public Guid GuidField;
        public TimeSpan TimeSpanField;
        public DateTime DateTimeField;
        public DateTimeOffset DateTimeOffsetField;
        public decimal DecimalField;
        private string PrivateProperty;

        public string ReadPublicProperty => PrivateProperty;

        public StructType(int x, int y)
        {
            X = x;
            Y = y;
            NullableInt = null;
            IntArray = Array.Empty<int>();
            IntField = 0;
            StringField = "StringField";
            IndexedProperty = new IndexedProperty();
            CompileGeneratedField = "CompilerGeneratedField";
            PrivateProperty = "PrivateProperty";
            GuidField = Guid.NewGuid();
            TimeSpanField = TimeSpan.FromSeconds(1);
            DateTimeField = DateTime.Now;
            DateTimeOffsetField = DateTimeOffset.Now;
            DecimalField = 1.23456789m;
        }
    }

    public class Company
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
    }

    public enum OrderStatus
    {
        New = 0,
        Processing = 1,
        Processed = 2
    }

    public class Order
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public decimal? OrderAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Company Company { get; set; }
        public List<Company> Companies { get; set; } = new();
        public StructType StructType { get; set; }

        public Guid GuidField;
        public TimeSpan TimeSpanField;
        public DateTime DateTimeField;
        public DateTimeOffset DateTimeOffsetField;
        public decimal DecimalProperty => 1.23456789m;


        public StructType StructTypeNotWrite => this.StructType;

        public Dictionary<string, object> Dynamic { get; set; } = new();
        public string Id { get; set; }
    }

    public class OrderEqualityComparer : IEqualityComparer<Order>
    {
        public bool Equals([AllowNull] Order x, [AllowNull] Order y)
        {
            if ((x == null) ^ (y == null)) return false;

            if (x == null) return true;


            return x.Id == y.Id
                   && Math.Abs(x.CreateDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds -
                               y.CreateDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds) < 600
                   && x.OrderAmount == y.OrderAmount;
        }

        public int GetHashCode([DisallowNull] Order obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class OrderTestData
    {
        static OrderTestData()
        {
            CreateOrders();
        }

        public static List<Order> RandomOrders { get; private set; }

        public static Order FirstOrder => RandomOrders[0];


        public static Order RandomOrder
        {
            get
            {
                if (RandomOrders.Count == 0) CreateOrders();

                var r = new Random();

                var id = r.Next(0, RandomOrders.Count - 1);
                return RandomOrders[id];
            }
        }

        private static void CreateOrders()
        {
            var r = new Random();
            var result = new List<Order>();
            for (var i = 0; i < 500; i++)
            {
                var order = new Order
                {
                    Id = i.ToString(),
                    OrderAmount = i
                };
                if (i % 100 == 1)
                {
                    order.OrderAmount = null;
                    order.OrderStatus = OrderStatus.Processed;
                }
                else
                {
                    order.OrderStatus = (OrderStatus)r.Next(0, 2);
                }

                var companyCount = i % 10 + 1;

                for (var j = 0; j < companyCount; j++)
                {
                    var companyId = "C_" + j;
                    var companyName = "CN_" + companyId;
                    order.Companies.Add(new Company
                    {
                        CompanyId = companyId,
                        CompanyName = companyName
                    });
                }


                order.CreateDate = DateTime.UtcNow;
                order.Company = order.Companies.FirstOrDefault();

                order.StructType = new StructType
                {
                    X = r.Next(0, 42),
                    Y = r.Next(0, 26)
                };


                order.Dynamic["id"] = order.Id;
                order.Dynamic["status"] = order.OrderStatus;
                order.Dynamic["company"] = order.Companies;
                result.Add(order);
            }

            RandomOrders = result;
        }
    }
}