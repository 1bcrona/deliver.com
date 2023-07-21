namespace DeliverCom.Application.Infrastructure
{
    public abstract class BaseRequest
    {
        #region Public Properties

        public string CorrelationId { get; set; }

        #endregion Public Properties
    }
}