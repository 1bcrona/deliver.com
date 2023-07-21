using DeliverCom.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;

namespace DeliverCom.Test.Unit.Context
{
    public class Tests
    {
        [Test]
        public void InMemoryDbContext_Should_Be_Created()
        {
            var context = new DeliverComDbContext();
            Assert.That(context, Is.Not.Null);
        }

        [Test]
        public void InMemoryDbContext_Should_Be_Created_With_Options()
        {
            var options = new DbContextOptionsBuilder<DeliverComDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .UseLazyLoadingProxies()
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DeliverComDbContext(options);
            Assert.Multiple(() =>
            {
                Assert.That(context, Is.Not.Null);
                Assert.That(() => context.Model, Throws.Nothing);
            });
            Assert.Multiple(() =>
            {
                Assert.That(context.Users, Is.Not.Null);
                Assert.That(context.Companies, Is.Not.Null);
                Assert.That(context.Deliveries, Is.Not.Null);
            });
        }
    }
}