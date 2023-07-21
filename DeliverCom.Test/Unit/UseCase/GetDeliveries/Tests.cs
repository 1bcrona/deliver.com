using DeliverCom.Core.Context.Impl;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Company;
using DeliverCom.Domain.Delivery;
using DeliverCom.Domain.Delivery.Contracts;
using DeliverCom.Domain.Delivery.ValueObject;
using DeliverCom.Repository.Context;
using DeliverCom.Repository.Query;
using DeliverCom.Repository.Query.Delivery;
using DeliverCom.UseCase.GetDeliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace DeliverCom.Test.Unit.UseCase.GetDeliveries
{
    public class Tests
    {
        private Mock<DeliverComDbContext> _dataContext;
        private Mock<AmbientExecutionContext> _context;

        private static Company _mockCompany = Company.NewCompany("CompanyId1");

        private static readonly List<Delivery> _fakeDeliveries = new()
        {
            Delivery.NewDelivery(
                new Address("S1", "T1", "C1", "Z1"),
                new Address("S2", "T2", "C2", "Z2"),
                _mockCompany),
            Delivery.NewDelivery(
                new Address("2S1", "2T1", "2C1", "2Z1"),
                new Address("2S2", "2T2", "2C2", "2Z2"),
                _mockCompany),
            Delivery.NewDelivery(
                new Address("S1", "T1", "C1", "Z1"),
                new Address("S2", "T2", "C2", "Z2"),
                Company.NewCompany("CompanyId2"))
        };

        [OneTimeSetUp]
        public void Init()
        {
            _context = new Mock<AmbientExecutionContext>();
            _context.Object.SetIdentity(new Identity("UserId", _mockCompany.EntityId));
            var options = new DbContextOptionsBuilder<DeliverComDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .UseLazyLoadingProxies()
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dataContext = new Mock<DeliverComDbContext>(options);
            var facade = new DatabaseFacade(dataContext.Object);
            dataContext.SetupGet(a => a.Database).Returns(facade);
            dataContext.Setup(x => x.Deliveries).Returns(_fakeDeliveries.BuildMock().BuildMockDbSet().Object);
            _dataContext = dataContext;
        }


        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_AmbientContext_Is_Null()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = _mockCompany.EntityId
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_AmbientContext_Identity_Is_Null()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = _mockCompany.EntityId
            };
            var ctx = new AmbientExecutionContext();
            ctx.SetIdentity(null);
            useCase.SetExecutionContext(ctx);
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_AmbientContext_Identity_Is_Empty()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = _mockCompany.EntityId
            };
            var ctx = new AmbientExecutionContext();
            useCase.SetExecutionContext(ctx);
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_CompanyId_Different_From_Identity()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = "CompanyId2"
            };
            useCase.SetExecutionContext(_context.Object);
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }


        [Test]
        public async Task GetDeliveriesUseCase_Should_Be_Executed_With_Pagination()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = _mockCompany.EntityId,
                PaginationContext = new PaginationContext()
                {
                    PageNumber = 1,
                    PageSize = 1
                }
            };
            useCase.SetExecutionContext(_context.Object);
            useCase.SetInput(parameters);
            var result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            var deliveries = result.Result as PaginatedDeliveriesContract;
            Assert.That(deliveries, Is.Not.Null);
            Assert.That(deliveries.Deliveries, Is.Not.Null);
            Assert.That(deliveries.Deliveries.Length, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(deliveries.Deliveries[0].Company.EntityId, Is.EqualTo(parameters.CompanyId));
                Assert.That(deliveries.PageNumber, Is.EqualTo(1));
                Assert.That(deliveries.PageSize, Is.EqualTo(1));
                Assert.That(deliveries.TotalCount, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task GetDeliveriesUseCase_Should_Be_Executed_With_All_Filters()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = _mockCompany.EntityId,
                PaginationContext = new PaginationContext()
                {
                    PageNumber = 1,
                    PageSize = 1
                },
                SenderCity = "C1",
                SenderTown = "T1",
                DeliveryCity = "C2",
                DeliveryTown = "T2",
                DeliveryId = _fakeDeliveries[0].EntityId,
                DeliveryNumber = _fakeDeliveries[0].DeliveryNumber,
                DeliveryStatus = DeliveryStatus.NotShipped
            };
            useCase.SetExecutionContext(_context.Object);
            useCase.SetInput(parameters);
            var result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            var deliveries = result.Result as PaginatedDeliveriesContract;
            Assert.That(deliveries, Is.Not.Null);
            Assert.That(deliveries.Deliveries, Is.Not.Null);
            Assert.That(deliveries.Deliveries.Length, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(deliveries.Deliveries[0].Company.EntityId, Is.EqualTo(parameters.CompanyId));
                Assert.That(deliveries.PageNumber, Is.EqualTo(1));
                Assert.That(deliveries.PageSize, Is.EqualTo(1));
                Assert.That(deliveries.TotalCount, Is.EqualTo(1));
            });
        }


        [Test]
        public async Task GetDeliveriesUseCase_Should_Be_Executed()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = _mockCompany.EntityId
            };
            useCase.SetExecutionContext(_context.Object);
            useCase.SetInput(parameters);
            var result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            var deliveries = result.Result as PaginatedDeliveriesContract;
            Assert.That(deliveries, Is.Not.Null);
            Assert.That(deliveries.Deliveries, Is.Not.Null);
            Assert.That(deliveries.Deliveries.Length, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(deliveries.Deliveries[0].Company.EntityId, Is.EqualTo(parameters.CompanyId));
                Assert.That(deliveries.PageNumber, Is.EqualTo(1));
                Assert.That(deliveries.PageSize, Is.EqualTo(2));
                Assert.That(deliveries.TotalCount, Is.EqualTo(2));
            });
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Throw_Exception_When_Input_Is_Null()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            useCase.SetInput(null);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }


        [Test]
        public Task GetDeliveriesUseCase_Should_Throw_Exception_When_CompanyId_Is_Empty()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = string.Empty
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Throw_Exception_When_CompanyId_Is_Null()
        {
            var useCase = new GetDeliveriesUseCase(_dataContext.Object);
            var parameters = new DeliveryQueryContext()
            {
                CompanyId = null
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }
    }
}