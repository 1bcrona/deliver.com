using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Base;
using DeliverCom.Domain.User.ValueObject;

namespace DeliverCom.Domain.User
{
    public class User : BaseEntity<string>
    {
        public virtual Company.Company Company { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public UserRole Role { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        protected override void SetId()
        {
            Id = Guid.NewGuid().ToString("D");
        }

        public static User NewUser(Company.Company company, string firstName, string surname, string password,
            string email,
            UserRole userRole)
        {
            var newUser = new User
            {
                Company = company,
                FirstName = firstName,
                Surname = surname,
                Password = password,
                Email = email,
                Role = userRole
            };
            newUser.SetEntityId();
            newUser.SetId();
            newUser.Activate();
            newUser.EnsureCreationDate();
            newUser.EnsureUpdateDate();
            newUser.Validate();
            return newUser;
        }

        public override void Validate()
        {
            if (Role == UserRole.COMPANY_USR)
                if (Company == null)
                    throw new ArgumentNullException($"{nameof(Company)} is null");

            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentNotValidException($"{nameof(FirstName)} is not valid");

            if (string.IsNullOrWhiteSpace(Surname))
                throw new ArgumentNotValidException($"{nameof(Surname)} is not valid");

            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentNotValidException($"{nameof(Email)} is not valid");
        }
    }
}