using Microsoft.EntityFrameworkCore;
using ResultPatternTesting.Abstractions;
using ResultPatternTesting.Caching;
using ResultPatternTesting.Entity;

namespace ResultPatternTesting.Features.Query
{
    public class GetAllQueryHandler : IQueryHandler<GetAllQuery, IEnumerable<Data>>
    {
        private readonly DataDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetAllQueryHandler> _logger;

        public GetAllQueryHandler(DataDbContext dbContext, ICacheService cacheService, ILogger<GetAllQueryHandler> logger)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task<Result<IEnumerable<Data>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = "datas-123";
            var results = await _cacheService.GetAsync<IEnumerable<Data>>(cacheKey);
            if(results is not null && results.Count() > 0)
            {
                _logger.LogInformation("Results from cache");
                return Result.Success<IEnumerable<Data>>(results);
            }
            results = await _dbContext.Datas
                .AsNoTracking()
                .ToListAsync();
            await _cacheService.SetAsync(cacheKey, results);
            _logger.LogInformation("Results from database");
            return Result.Success<IEnumerable<Data>>(results);
        }
    }
}
