namespace BAM.Domain.Models
{
    public class Bank
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string BIC { get; set; } = string.Empty;
    }
}
