using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ResultPatternTesting.Entity;
using ResultPatternTesting.Features.Query;
using ResultPatternTesting.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ResultPatternTesting.Services.Tests
{
    public class DataServiceTests
    {
        [Fact()]
        public void PostTest_WithCorrectData()
        {
            var data = new Data()
            {
                Value = "someValue"
            };
            var getQuery = new GetAllQuery();
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseInMemoryDatabase("database");
            var dataService = new DataService(new DataDbContext(optionsBuilder.Options));

            var result = dataService.Post(data);

            result.IsSuccess.Should().BeTrue();

        }
        [Fact()]
        public void PostTest_WithInCorrectData()
        {
            var data = new Data()
            {
                Value = "Error"
            };
            var getQuery = new GetAllQuery();
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseInMemoryDatabase("database");
            var dataService = new DataService(new DataDbContext(optionsBuilder.Options));

            var result = dataService.Post(data);

            result.IsFailure.Should().BeTrue();

        }
    }
}