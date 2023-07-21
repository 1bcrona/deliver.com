namespace DeliverCom.API.Model
{
    public class ApiErrorResponse
    {
        /// <summary>
        ///     Type of Error
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        ///     Error Message
        /// </summary>
        public string Message { get; set; }
    }
}