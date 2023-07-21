namespace DeliverCom.Domain.Base
{
    public abstract class BaseEntity<T>
    {
        public string EntityId { get; protected set; }

        public long CreateDate { get; protected set; }

        public long UpdateDate { get; protected set; }

        public bool IsActive { get; protected set; }

        public T Id { get; protected set; }

        public void Deactivate()
        {
            IsActive = false;
        }

        protected void SetEntityId()
        {
            EntityId = Guid.NewGuid().ToString("D").ToLowerInvariant();
        }

        protected void EnsureCreationDate()
        {
            var currentDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SetCreateDate(currentDate);
        }

        protected void EnsureUpdateDate()
        {
            var currentDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SetUpdateDate(currentDate);
        }

        internal void SetCreateDate(long createDate)
        {
            CreateDate = createDate;
        }


        internal void SetUpdateDate(long updateDate)
        {
            UpdateDate = updateDate;
        }

        protected void Activate()
        {
            IsActive = true;
        }

        protected abstract void SetId();
        public abstract void Validate();
    }
}