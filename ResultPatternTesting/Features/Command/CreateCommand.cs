using System.Windows.Input;

namespace ResultPatternTesting.Features.Command
{
    public sealed record CreateCommand(string Value) : Abstractions.ICommand;
}
