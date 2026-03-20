namespace Demo01.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string ExternalRef { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}