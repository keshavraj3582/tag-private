namespace School_Login_SignUp.Models
{
    // CreateOrderRequest.cs
    public class CreateOrderRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Dictionary<string, string> Notes { get; set; }
    }

}
