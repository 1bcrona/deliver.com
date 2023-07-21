namespace DeliverCom.API.Model
{
    public class ApiResponse<TData>
    {
        /// <summary>
        ///     Response's Data.
        /// </summary>
        public TData Data { get; set; }
    }
}