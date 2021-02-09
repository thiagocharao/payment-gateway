namespace PaymentAPI.Domain
{
    public class User : Document
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
