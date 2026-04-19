namespace SkinHolderDesktop.Models;

public class UserInfoResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
