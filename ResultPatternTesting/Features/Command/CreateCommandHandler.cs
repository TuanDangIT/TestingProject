using MediatR.Wrappers;
using ResultPatternTesting.Abstractions;
using ResultPatternTesting.Entity;

namespace ResultPatternTesting.Features.Command
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand>
    {
        private readonly DataDbContext _dbContext;

        public CreateCommandHandler(DataDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            if(request.Value is null || request.Value == "")
            {
                return Result.Failure(DataErrors.InvalidValue);
            }
            var data = new Data()
            {
                Value = request.Value,
            };
            var entry = _dbContext.Attach(data);
            await _dbContext.AddAsync(data);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
    }
}
