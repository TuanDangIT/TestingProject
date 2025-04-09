using Xunit;
using ResultPatternTesting.Features.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ResultPatternTesting.Entity;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace ResultPatternTesting.Features.Command.Tests
{
    public class CreateCommandHandlerTests
    {
        [Fact()]
        public async Task Handle_CreatesData_WithValidInput()
        {
            var createCommand = new CreateCommand("Data");
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseInMemoryDatabase("database");
            var handler = new CreateCommandHandler(new DataDbContext(optionsBuilder.Options));
            var result = await handler.Handle(createCommand, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }
        [Fact()]
        public async Task Handle_DoesntCreatesData_WithInvalidInput()
        {
            var createCommand = new CreateCommand(default!);
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseInMemoryDatabase("database");
            var handler = new CreateCommandHandler(new DataDbContext(optionsBuilder.Options));
            var result = await handler.Handle(createCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}