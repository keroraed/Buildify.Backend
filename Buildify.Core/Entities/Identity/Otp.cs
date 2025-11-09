namespace Buildify.Core.Entities.Identity;

public class Otp
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsUsed { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiration { get; set; }
    public OtpPurpose Purpose { get; set; } // Email verification or password reset
}

public enum OtpPurpose
{
    EmailVerification = 1,
    PasswordReset = 2
}
