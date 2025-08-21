namespace SkinHolderDesktop.Models;

public class SteamItemInfo
{
    public string HashName { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
    public bool IsError { get; set; } = false;
    public bool IsWarning { get; set; } = false;
}
