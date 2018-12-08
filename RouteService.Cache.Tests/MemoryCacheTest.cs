using NUnit.Framework;
using System;
using System.Threading.Tasks;
using RouteService.Cache;

namespace RouteService.Cache.Tests
{
    [TestFixture]
    public class MemoryCacheTests
    {
        [Test]
        public async Task GetOrCreateAsyncFactoryCalledTest()
        {
            MemoryCache<string, string> cache = new MemoryCache<string, string>(TimeSpan.FromHours(1));
            int callsCount = 0;
            var value = await cache.GetOrCreateAsync("key", key => { callsCount++; return Task.FromResult("value"); });
            Assert.AreEqual(1, callsCount);
            Assert.AreEqual("value", value);
        }

        [Test]
        public async Task GetOrCreateAsyncValueCachedTest()
        {
            MemoryCache<string, string> cache = new MemoryCache<string, string>(TimeSpan.FromHours(1));
            int callsCount = 0;
            await cache.GetOrCreateAsync("key", key => { callsCount++; return Task.FromResult("value"); });
            var value = await cache.GetOrCreateAsync("key", key => { callsCount++; return Task.FromResult("value 2"); });
            Assert.AreEqual(1, callsCount);
            Assert.AreEqual("value", value);
        }

        [Test]
        public async Task GetOrCreateAsyncTimespanTest()
        {
            MemoryCache<string, string> cache = new MemoryCache<string, string>(TimeSpan.FromTicks(0));
            int callsCount = 0;
            await cache.GetOrCreateAsync("key", key => { callsCount++; return Task.FromResult("value"); });
            var value = await cache.GetOrCreateAsync("key", key => { callsCount++; return Task.FromResult("value 2"); });
            Assert.AreEqual(2, callsCount);
            Assert.AreEqual("value 2", value);
        }

    }
}
