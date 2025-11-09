using Buildify.Core.Entities.Identity;

namespace Buildify.Core.Repositories;

public interface IOtpRepository
{
    Task<Otp> GetOtpByEmailAsync(string email, OtpPurpose purpose);
    Task<Otp> GetOtpByResetTokenAsync(string resetToken);
    Task AddOtpAsync(Otp otp);
    Task UpdateOtpAsync(Otp otp);
    Task DeleteOtpAsync(Otp otp);
    Task InvalidateAllOtpsByEmailAsync(string email, OtpPurpose purpose);
}
