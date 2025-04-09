using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using ResultPatternTesting.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ResultPatternTesting.Features.Command;
using Moq;
using Newtonsoft.Json;
using FluentAssertions;
using System.Net;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using ResultPatternTesting.Features.Query;
using ResultPatternTesting.Entity;
using Microsoft.EntityFrameworkCore;
using ResultPatternTesting.Caching;

namespace ResultPatternTesting.Controllers.Tests
{
    [Collection("api")]
    public class DataControllerTests : WebApplicationFactory<Program>
    {
        private HttpClient _client;
        public DataControllerTests(/*Action<IServiceCollection> services = default!*/)
        {
            _client = WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("test");
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<DataDbContext>));

                    services.Remove(dbContextOptions!);
                    services
                         .AddDbContext<DataDbContext>(options => options.UseInMemoryDatabase("DataDb"));
                });
            }).CreateClient();
        }
        [Fact()]
        public async Task Create_Data_With_Valid_Input_Should_Return_NoContent()
        {
            //var factory = new WebApplicationFactory<Program>();
            //var _client = factory.CreateClient();
            var createCommand = new CreateCommand("testvalue");
            //var serializedModel = JsonConvert.SerializeObject(createCommand);
            //var httpContent = new StringContent(serializedModel, Encoding.UTF8, "application/json");
            var response = await _client.PostAsJsonAsync("/data", createCommand);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact()]
        public async Task Create_Data_With_Invalid_Input_Should_Return_BadRequest()
        {
            var createCommand = new CreateCommand("");
            var response = await _client.PostAsJsonAsync("/data", createCommand);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact()]
        public async Task GetAllTest()
        {
            IEnumerable<Data> datas = new List<Data>()
            {
                new Data(){ Value = "1"},
                new Data(){ Value = "2"},
                new Data(){ Value = "3"},
            };
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(x => x.GetAsync<IEnumerable<Data>>(It.IsAny<string>()))
                .ReturnsAsync(datas);
            var response = await _client.GetAsync("/data");
            response.Content.ReadAsStringAsync().Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}