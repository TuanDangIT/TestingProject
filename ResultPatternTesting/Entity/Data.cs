namespace ResultPatternTesting.Entity
{
    public class Data : IAuditable
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        private string AnotherValue = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set;}
    }
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
