using DeliverCom.Domain.User.ValueObject;

namespace DeliverCom.API.Model.LoginUser
{
    public class LoginUserResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string CompanyId { get; set; }
        public UserRole Role { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
    }
}