using DeliverCom.Core.Context.Infrastructure;

namespace DeliverCom.Core.Context.Impl
{
    public sealed class Identity : IIdentity, IEquatable<Identity>
    {
        #region Public Constructors

        public Identity(string email, string companyId)
        {
            Email = email;
            CompanyId = companyId;
        }

        #endregion Public Constructors

        public bool Equals(Identity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CompanyId == other.CompanyId && Email == other.Email;
        }

        public string Email { get; }
        public string CompanyId { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Identity);
        }

        #region Public Properties

        public static Identity Empty => new(null, null);


        public override int GetHashCode()
        {
            return HashCode.Combine(Email, CompanyId);
        }

        #endregion Public Properties
    }
}