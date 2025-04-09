using Xunit;
using ResultPatternTesting.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using ResultPatternTesting.Entity;
using FluentAssertions;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace ResultPatternTesting.Caching.Tests
{
    public class CacheServiceTests
    {
        [Fact()]
        public async Task GetAsync_WithNotEmptyCache_ShouldReturnValue()
        {
            var data = "{\"Id\":1,\"Value\":\"string\",\"CreatedAt\":\"0001-01-01T00:00:00\",\"LastUpdatedAt\":\"0001-01-01T00:00:00\"}";
            var byteData = Encoding.UTF8.GetBytes(data);
            var distributedCacheMock = new Mock<IDistributedCache>();
            distributedCacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(byteData);
            var cacheService = new CacheService(distributedCacheMock.Object);
            var result = await cacheService.GetAsync<Data>("key");
            result.Should().NotBeNull();
            result.Should().BeOfType<Data>();
        }
        [Fact()]
        public async Task GetAsync_WithEmptyCache_ShouldReturnNull()
        {
            //var data = "{\"Id\":1,\"Value\":\"string\",\"CreatedAt\":\"0001-01-01T00:00:00\",\"LastUpdatedAt\":\"0001-01-01T00:00:00\"}";
            var distributedCacheMock = new Mock<IDistributedCache>();
            distributedCacheMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])default!);
            var cacheService = new CacheService(distributedCacheMock.Object);
            var result = await cacheService.GetAsync<Data>("key");
            result.Should().BeNull();
            //result.Should().BeOfType<Data>();
        }
        [Fact()]
        public async Task SetAsyncTest()
        {
            var distributedCacheMock = new Mock<IDistributedCache>();
            //distributedCacheMock.Setup(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            var cacheService = new CacheService(distributedCacheMock.Object);
            await cacheService.SetAsync("key", It.IsAny<Data>());

            distributedCacheMock.Verify(x => x.SetAsync("key", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}