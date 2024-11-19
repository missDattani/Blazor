

using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Constants = ServerLibrary.Helpers.Constants;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository : IUserAccount
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly JwtConfig _jwtConfig;
        public UserAccountRepository(IOptions<JwtConfig> jwtConfig, FirestoreDb firestoreDb)
        {
            _jwtConfig = jwtConfig.Value;
            _firestoreDb = firestoreDb;
        }
        public async Task<GeneralResponse> CreateAsync(Register register)
        {
            if(register is null) return new GeneralResponse(false, "Model is empty");

            var checkUser = await FindUserByEmail(register.Email);
            if (checkUser != null) return new GeneralResponse(false, "User registered already");

            var applicationUser = await AddUserAsync(new ApplicationUser()
            {
                FullName = register.Fullname,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password)
            });

            var adminRole = await GetOrCreateRoleAsync(Constants.Admin);

            // Assign the role to the new user
            await AssignRoleToUser(adminRole.Id, applicationUser.Id);

            var userRole = await GetOrCreateRoleAsync(Constants.User);

            // Assign the user role to the new user
            await AssignRoleToUser(userRole.Id, applicationUser.Id);

            return new GeneralResponse(true, "Account created!");
        }

        public async Task<LoginResponse> SignInAsync(Login login)
        {
            if (login is null) return new LoginResponse(false, "Model is empty");

            var findUser = await FindUserByEmail(login.Email);
            if (findUser == null) return new LoginResponse(false, "User not found");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, findUser.Password))
                return new LoginResponse(false, "Email/Password not valid");

            // Query the UserRoles collection to find the user's role by UserId
            var userRolesRef = _firestoreDb.Collection("UserRole");
            var userRoleQuery = userRolesRef.WhereEqualTo("UserId", findUser.Id);
            var userRoleSnapshot = await userRoleQuery.GetSnapshotAsync();

            // Find the user role by UserId
            var userRole = await FindUserRole(findUser.Id);
            if (userRole == null)
                return new LoginResponse(false, "User role not found");

            // Find the role name by RoleId
            var roleName = await FindRoleName(userRole.RoleId);
            if (roleName == null)
                return new LoginResponse(false, "Role not found");

            string jwtToken = GenerateToken(findUser, roleName);
            string refreshToken = GenerateRefreshToken();

            // Save or update refresh token in Firestore
            var refreshTokenRef = _firestoreDb.Collection("RefreshTokenInfo");
            // Check if a refresh token already exists for the user
            var existingTokenQuery = refreshTokenRef.WhereEqualTo("UserId", findUser.Id);
            var existingTokenSnapshot = await existingTokenQuery.GetSnapshotAsync();
            if (existingTokenSnapshot.Documents.Count > 0)
            {
                // Update the existing refresh token
                var existingTokenDoc = existingTokenSnapshot.Documents.First();
                var updateData = new Dictionary<string, object>
                {
                    { "RToken", refreshToken },
                    { "UpdatedAt", Timestamp.FromDateTime(DateTime.UtcNow) }  // optional, to track token update time
                };
                await existingTokenDoc.Reference.UpdateAsync(updateData);
            }
            else
            {
                // Save a new refresh token entry
                var newRefreshToken = new Dictionary<string, object>
                {
                    { "UserId", findUser.Id },
                    { "RToken", refreshToken },
                    { "CreatedAt", Timestamp.FromDateTime(DateTime.UtcNow) }  // optional, to track token creation time
                };
                await refreshTokenRef.AddAsync(newRefreshToken);
            }

            return new LoginResponse(true, "Login Successfull",jwtToken, refreshToken);
        }
        private async Task<UserRole?> FindUserRole(string userId)
        {
            var userRolesRef = _firestoreDb.Collection("UserRole");
            var userRoleQuery = userRolesRef.WhereEqualTo("UserId", userId);
            var userRoleSnapshot = await userRoleQuery.GetSnapshotAsync();

            if (userRoleSnapshot.Documents.Count == 0)
                return null;  // No user role found

            // Get the first user role (assuming one role per user)
            var userRoleDoc = userRoleSnapshot.Documents.First();
            return userRoleDoc.ConvertTo<UserRole>();  // Convert to UserRole entity
        }

        private async Task<string?> FindRoleName(string roleId)
        {
            var systemRoleDocRef = _firestoreDb.Collection("SystemRole").Document(roleId);
            var systemRoleDoc = await systemRoleDocRef.GetSnapshotAsync();

            if (!systemRoleDoc.Exists)
                return null;  // No system role found

            // Convert to SystemRole entity and return the role name
            var systemRole = systemRoleDoc.ConvertTo<SystemRole>();
            return systemRole?.Name;  // Return the role name
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            // Define the claims for the token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role!)
            };
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        public async Task<ApplicationUser> FindUserByEmail(string email)
        {
            CollectionReference userRef = _firestoreDb.Collection("ApplicationUser");
            Query query = userRef.WhereEqualTo("Email", email);
            QuerySnapshot querySnapshots = await query.GetSnapshotAsync();

            if(querySnapshots.Documents.Count > 0)
            {
                DocumentSnapshot documentSnapshot = querySnapshots.Documents.First();
                ApplicationUser user = documentSnapshot.ConvertTo<ApplicationUser>();
                return user;
            }
            return null;
        }

        public async Task<ApplicationUser> AddUserAsync(ApplicationUser applicationUser)
        {
            if (applicationUser == null)
                throw new ArgumentNullException(nameof(applicationUser));

            CollectionReference userRef = _firestoreDb.Collection("ApplicationUser");
            DocumentReference documentReference = await userRef.AddAsync(applicationUser);

            // Set Firestore document ID
            applicationUser.Id = documentReference.Id;

            return applicationUser;
        }


        private async Task<SystemRole> GetOrCreateRoleAsync(string roleName)
        {
            var role = await FindRoleByName(roleName);

            if (role == null)
            {
                role = await AddRoleAsync(new SystemRole { Name = roleName });
            }

            return role;
        }

        private async Task<SystemRole> FindRoleByName(string roleName)
        {
            var rolesRef = _firestoreDb.Collection("SystemRole");
            var query = rolesRef.WhereEqualTo("Name", roleName);
            var querySnapshot = await query.GetSnapshotAsync();

            return querySnapshot.Documents.Count > 0
                ? querySnapshot.Documents.First().ConvertTo<SystemRole>()
                : null;
        }

        private async Task<SystemRole> AddRoleAsync(SystemRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var rolesRef = _firestoreDb.Collection("SystemRole");
            DocumentReference docRef = await rolesRef.AddAsync(role);
            // Set Firestore document ID
            role.Id = docRef.Id;  // Now the Id is a string
            return role;
        }


        private async Task AssignRoleToUser(string roleId, string userId)
        {
            if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(userId))
                throw new ArgumentException("RoleId and UserId must not be null or empty");

            var userRolesRef = _firestoreDb.Collection("UserRole");
            var userRole = new UserRole
            {
                RoleId = roleId,
                UserId = userId
            };

            await userRolesRef.AddAsync(userRole);
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null) return new LoginResponse(false,"Model is empty");

            var findTokenRef = _firestoreDb.Collection("RefreshTokenInfo");
            var findTokenQuery = findTokenRef.WhereEqualTo("RToken",token.RToken);
            var findTokenSnapshot = await findTokenQuery.GetSnapshotAsync();
            if (findTokenSnapshot.Documents.Count == 0)
                return new LoginResponse(false,"RefreshToken is required");  // No token found

            // Get the first user role (assuming one role per user)
            var findTokenDoc = findTokenSnapshot.Documents.First();
            var findTokenData = findTokenDoc.ConvertTo<RefreshTokenInfo>();

            var usersRef = _firestoreDb.Collection("ApplicationUser").Document(findTokenData.UserId);
            var userSnapshot = await usersRef.GetSnapshotAsync();

            // Check if the user exists
            if (!userSnapshot.Exists)
                return new LoginResponse(false, "Refresh token could not be generated because user not found");

            var user = userSnapshot.ConvertTo<ApplicationUser>();

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.RoleId);
            string jwtToken = GenerateToken(user,roleName);
            string newRefreshToken = GenerateRefreshToken();

            // Update the existing refresh token document with the new token
            var updateData = new Dictionary<string, object>
            {
                { "RToken", newRefreshToken },
                { "CreatedAt", Timestamp.FromDateTime(DateTime.UtcNow) },
                { "UpdatedAt", Timestamp.FromDateTime(DateTime.UtcNow) }
            };

            // Apply the update to Firestore
            await findTokenDoc.Reference.UpdateAsync(updateData);

            // Return a successful response with the updated tokens
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, newRefreshToken);
        }
    }
}
