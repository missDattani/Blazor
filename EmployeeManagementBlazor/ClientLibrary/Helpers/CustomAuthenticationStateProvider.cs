using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;

namespace ClientLibrary.Helpers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorageService _storageService;
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public CustomAuthenticationStateProvider(LocalStorageService storageService)
        {
            _storageService = storageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var stringToken = await _storageService.GetToken();
            if (string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(anonymous));

            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
            if(deserializeToken == null) return await Task.FromResult(new AuthenticationState(anonymous));

            var getUserClaims = DecryptToken(deserializeToken.Token);
            if(getUserClaims == null) return await Task.FromResult(new AuthenticationState(anonymous));

            var claimPrincipal = SetClaimPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        }

        private static CustomUserClaims DecryptToken(string jwtToken)
        {
            try { 
            if (string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();

            // Deserialize the JSON token object
            var tokenObject = JsonSerializer.Deserialize<UserSession>(jwtToken);
            if (tokenObject == null || string.IsNullOrEmpty(tokenObject.Token)) return new CustomUserClaims();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenObject.Token); // Use the actual JWT from the deserialized object

            var userId = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier);
            var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name);
            var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email);
            var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role);
                return new CustomUserClaims(userId?.Value, name?.Value, email?.Value, role?.Value);
            }
            catch(Exception ex)
            {
                return new CustomUserClaims();
            }
        }


        public static ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, claims.Id),
                    new(ClaimTypes.Name, claims.Name),
                    new(ClaimTypes.Email, claims.Email),
                    new(ClaimTypes.Role, claims.Role)
                }, "JwtAuth"));
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if(userSession.Token != null || userSession.RefreshToken != null)
            {
                var serializeSession = Serializations.SerializeObj(userSession);
                await _storageService.SetToken(serializeSession);
                var getUserClaims = DecryptToken(serializeSession);
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }
            else
            {
                await _storageService.RemoveToken();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
