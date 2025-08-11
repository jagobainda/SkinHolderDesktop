namespace SkinHolderDesktop.Models;

public class Logger
{
    public string LogDescription { get; set; } = null!;

    public int LogTypeId { get; set; }

    public int LogPlaceId { get; set; }

    public int UserId { get; set; }
}
