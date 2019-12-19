using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using noHRforIT.Business.Extensions;
using noHRforIT.Business.Helpers;
using noHRforIT.Business.Models;
using noHRforIT.Business.Models.AuthorizationModels;
using noHRforIT.Data;
using noHRforIT.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace noHRforIT.Business.Services
{
    public interface IUserService
    {
        Task Create(RegisterModel model);
        Task Delete(int id);
        IEnumerable GetAll();
        Task<User> GetUserById(string id);
        Task Update(UpdateModel model);
        Task<List<string>> GetAllRoles();
    }

    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly UserManager<UserDTO> _userManager;
        private readonly RoleManager<UserDTO> _roleManager;

        public UserService(IApplicationDbContext dbContext,
                           IMapper mapper,
                           IAuthService authService,
                           UserManager<UserDTO> userManager,
                           RoleManager<UserDTO> roleManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authService = authService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Create(RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                throw new Exception("Password is required");

            if (IsUsernameTaken(model.UserName) || IsEmailTaken(model.Email))
                throw new Exception($"Email or Username is already taken"); ;

            var user = _mapper.Map<UserDTO>(model);

            byte[] passwordHash, passwordSalt;
            _authService.CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            //_dbContext.Users.Add(user);
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable GetAll()
        {
            //return _mapper.Map<List<User>>(_dbContext.Users.ToList());
            return _mapper.Map<List<User>>(_userManager.Users.ToList());
        }

        public async Task<User> GetUserById(string id)
        {
            //return _mapper.Map<User>(await _dbContext.Users.SingleOrDefaultAsync(u => int.Parse(u.Id) == id));
            return _mapper.Map<User>(await _userManager.FindByIdAsync(id));
        }

        public async Task<List<string>> GetAllRoles()
        {
            return await Task.Run(() => _roleManager.Roles.Select(r => r.Role).ToList());
        }

        public async Task Update(UpdateModel model)
        {
            //users needs to be mapped before return
            throw new NotImplementedException();
        }

        public async Task Delete(int id)
        {
            //users needs to be mapped before return
            throw new NotImplementedException();
        }

        private bool IsEmailTaken(string email)
        {
            return _dbContext.UsersDTO.FirstOrDefault(u => u.Email == email) != null;
        }

        private bool IsUsernameTaken(string username)
        {
            return _dbContext.UsersDTO.FirstOrDefault(u => u.UserName == username) != null;
        }
    }
}
