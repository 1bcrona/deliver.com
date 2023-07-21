using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Operation.Model;
using DeliverCom.Core.UseCase.Infrastructure;
using DeliverCom.Domain.Delivery;
using DeliverCom.Domain.Delivery.ValueObject;
using DeliverCom.Repository.Context;
using DotNetCore.CAP;

namespace DeliverCom.UseCase.CreateDelivery
{
    public class CreateDeliveryUseCase : BaseUseCase<CreateDeliveryUseCaseParameters>
    {
        private readonly DeliverComDbContext _dataContext;
        private readonly ICapPublisher _capPublisher;

        public CreateDeliveryUseCase(DeliverComDbContext dataContext, ICapPublisher capPublisher)
        {
            _dataContext = dataContext;
            _capPublisher = capPublisher;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException($"{nameof(Input)} is null");

            if (string.IsNullOrWhiteSpace(Input.CompanyId))
                throw new ArgumentNotValidException($"{nameof(Input.CompanyId)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.SenderAddressCity))
                throw new ArgumentNotValidException($"{nameof(Input.SenderAddressCity)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.SenderAddressStreet))
                throw new ArgumentNotValidException($"{nameof(Input.SenderAddressStreet)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.SenderAddressTown))
                throw new ArgumentNotValidException($"{nameof(Input.SenderAddressTown)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.SenderAddressZipCode))
                throw new ArgumentNotValidException($"{nameof(Input.SenderAddressZipCode)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.DeliveryAddressCity))
                throw new ArgumentNotValidException($"{nameof(Input.DeliveryAddressCity)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.DeliveryAddressStreet))
                throw new ArgumentNotValidException($"{nameof(Input.DeliveryAddressStreet)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.DeliveryAddressTown))
                throw new ArgumentNotValidException($"{nameof(Input.DeliveryAddressTown)} is not valid");

            if (string.IsNullOrWhiteSpace(Input.DeliveryAddressZipCode))
                throw new ArgumentNotValidException($"{nameof(Input.DeliveryAddressZipCode)} is not valid");
        }

        protected override async Task<OperationResult> ExecuteInternal()
        {
            if (AmbientExecutionContext == null)
                throw new AuthenticationException("Authentication failed");
            if (AmbientExecutionContext.Identity == null)
                throw new AuthenticationException("Authentication failed");
            if (AmbientExecutionContext.IsEmpty)
                throw new AuthenticationException("Authentication failed");
            if (AmbientExecutionContext.Identity.CompanyId != Input.CompanyId)
                throw new AuthenticationException("Authentication failed");

            var company = _dataContext.Companies.FirstOrDefault(x => x.EntityId == Input.CompanyId);

            if (company == null)
                throw new NotFoundException("Company not found");

            var delivery = Delivery.NewDelivery(
                new Address(Input.SenderAddressStreet, Input.SenderAddressTown,
                    Input.SenderAddressCity, Input.SenderAddressZipCode),
                new Address(Input.DeliveryAddressStreet, Input.DeliveryAddressTown,
                    Input.DeliveryAddressCity, Input.DeliveryAddressZipCode),
                company);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync(_capPublisher);
            try
            {
                await _dataContext.Deliveries.AddAsync(delivery);
                await _capPublisher.PublishAsync("delivercom.delivery.created", delivery);
                await _dataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }


            return OperationResult.Ok(delivery);
        }
    }
}