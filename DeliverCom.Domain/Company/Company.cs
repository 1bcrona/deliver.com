using System.Text.Json.Serialization;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Base;

namespace DeliverCom.Domain.Company
{
    public class Company : BaseEntity<string>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<User.User> Users { get; protected set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual ICollection<Delivery.Delivery> Deliveries { get; protected set; }

        public string Name { get; set; }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentNotValidException($"{nameof(Name)} is not valid");
        }

        protected override void SetId()
        {
            Id = Guid.NewGuid().ToString("D");
        }

        public static Company NewCompany(string name)
        {
            var newCompany = new Company
            {
                Name = name
            };
            newCompany.SetEntityId();
            newCompany.SetId();
            newCompany.Activate();
            newCompany.EnsureCreationDate();
            newCompany.EnsureUpdateDate();
            newCompany.Validate();
            return newCompany;
        }
    }
}