using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.UseCase.Infrastructure;
using DeliverCom.Domain.Company;
using DeliverCom.Repository.Context;

namespace DeliverCom.UseCase.RegisterCompany
{
    public class RegisterCompanyUseCase : BaseUseCase<RegisterCompanyUseCaseParameters>
    {
        private readonly DeliverComDbContext _dataContext;

        public RegisterCompanyUseCase(DeliverComDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException($"{nameof(Input)} is null");

            if (string.IsNullOrWhiteSpace(Input.Name))
                throw new ArgumentNotValidException($"{nameof(Input.Name)} is not valid");
        }

        protected override async Task<OperationResult> ExecuteInternal()
        {
            var company = Company.NewCompany(Input.Name);
            _dataContext.Companies.Add(company);
            await _dataContext.SaveChangesAsync();
            return OperationResult.Ok(company);
        }
    }
}