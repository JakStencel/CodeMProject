using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using noHRforIT.Business.Extensions;
using noHRforIT.Business.Helpers;
using noHRforIT.Business.Models;
using noHRforIT.Business.Models.AuthorizationModels;
using noHRforIT.Data;
using noHRforIT.Data.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace noHRforIT.Business.Services
{
    public interface IAuthService
    {
        Task<User> Authenticate(AuthenticateModel authUser);
        Task CreateJWTforUserAsync(UserDTO user);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
        Task<UserDTO> VerifyUser(AuthenticateModel model);
    }

    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<UserDTO> _userManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IMapper _mapper;

        public AuthService(IApplicationDbContext dbContext,
                           IOptions<AppSettings> appSettings,
                           IMapper mapper,
                           UserManager<UserDTO> userManager)
        {
            _dbContext = dbContext;
            _appSettings = appSettings;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<User> Authenticate(AuthenticateModel authUser)
        {
            var user = await VerifyUser(authUser);

            await CreateJWTforUserAsync(user);

            return _mapper.Map<User>(user).WithoutPassword();
        }

        public async Task<UserDTO> VerifyUser(AuthenticateModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return null;

            var user = await _userManager.FindByEmailAsync(model.Email);
            //_dbCOntext.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task CreateJWTforUserAsync(UserDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Value.JWTSecret);
            var expirationTime = DateTime.UtcNow.AddSeconds(_appSettings.Value.JWTLifeSpan);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(
                                    new SymmetricSecurityKey(key),
                                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.TokenExpirationTime = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds();

            //_dbContext.Users.Update(user);
            await _userManager.UpdateAsync(user);
            _dbContext.SaveChanges();
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");

            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
