namespace SkinHolderDesktop.Core;

public interface IAuthSession : ITokenProvider
{
    public new string? Token { get; set; }
    public string? CurrentUsername { get; set; }
    public int UserId { get; set; }
    public bool IsAuthenticated { get; set; }
}
