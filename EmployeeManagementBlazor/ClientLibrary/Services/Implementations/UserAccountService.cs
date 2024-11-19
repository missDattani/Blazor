using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.Services.Implementations
{
    public class UserAccountService : IUserAccountService
    {
        private readonly GetHttpClient _getHttpClient;
        public const string AuthUrl = "api/Authentication";
        public UserAccountService(GetHttpClient getHttpClient)
        {
            _getHttpClient = getHttpClient;
        }
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var httpClient = _getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error Occured");

            return await result.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<LoginResponse> LoginAsync(Login user)
        {
            var httpClient = _getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occured");

            return await result.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken refreshToken)
        {
            var httpClient = _getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh_token", refreshToken);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occured");

            return await result.Content.ReadFromJsonAsync<LoginResponse>();
        }
    }
}
