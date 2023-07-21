using System.Data;

namespace DeliverCom.Core.Request.Model
{
    public class RequestParameter
    {
        public string ParameterName { get; set; }
        public Type ParameterType { get; set; }
        public string ParameterValueType { get; set; }
        public ParameterDirection ParameterDirection { get; set; }
    }
}