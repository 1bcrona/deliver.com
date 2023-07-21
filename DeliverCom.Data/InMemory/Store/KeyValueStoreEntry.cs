namespace DeliverCom.Data.InMemory.Store
{
    public class KeyValueStoreEntry
    {
        #region Public Properties

        public long CreationDate { get; set; }
        public long ExpireDate { get; set; }
        public object Item { get; set; }

        public string Key { get; set; }

        #endregion Public Properties
    }
}