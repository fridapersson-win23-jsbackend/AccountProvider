namespace AccountProvider.Models;

public class SignInRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}
