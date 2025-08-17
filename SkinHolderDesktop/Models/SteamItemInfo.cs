namespace SkinHolderDesktop.Models;

public class SteamItemInfo
{
    public string HashName { get; set; } = string.Empty;
    public double Price { get; set; } = 0.0;
    public bool IsError { get; set; } = false;
}
