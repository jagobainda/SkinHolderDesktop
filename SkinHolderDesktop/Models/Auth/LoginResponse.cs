namespace SkinHolderDesktop.Models.Auth;

public class LoginResponse
{
    public string Token { get; set; } = "";
    public string Username { get; set; } = "";
    public int UserId { get; set; }
}
