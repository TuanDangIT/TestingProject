namespace ResultPatternTesting
{
    public sealed record Error(string Code, string? Description = null)
    {
        public static readonly Error None = new(string.Empty);
        public static implicit operator Result(Error error) => Result.Failure(error);
    }
    public class DataErrors
    {
        public static readonly Error InvalidId = new Error("Data.Id", "Invalid Id");
        public static readonly Error InvalidValue = new Error("Data.Value", "Invalid Value");
    }
}
