using FluentValidation;
using System.Data;

namespace ResultPatternTesting.Features.Command
{
    public class CreateCommandValidator : AbstractValidator<CreateCommand>
    {
        public CreateCommandValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                .NotNull()
                .NotEqual("123");
        }
    }
}
