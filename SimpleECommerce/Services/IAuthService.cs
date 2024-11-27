using SimpleECommerce.DataAndContext.ModelsForEommerce;
using SimpleECommerce.vModels;

namespace SimpleECommerce.Services
{
    public interface IAuthService
    {
        string getUserId();
        // for registering user
        Task<string> sendOtpToConfirmEmailAsync(sendOTPForLoginModel model);
        Task<registerResult> registerAsync(userDataModel model);
        // log in
        Task<registerResult> getTokenAsync(loginModel model);
        // log out
        string LogOutAsync();
        // forget password 2 funcs(OtpResetPass, resetPass)
        Task<string> SendOtpForResetPassAsync(sendOTPForLoginModel model);
        Task<string> resetPassAsync(resetPassModel model);

        // Address management methods
        Task<string> AddAddressToUser(AddressInputModel model);
        Task<IEnumerable<Address>> ShowUserAddresses();
        Task<string> DeleteAddressForUser(int addressId);
        Task<string> UpdateAddress(AddressInputModel model);
        Task<string> SetAsDefaultAddress(int addressId);

        // Token blacklisting methods
        void BlacklistToken(string token);
        bool IsTokenBlacklisted(string token);

    }
}

