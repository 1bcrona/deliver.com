using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.UseCase.Infrastructure;
using DeliverCom.Domain.User;
using DeliverCom.Domain.User.ValueObject;
using DeliverCom.Repository.Context;

namespace DeliverCom.UseCase.RegisterUser
{
    public class RegisterUserUseCase : BaseUseCase<RegisterUserUseCaseParameters>
    {
        private readonly DeliverComDbContext _dataContext;


        public RegisterUserUseCase(DeliverComDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override void Validate()
        {
            if (Input == null) throw new ArgumentNotValidException($"{nameof(Input)} is null");

            if (Input.Role == UserRole.COMPANY_USR)
                if (string.IsNullOrWhiteSpace(Input.CompanyId))
                    throw new ArgumentNotValidException($"{nameof(Input.CompanyId)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.FirstName))
                throw new ArgumentNotValidException($"{nameof(Input.FirstName)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.Surname))
                throw new ArgumentNotValidException($"{nameof(Input.Surname)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.EmailAddress))
                throw new ArgumentNotValidException($"{nameof(Input.EmailAddress)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.Password))
                throw new ArgumentNotValidException($"{nameof(Input.Password)} is not valid");
        }

        protected override async Task<OperationResult> ExecuteInternal()
        {
            // Check if email is already used
            var userWithEmail = _dataContext.Users.FirstOrDefault(x => x.Email == Input.EmailAddress);
            if (userWithEmail != null)
                throw new AlreadyExistsException($"{nameof(Input.EmailAddress)} is already used");

            var company = _dataContext.Companies.FirstOrDefault(w => w.EntityId == Input.CompanyId);
            if (company == null)
                if (Input.Role == UserRole.COMPANY_USR)
                    throw new NotFoundException("Company not found");

            var user = User.NewUser(company,
                Input.FirstName,
                Input.Surname,
                Input.Password,
                Input.EmailAddress,
                Input.Role);
            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            return OperationResult.Ok(user);
        }
    }
}