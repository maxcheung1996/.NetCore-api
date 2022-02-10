namespace api.Models
{
    public class UserLoginRequest
    {
        public string Mail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}