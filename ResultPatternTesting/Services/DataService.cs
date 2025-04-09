using ResultPatternTesting.Entity;

namespace ResultPatternTesting.Services
{
    public class DataService
    {
        private readonly DataDbContext _dbContext;

        public DataService(DataDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Result<int> Post(Data data)
        {
            if(data.Value == "Error")
            {
                return Result.Failure<int>(DataErrors.InvalidValue);
            }

            _dbContext.Add(data);
            _dbContext.SaveChanges();
            return Result.Success<int>(data.Id);
        }
    }
}
