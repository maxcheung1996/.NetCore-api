namespace api.Models
{
    public class Response
    {
        public bool Status { get; set; }
        public string Message { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}