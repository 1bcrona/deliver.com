namespace DeliverCom.Core.Operation.Model
{
    public class OperationResult
    {
        public object Result { get; set; }


        public static OperationResult Ok(object result = null)
        {
            var ok = new OperationResult
            {
                Result = result
            };
            return ok;
        }
    }
}