using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Company;
using DeliverCom.Domain.Context;
using DeliverCom.Domain.User;
using DeliverCom.Domain.User.ValueObject;
using DeliverCom.UseCase.RegisterUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace DeliverCom.Test.Unit.UseCase.RegisterUser
{
    public class Tests
    {
        private Mock<DeliverComDbContext> _dataContext;
        private static readonly List<User> _fakeUsers = new();
        private static readonly List<Company> _fakeCompanies = new() { Company.NewCompany("CompanyName") };

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
            dataContext.Setup(x => x.Users).Returns(_fakeUsers.BuildMock().BuildMockDbSet().Object);
            dataContext.Setup(x => x.Companies).Returns(_fakeCompanies.BuildMock().BuildMockDbSet().Object);
            _dataContext = dataContext;
        }


        [Test]
        public async Task RegisterUserUseCase_Should_Be_Executed()
        {
            var useCase = new RegisterUserUseCase(_dataContext.Object);
            var parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.SYSTEM_ADM,
                FirstName = "FirstName",
                Surname = "Surname",
                EmailAddress = "EmailAddress",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            var result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            var createdUser = result.Result as User;
            Assert.That(createdUser, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(parameters.FirstName, Is.EqualTo(createdUser.FirstName));
                Assert.That(parameters.Surname, Is.EqualTo(createdUser.Surname));
                Assert.That(parameters.EmailAddress, Is.EqualTo(createdUser.Email));
                Assert.That(parameters.Password, Is.EqualTo(createdUser.Password));
                Assert.That(parameters.Role, Is.EqualTo(createdUser.Role));
            });
            var currentCompany = _fakeCompanies[0];
            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.COMPANY_USR,
                CompanyId = currentCompany.EntityId,
                FirstName = "FirstName",
                Surname = "Surname",
                EmailAddress = "EmailAddress",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            createdUser = result.Result as User;
            Assert.That(createdUser, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(parameters.FirstName, Is.EqualTo(createdUser.FirstName));
                Assert.That(parameters.Surname, Is.EqualTo(createdUser.Surname));
                Assert.That(parameters.EmailAddress, Is.EqualTo(createdUser.Email));
                Assert.That(parameters.Password, Is.EqualTo(createdUser.Password));
                Assert.That(parameters.Role, Is.EqualTo(createdUser.Role));
                Assert.That(createdUser.Company, Is.Not.Null);
                Assert.That(createdUser.Company.Id, Is.EqualTo(currentCompany.Id));
                Assert.That(createdUser.Company.EntityId, Is.EqualTo(currentCompany.EntityId));
                Assert.That(createdUser.Company.Name, Is.EqualTo(currentCompany.Name));
            });
        }

        [Test]
        public async Task RegisterUserUseCase_Should_Not_Be_Executed()
        {
            var useCase = new RegisterUserUseCase(_dataContext.Object);
            var parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.SYSTEM_ADM,
                FirstName = "FirstName",
                Surname = "Surname",
                EmailAddress = "EmailAddress",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            var result = await useCase.Execute();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.Not.Null);
            var createdUser = result.Result as User;
            Assert.That(createdUser, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(parameters.FirstName, Is.EqualTo(createdUser.FirstName));
                Assert.That(parameters.Surname, Is.EqualTo(createdUser.Surname));
                Assert.That(parameters.EmailAddress, Is.EqualTo(createdUser.Email));
                Assert.That(parameters.Password, Is.EqualTo(createdUser.Password));
                Assert.That(parameters.Role, Is.EqualTo(createdUser.Role));
            });
            _fakeUsers.Add(createdUser);
            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.SYSTEM_ADM,
                FirstName = "FirstName",
                Surname = "Surname",
                EmailAddress = "EmailAddress",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<AlreadyExistsException>(async () => await useCase.Execute());

            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.COMPANY_USR,
                CompanyId = "CompanyId",
                FirstName = "FirstName",
                Surname = "Surname",
                EmailAddress = "EmailAddress2",
                Password = "Password"
            };

            useCase.SetInput(parameters);
            Assert.ThrowsAsync<NotFoundException>(async () => await useCase.Execute());
        }

        [Test]
        public Task RegisterUserUseCase_Should_Not_Be_Validated()
        {
            var useCase = new RegisterUserUseCase(_dataContext.Object);
            Assert.Throws<ArgumentNotValidException>(() => useCase.Validate());

            var parameters = new RegisterUserUseCaseParameters();
            useCase.SetInput(parameters);
            Assert.Throws<ArgumentNotValidException>(() => useCase.Validate());

            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.COMPANY_USR
            };
            useCase.SetInput(parameters);
            Assert.Throws<ArgumentNotValidException>(() => useCase.Validate());
            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.SYSTEM_ADM,
                FirstName = "",
                Surname = "Surname",
                EmailAddress = "EmailAddress",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            Assert.Throws<ArgumentNotValidException>(() => useCase.Validate());

            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.SYSTEM_ADM,
                FirstName = "FirstName",
                Surname = "",
                EmailAddress = "EmailAddress",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            Assert.Throws<ArgumentNotValidException>(() => useCase.Validate());

            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.SYSTEM_ADM,
                FirstName = "FirstName",
                Surname = "Surname",
                EmailAddress = "",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            Assert.Throws<ArgumentNotValidException>(() => useCase.Validate());

            parameters = new RegisterUserUseCaseParameters()
            {
                Role = UserRole.SYSTEM_ADM,
                FirstName = "FirstName",
                Surname = "Surname",
                EmailAddress = "EmailAddress",
                Password = ""
            };
            useCase.SetInput(parameters);
            Assert.Throws<ArgumentNotValidException>(() => useCase.Validate());
            return Task.CompletedTask;
        }
    }
}