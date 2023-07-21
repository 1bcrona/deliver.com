using DeliverCom.Domain.User.ValueObject;

namespace DeliverCom.UseCase.RegisterUser
{
    public class RegisterUserUseCaseParameters
    {
        public string CompanyId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public UserRole Role { get; set; }
        public string Password { get; set; }
    }
}