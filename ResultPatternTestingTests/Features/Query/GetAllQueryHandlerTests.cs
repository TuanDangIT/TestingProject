using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ResultPatternTesting.Caching;
using ResultPatternTesting.Entity;
using ResultPatternTesting.Features.Command;
using ResultPatternTesting.Features.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ResultPatternTesting.Features.Query.Tests
{
    public class GetAllQueryHandlerTests
    {
        [Fact()]
        public async Task Handler_ReturnsDatas_WithEmptyCache()
        {
            var getQuery = new GetAllQuery();
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseInMemoryDatabase("database");
            var cacheServiceMock = new Mock<ICacheService>();
            var loggerMock = new Mock<ILogger<GetAllQueryHandler>>();
            var handler = new GetAllQueryHandler(new DataDbContext(optionsBuilder.Options), cacheServiceMock.Object, loggerMock.Object);
            var result = await handler.Handle(getQuery, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<Data>>();
        }
        [Fact()]
        public async Task Handler_ReturnsDatas_WithNotEmptyCache()
        {
            var getQuery = new GetAllQuery();
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseInMemoryDatabase("database");
            var cacheServiceMock = new Mock<ICacheService>();
            IEnumerable<Data> datas = new List<Data>() 
            { 
                new Data(){ Value = "1"},
                new Data(){ Value = "2"},
                new Data(){ Value = "3"},
            };
            cacheServiceMock.Setup(x => x.GetAsync<IEnumerable<Data>>(It.IsAny<string>()))
                .ReturnsAsync(datas);
            var loggerMock = new Mock<ILogger<GetAllQueryHandler>>();
            var handler = new GetAllQueryHandler(new DataDbContext(optionsBuilder.Options), cacheServiceMock.Object, loggerMock.Object);
            var result = await handler.Handle(getQuery, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<Data>>();
        }

    }
}