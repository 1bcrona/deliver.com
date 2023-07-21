using System.ComponentModel;

namespace DeliverCom.Domain.User.ValueObject
{
    public enum UserRole
    {
        [Description("System Admin")] SYSTEM_ADM = 1,
        [Description("Company User")] COMPANY_USR = 2
    }
}