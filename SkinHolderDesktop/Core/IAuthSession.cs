namespace SkinHolderDesktop.Core;

public interface IAuthSession : ITokenProvider
{
    new string? Token { get; set; }
    string? CurrentUsername { get; set; }
    int UserId { get; set; }
    bool IsAuthenticated { get; set; }
}
