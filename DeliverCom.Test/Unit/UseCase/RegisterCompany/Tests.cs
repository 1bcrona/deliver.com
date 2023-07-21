using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Company;
using DeliverCom.Repository.Context;
using DeliverCom.UseCase.RegisterCompany;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace DeliverCom.Test.Unit.UseCase.RegisterCompany
{
    public class Tests
    {
        private Mock<DeliverComDbContext> _dataContext;
        private static readonly List<Company> _fakeCompanies = new();

        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DeliverComDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .UseLazyLoadingProxies()
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dataContext = new Mock<DeliverComDbContext>(options);
            var facade = new DatabaseFacade(dataContext.Object);
            dataContext.SetupGet(a => a.Database).Returns(facade);
            dataContext.Setup(x => x.Companies).Returns(_fakeCompanies.BuildMock().BuildMockDbSet().Object);
            _dataContext = dataContext;
        }

        [Test]
        public async Task RegisterCompanyUseCase_Should_Be_Executed()
        {
            var useCase = new RegisterCompanyUseCase(_dataContext.Object);
            var parameters = new RegisterCompanyUseCaseParameters()
            {
                Name = "CompanyName"
            };
            useCase.SetInput(parameters);
            var result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            var createdCompany = result.Result as Company;
            Assert.That(createdCompany, Is.Not.Null);
            Assert.Multiple(() => { Assert.That(parameters.Name, Is.EqualTo(createdCompany.Name)); });
        }

        [Test]
        public Task RegisterCompanyUseCase_Should_Throw_Exception_When_Name_Is_Null()
        {
            var useCase = new RegisterCompanyUseCase(_dataContext.Object);
            var parameters = new RegisterCompanyUseCaseParameters()
            {
                Name = null
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task RegisterCompanyUseCase_Should_Throw_Exception_When_Name_Is_Empty()
        {
            var useCase = new RegisterCompanyUseCase(_dataContext.Object);
            var parameters = new RegisterCompanyUseCaseParameters()
            {
                Name = string.Empty
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task RegisterCompanyUseCase_Should_Throw_Exception_When_Name_Is_WhiteSpace()
        {
            var useCase = new RegisterCompanyUseCase(_dataContext.Object);
            var parameters = new RegisterCompanyUseCaseParameters()
            {
                Name = " "
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task RegisterCompanyUseCase_Should_Throw_Exception_When_Name_Is_WhiteSpace2()
        {
            var useCase = new RegisterCompanyUseCase(_dataContext.Object);
            useCase.SetInput(null);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }
    }
}