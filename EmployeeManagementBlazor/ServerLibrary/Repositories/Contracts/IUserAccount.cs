using BaseLibrary.DTOs;
using BaseLibrary.Responses;


namespace ServerLibrary.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register register);
        Task<LoginResponse> SignInAsync(Login login);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken refreshToken);
    }
}
