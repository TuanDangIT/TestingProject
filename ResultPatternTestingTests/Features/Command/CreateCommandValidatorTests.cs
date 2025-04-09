using Xunit;
using ResultPatternTesting.Features.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;

namespace ResultPatternTesting.Features.Command.Tests
{
    public class CreateCommandValidatorTests
    {
        [Fact()]
        public void Validate_WithCorrectCommand_ShouldNotHaveValidationErrors()
        {
            var validator = new CreateCommandValidator();
            var command = new CreateCommand("data");
            var result = validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
        [Fact()]
        public void Validate_WithIncorrectCommand_ShouldHaveValidationErrors()
        {
            var validator = new CreateCommandValidator();
            var command = new CreateCommand(default!);
            var result = validator.TestValidate(command);
            result.ShouldHaveAnyValidationError();
        }
    }
}