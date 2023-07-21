using DeliverCom.Core.Exception.Impl;
using DeliverCom.Domain.Company;
using DeliverCom.Domain.Context;
using DeliverCom.Domain.User;
using DeliverCom.Domain.User.Error;
using DeliverCom.Domain.User.ValueObject;
using DeliverCom.UseCase.LoginUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace DeliverCom.Test.Unit.UseCase.LoginUser
{
    public class Tests
    {
        private Mock<DeliverComDbContext> _dataContext;

        private static readonly List<User> _fakeUsers = new()
        {
            User.NewUser(Company.NewCompany("CompanyName"),
                "FirstName",
                "Surname",
                "Password",
                "EmailAddress",
                UserRole.SYSTEM_ADM)
        };

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
            _dataContext = dataContext;
        }

        [Test]
        public async Task LoginUserUseCase_Should_Be_Executed()
        {
            var useCase = new LoginUserUseCase(_dataContext.Object);
            var parameters = new LoginUserUseCaseParameters()
            {
                Email = "EmailAddress",
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
                Assert.That(parameters.Email, Is.EqualTo(createdUser.Email));
                Assert.That(parameters.Password, Is.EqualTo(createdUser.Password));
            });
        }

        [Test]
        public Task LoginUserUseCase_Should_Throw_Exception_When_Input_Is_Null()
        {
            var useCase = new LoginUserUseCase(_dataContext.Object);
            useCase.SetInput(null);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task LoginUserUseCase_Should_Throw_Exception_When_Email_Is_Null()
        {
            var useCase = new LoginUserUseCase(_dataContext.Object);
            var parameters = new LoginUserUseCaseParameters()
            {
                Email = null,
                Password = "Password"
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task LoginUserUseCase_Should_Throw_Exception_When_Password_Is_Null()
        {
            var useCase = new LoginUserUseCase(_dataContext.Object);
            var parameters = new LoginUserUseCaseParameters()
            {
                Email = "EmailAddress",
                Password = null
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<ArgumentNotValidException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task LoginUserUseCase_Should_Throw_Exception_When_Email_Is_Not_Found()
        {
            var useCase = new LoginUserUseCase(_dataContext.Object);
            var parameters = new LoginUserUseCaseParameters()
            {
                Email = "EmailAddressNotFound",
                Password = "Password"
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<UserOrPasswordIncorrectException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }

        [Test]
        public Task LoginUserUseCase_Should_Throw_Exception_When_Password_Is_Not_Found()
        {
            var useCase = new LoginUserUseCase(_dataContext.Object);
            var parameters = new LoginUserUseCaseParameters()
            {
                Email = "EmailAddress",
                Password = "PasswordNotFound"
            };
            useCase.SetInput(parameters);
            Assert.ThrowsAsync<UserOrPasswordIncorrectException>(async () => await useCase.Execute());
            return Task.CompletedTask;
        }
    }
}