using MediatR;
using Microsoft.EntityFrameworkCore;
using ResultPatternTesting.Abstractions;
using ResultPatternTesting.Entity;

namespace ResultPatternTesting.Features.Query
{
    public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, Data>
    {
        private readonly DataDbContext _dataDbContext;

        public GetByIdQueryHandler(DataDbContext dataDbContext)
        {
            _dataDbContext = dataDbContext;
        }

        public async Task<Result<Data>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _dataDbContext.Datas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (data == null)
            {
                return Result.Failure<Data>(DataErrors.InvalidId);
            }
            return Result.Success<Data>(data);
        }
    }
}
