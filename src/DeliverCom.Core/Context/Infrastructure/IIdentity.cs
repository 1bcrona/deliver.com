namespace DeliverCom.Core.Context.Infrastructure
{
    public interface IIdentity
    {
        #region Public Properties

        string Email { get; }
        string CompanyId { get; }

        #endregion Public Properties
    }
}