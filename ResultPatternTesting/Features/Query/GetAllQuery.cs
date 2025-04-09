using ResultPatternTesting.Abstractions;
using ResultPatternTesting.Entity;

namespace ResultPatternTesting.Features.Query
{
    public sealed record GetAllQuery() : IQuery<IEnumerable<Data>>;
}
