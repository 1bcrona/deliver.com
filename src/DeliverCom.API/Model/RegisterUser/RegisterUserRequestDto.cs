using System.ComponentModel;
using DeliverCom.Domain.User.ValueObject;

namespace DeliverCom.API.Model.RegisterUser
{
    public class RegisterUserRequestDto
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }

        public string Password { get; set; }

        public string CompanyId { get; set; }
        public UserRole Role { get; set; }
    }

    public class EncryptedStringConverter : TypeConverter
    {
    }
}