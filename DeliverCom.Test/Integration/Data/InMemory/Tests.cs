using DeliverCom.Data.InMemory.Store;
using DeliverCom.Test.Integration.Model;
using NUnit.Framework;

namespace DeliverCom.Test.Integration.Data.InMemory
{
    public class Tests
    {
        private InMemoryKeyValueStore _store = new();


        [Test]
        public virtual async Task NotFoundKey()
        {
            var value = await _store.GetAsync<string>("none");
            Assert.That(value, Is.Null);
        }

        [Test]
        public virtual Task SetPrimitiveTypesTestAsync()
        {
            Assert.Multiple(async () =>
            {
                Assert.That(await _store.SetAsync("string", "str"));
                Assert.That(await _store.SetAsync("short", short.MaxValue), Is.True);
                Assert.That(await _store.SetAsync("int", int.MaxValue), Is.True);
                Assert.That(await _store.SetAsync("long", long.MaxValue), Is.True);
                Assert.That(await _store.SetAsync("decimal", decimal.MaxValue / 10), Is.True);
                Assert.That(await _store.SetAsync("float", float.MaxValue / 10), Is.True);
                Assert.That(await _store.SetAsync("date", DateTime.Today), Is.True);
                Assert.That(await _store.SetAsync("null", null), Is.False);
            });
            return Task.CompletedTask;
        }

        [Test]
        public virtual Task GetPrimitiveTypesTestAsync()
        {
            Assert.Multiple(async () =>
            {
                Assert.That(await _store.GetAsync<string>("string"), Is.EqualTo("str"));
                Assert.That(await _store.GetAsync<short>("short"), Is.EqualTo(short.MaxValue));
                Assert.That(await _store.GetAsync<int>("int"), Is.EqualTo(int.MaxValue));
                Assert.That(await _store.GetAsync<long>("long"), Is.EqualTo(long.MaxValue));
                Assert.That(await _store.GetAsync<decimal>("decimal"), Is.EqualTo(decimal.MaxValue / 10));
                Assert.That(await _store.GetAsync<float>("float"), Is.EqualTo(float.MaxValue / 10));
                Assert.That(await _store.GetAsync<DateTime>("date"), Is.EqualTo(DateTime.Today));
                Assert.That(await _store.GetAsync<object>("null"), Is.Null);
            });
            return Task.CompletedTask;
        }

        [Test]
        public virtual async Task SetStructTestAsync()
        {
            var result = await _store.SetAsync("location", new StructType(40, 40));
            Assert.That(result, Is.True);
        }

        [Test]
        public virtual async Task SetEnumTestAsync()
        {
            var result = await _store.SetAsync("enum", OrderStatus.Processed);
            Assert.That(result, Is.True);

            var @enum = await _store.GetAsync<OrderStatus>("enum");
            Assert.That(@enum, Is.EqualTo(OrderStatus.Processed));
        }


        [Test]
        public virtual async Task GetStructTestAsync()
        {
            var location = await _store.GetAsync<StructType>("location");
            Assert.Multiple(() =>
            {
                Assert.That(location.X, Is.EqualTo(40));
                Assert.That(location.Y, Is.EqualTo(40));
            });
        }

        [Test]
        public virtual async Task SetClassTestAsync()
        {
            var result = await _store.SetAsync("order", OrderTestData.FirstOrder);
            Assert.That(result, Is.True);
        }


        [Test]
        public virtual async Task GetClassTestAsync()
        {
            await _store.SetAsync("order", OrderTestData.FirstOrder);
            var result = await _store.GetAsync<Order>("order");
            Assert.That(new OrderEqualityComparer().Equals(result, OrderTestData.FirstOrder), Is.True);
        }

        [Test]
        public virtual async Task DeleteTestAsync()
        {
            var key = "DeleteTest";

            var result = await _store.SetAsync(key, key);
            Assert.That(result, Is.True);

            var value = await _store.GetAsync<string>(key);
            Assert.That(value, Is.EqualTo(key));

            result = await _store.DeleteAsync(key);
            Assert.That(result, Is.True);

            result = await _store.DeleteAsync(key);
            Assert.That(result, Is.False);
        }

        [Test]
        public virtual async Task ExistsTestAsync()
        {
            var key = "ExistsTest";

            var result = await _store.ExistsAsync(key);
            Assert.That(result, Is.False);

            result = await _store.SetAsync(key, key);
            Assert.That(result, Is.True);

            result = await _store.ExistsAsync(key);
            Assert.That(result, Is.True);
        }


        [Test]
        public virtual async Task SetWithExpireTestAsync()
        {
            var expire = TimeSpan.FromSeconds(3);
            var key = "SetWithExpireTest";

            var result = await _store.SetAsync(key, key, expire);
            Assert.That(result, Is.True);

            var value = await _store.GetAsync<string>(key);
            Assert.That(value, Is.EqualTo(key));

            Thread.Sleep(1000);
            Thread.Sleep(expire);

            result = await _store.ExistsAsync(key);
            Assert.That(result, Is.False);
        }

        [Test]
        public virtual async Task ExpireTestAsync()
        {
            var expire = TimeSpan.FromSeconds(3);
            var key = "ExpireTest";

            var result = await _store.SetAsync(key, key, expire);
            Assert.That(result, Is.True);

            var value = await _store.GetAsync<string>(key);
            Assert.That(value, Is.EqualTo(key));

            Thread.Sleep(expire.Subtract(TimeSpan.FromSeconds(1)));

            result = await _store.ExpireAsync(key, expire);
            Assert.That(result, Is.True);

            Thread.Sleep(expire.Subtract(TimeSpan.FromSeconds(1)));

            result = await _store.ExistsAsync(key);
            Assert.That(result, Is.True);
        }

        [Test]
        public virtual async Task PersistTestAsync()
        {
            var expire = TimeSpan.FromSeconds(3);
            var key = "PersistTest";

            var result = await _store.SetAsync(key, key, expire);
            Assert.That(result, Is.True);

            var value = await _store.GetAsync<string>(key);
            Assert.That(value, Is.EqualTo(key));

            Thread.Sleep(expire.Subtract(TimeSpan.FromSeconds(1)));

            result = await _store.PersistAsync(key);
            Assert.That(result, Is.True);

            Thread.Sleep(expire);

            result = await _store.ExistsAsync(key);
            Assert.That(result, Is.True);
        }


        [Test]
        public virtual void SetPrimitiveTypesTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_store.Set("string", "str"), Is.True);
                Assert.That(_store.Set("short", short.MaxValue), Is.True);
                Assert.That(_store.Set("int", int.MaxValue), Is.True);
                Assert.That(_store.Set("long", long.MaxValue), Is.True);
                Assert.That(_store.Set("decimal", decimal.MaxValue / 10), Is.True);
                Assert.That(_store.Set("float", float.MaxValue / 10), Is.True);
                Assert.That(_store.Set("date", DateTime.Today), Is.True);
                Assert.That(_store.Set("null", null), Is.False);
            });
        }

        [Test]
        public virtual void GetPrimitiveTypesTest()
        {
            _store.Set("string", "str");
            _store.Set("short", short.MaxValue);
            _store.Set("int", int.MaxValue);
            _store.Set("long", long.MaxValue);
            _store.Set("decimal", decimal.MaxValue / 10);
            _store.Set("float", float.MaxValue / 10);
            _store.Set("date", DateTime.Today);
            _store.Set("null", null);
            Assert.Multiple(() =>
            {
                Assert.That(_store.Get<string>("string"), Is.EqualTo("str"));
                Assert.That(_store.Get<short>("short"), Is.EqualTo(short.MaxValue));
                Assert.That(_store.Get<int>("int"), Is.EqualTo(int.MaxValue));
                Assert.That(_store.Get<long>("long"), Is.EqualTo(long.MaxValue));
                Assert.That(_store.Get<decimal>("decimal"), Is.EqualTo(decimal.MaxValue / 10));
                Assert.That(_store.Get<float>("float"), Is.EqualTo(float.MaxValue / 10));
                Assert.That(_store.Get<DateTime>("date"), Is.EqualTo(DateTime.Today));
                Assert.That(_store.Get<object>("null"), Is.Null);
            });
        }

        [Test]
        public virtual void SetStructTest()
        {
            var result = _store.Set("location", new StructType(40, 40));
            Assert.That(result, Is.True);
        }

        [Test]
        public virtual void SetEnumTest()
        {
            var result = _store.Set("enum", OrderStatus.Processed);
            Assert.That(result, Is.True);

            var @enum = _store.Get<OrderStatus>("enum");
            Assert.That(@enum, Is.EqualTo(OrderStatus.Processed));
        }


        [Test]
        public virtual void GetStructTest()
        {
            _store.Set("location", new StructType(40, 40));
            var location = _store.Get<StructType>("location");
            Assert.Multiple(() =>
            {
                Assert.That(location.X, Is.EqualTo(40));
                Assert.That(location.Y, Is.EqualTo(40));
            });
        }

        [Test]
        public virtual void SetClassTest()
        {
            var result = _store.Set("order", OrderTestData.FirstOrder);
            Assert.That(result, Is.True);
        }


        [Test]
        public virtual void GetClassTest()
        {
            _store.Set("order", OrderTestData.FirstOrder);
            var result = _store.Get<Order>("order");
            Assert.That(new OrderEqualityComparer().Equals(result, OrderTestData.FirstOrder), Is.True);

            var notFoundOrder = _store.Get<Order>("NotFoundOrder");
            Assert.That(notFoundOrder, Is.Null);
        }

        [Test]
        public virtual void DeleteTest()
        {
            var key = "DeleteTest";

            var result = _store.Set(key, key);
            Assert.That(result, Is.True);

            var value = _store.Get<string>(key);
            Assert.That(value, Is.EqualTo(key));

            result = _store.Delete(key);
            Assert.That(result, Is.True);

            result = _store.Delete(key);
            Assert.That(result, Is.False);
        }

        public virtual void ExistsTest()
        {
            var key = "ExistsTest";

            var result = _store.Exists(key);
            Assert.That(result, Is.False);

            result = _store.Set(key, key);
            Assert.That(result, Is.True);

            result = _store.Exists(key);
            Assert.That(result, Is.True);
        }


        [Test]
        public virtual void SetWithExpireTest()
        {
            var expire = TimeSpan.FromSeconds(3);
            var key = "SetWithExpireTest";

            var result = _store.Set(key, key, expire);
            Assert.That(result, Is.True);

            var value = _store.Get<string>(key);
            Assert.That(value, Is.EqualTo(key));

            Thread.Sleep(1000);
            Thread.Sleep(expire);

            result = _store.Exists(key);
            Assert.That(result, Is.False);
        }

        [Test]
        public virtual void ExpireTest()
        {
            var expire = TimeSpan.FromSeconds(3);
            var key = "ExpireTest";

            var result = _store.Set(key, key, expire);
            Assert.That(result, Is.True);

            var value = _store.Get<string>(key);
            Assert.That(value, Is.EqualTo(key));

            Thread.Sleep(expire.Subtract(TimeSpan.FromSeconds(1)));

            result = _store.Expire(key, expire);
            Assert.That(result, Is.True);

            Thread.Sleep(expire.Subtract(TimeSpan.FromSeconds(1)));

            result = _store.Exists(key);
            Assert.That(result, Is.True);

            result = _store.Expire("NotFoundKey", expire);
            Assert.That(result, Is.False);
        }

        [Test]
        public virtual void PersistTest()
        {
            var expire = TimeSpan.FromSeconds(3);
            var key = "PersistTest";

            var result = _store.Set(key, key, expire);
            Assert.That(result, Is.True);

            var value = _store.Get<string>(key);
            Assert.That(value, Is.EqualTo(key));

            Thread.Sleep(expire.Subtract(TimeSpan.FromSeconds(1)));

            result = _store.Persist(key);
            Assert.That(result, Is.True);

            Thread.Sleep(expire);

            result = _store.Exists(key);
            Assert.That(result, Is.True);

            result = _store.Persist("NotFoundKey");
            Assert.That(result, Is.False);
        }
    }
}