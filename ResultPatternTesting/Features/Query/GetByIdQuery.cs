using ResultPatternTesting.Abstractions;
using ResultPatternTesting.Entity;

namespace ResultPatternTesting.Features.Query
{
    public class GetByIdQuery : IQuery<Data>
    {
        public GetByIdQuery(int id)
        {
            Id = id;
        }
        public int Id { get; set; } 
    }
}
