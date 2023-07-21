using System.Data;
using DeliverCom.Core.Context.Impl;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Company;
using DeliverCom.Domain.Delivery;
using DeliverCom.Repository.Context;
using DeliverCom.UseCase.CreateDelivery;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.SqlServer.Diagnostics;
using DotNetCore.CAP.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace DeliverCom.Test.UseCase.CreateDelivery
{
    public class Tests
    {
        private Mock<DeliverComDbContext> _dataContext;
        private Mock<AmbientExecutionContext> _context;
        private Mock<ICapPublisher> _mockCapPublisher;
        private static readonly Company _mockCompany = Company.NewCompany("CompanyId1");
        private static readonly List<Delivery> _fakeDeliveries = new();
        private static readonly List<Company> _fakeCompanies = new() { _mockCompany };

        [OneTimeSetUp]
        public void Init()
        {
            _context = new Mock<AmbientExecutionContext>();
            _context.Object.SetIdentity(new Identity("UserId", _mockCompany.EntityId));

            var mockCapPublisher = new Mock<ICapPublisher>();
            var mockCapTransaction = new Mock<ICapTransaction>();
            mockCapTransaction.SetupProperty(x => x.AutoCommit, false);
            mockCapTransaction.SetupProperty(x => x.DbTransaction, Mock.Of<IDbTransaction>());
            mockCapPublisher.Setup(x => x.Transaction).Returns(new AsyncLocal<ICapTransaction>
            {
                Value = mockCapTransaction.Object
            });

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(ICapPublisher))).Returns(mockCapPublisher.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IDispatcher))).Returns(new Mock<IDispatcher>().Object);
            serviceProvider.Setup(x => x.GetService(typeof(IConsumerServiceSelector)))
                .Returns(new Mock<IConsumerServiceSelector>().Object);
            serviceProvider.Setup(x => x.GetService(typeof(ITransport))).Returns(new Mock<ITransport>().Object);
            serviceProvider.Setup(x => x.GetService(typeof(DiagnosticProcessorObserver)))
                .Returns(new Mock<DiagnosticProcessorObserver>(new Mock<IDispatcher>().Object).Object);

            mockCapPublisher.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            var options = new DbContextOptionsBuilder<DeliverComDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .UseLazyLoadingProxies()
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dataContext = new Mock<DeliverComDbContext>(options);
            var facade = new DatabaseFacade(dataContext.Object);
            dataContext.SetupGet(a => a.Database).Returns(facade);
            dataContext.Setup(x => x.Deliveries).Returns(_fakeDeliveries.BuildMock().BuildMockDbSet().Object);
            dataContext.Setup(x => x.Companies).Returns(_fakeCompanies.BuildMock().BuildMockDbSet().Object);
            _mockCapPublisher = mockCapPublisher;
            _dataContext = dataContext;
        }


        [Test]
        public Task CreateDeliveryUseCase_Should_Not_Be_Executed_When_AmbientContext_Is_Null()
        {
            var useCase =
                new CreateDeliveryUseCase(_dataContext.Object, _mockCapPublisher.Object);
            var parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_AmbientContext_Identity_Is_Null()
        {
            var useCase =
                new CreateDeliveryUseCase(_dataContext.Object, _mockCapPublisher.Object);
            var parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            var ctx = new AmbientExecutionContext();
            ctx.SetIdentity(null);
            useCase.SetExecutionContext(ctx);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_AmbientContext_Identity_Is_Empty()
        {
            var useCase =
                new CreateDeliveryUseCase(_dataContext.Object, _mockCapPublisher.Object);


            var parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            var ctx = new AmbientExecutionContext();
            useCase.SetExecutionContext(ctx);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_Validation_Failed()
        {
            var useCase =
                new CreateDeliveryUseCase(_dataContext.Object, _mockCapPublisher.Object);

            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            
            var parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = null,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = string.Empty,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = " ",
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = null,
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = string.Empty,
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = " ",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = null,
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };
            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = string.Empty,
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = " ",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = null,
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = string.Empty,
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = " ",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = null,
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = string.Empty,
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = " ",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = null,
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = string.Empty,
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = " ",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = null,
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = string.Empty,
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = " ",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = null,
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = string.Empty,
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = " ",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = null
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = string.Empty
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C1",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = " "
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());

            parameters = new CreateDeliveryUseCaseParameters()
            {
                CompanyId = null,
                SenderAddressCity = null,
                SenderAddressStreet = null,
                SenderAddressTown = null,
                SenderAddressZipCode = null,
                DeliveryAddressCity = null,
                DeliveryAddressStreet = null,
                DeliveryAddressTown = null,
                DeliveryAddressZipCode = null
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());


            return Task.CompletedTask;
        }


        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_CompanyId_Different_From_Identity()
        {
            var useCase =
                new CreateDeliveryUseCase(_dataContext.Object, _mockCapPublisher.Object);
            var parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = "CompanyId2",
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            Assert.ThrowsAsync<AuthenticationException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task GetDeliveriesUseCase_Should_Not_Be_Executed_When_Company_NotFound()
        {
            var useCase =
                new CreateDeliveryUseCase(_dataContext.Object, _mockCapPublisher.Object);
            var parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = "CompanyId2",
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };


            useCase.SetInput(parameters);
            var ctx = new AmbientExecutionContext();
            ctx.SetIdentity(new Identity("email", "CompanyId2"));
            useCase.SetExecutionContext(ctx);
            Assert.ThrowsAsync<NotFoundException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }


        [Test]
        public async Task CreateDeliveryUseCase_Should_Be_Executed()
        {
            var useCase =
                new CreateDeliveryUseCase(_dataContext.Object, _mockCapPublisher.Object);
            var parameters = new CreateDeliveryUseCaseParameters
            {
                CompanyId = _mockCompany.EntityId,
                SenderAddressCity = "C1",
                SenderAddressStreet = "S1",
                SenderAddressTown = "T1",
                SenderAddressZipCode = "Z1",
                DeliveryAddressCity = "C2",
                DeliveryAddressStreet = "S2",
                DeliveryAddressTown = "T2",
                DeliveryAddressZipCode = "Z2"
            };

            useCase.SetInput(parameters);
            useCase.SetExecutionContext(_context.Object);
            var result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            var createdDelivery = result.Result as Delivery;
            Assert.That(createdDelivery, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(parameters.SenderAddressCity, Is.EqualTo(createdDelivery.SenderAddress.City));
                Assert.That(parameters.SenderAddressStreet, Is.EqualTo(createdDelivery.SenderAddress.Street));
                Assert.That(parameters.SenderAddressTown, Is.EqualTo(createdDelivery.SenderAddress.Town));
                Assert.That(parameters.SenderAddressZipCode, Is.EqualTo(createdDelivery.SenderAddress.ZipCode));
                Assert.That(parameters.DeliveryAddressCity, Is.EqualTo(createdDelivery.DeliveryAddress.City));
                Assert.That(parameters.DeliveryAddressStreet, Is.EqualTo(createdDelivery.DeliveryAddress.Street));
                Assert.That(parameters.DeliveryAddressTown, Is.EqualTo(createdDelivery.DeliveryAddress.Town));
                Assert.That(parameters.DeliveryAddressZipCode, Is.EqualTo(createdDelivery.DeliveryAddress.ZipCode));
            });
        }
    }
}