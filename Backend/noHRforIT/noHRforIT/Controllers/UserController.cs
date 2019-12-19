using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using noHRforIT.Business.Models.AuthorizationModels;
using noHRforIT.Business.Services;
using noHRforIT.Helpers;

namespace noHRforIT.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model)
        {
            if (!CheckIfModelIsValid(model))
            {
                return BadRequest(new { message = "User name or password is invalid" });
            }

            var user = await _authService.Authenticate(model);
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            if (CheckIfModelIsValid(model))
            {
                try
                {
                    await _userService.Create(model);
                    return Ok();
                }
                catch(Exception e)
                {
                    return BadRequest(new { message = e.Message });
                }
            }

            return BadRequest(new { message = "There must be user name and password" });
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.Developer))]
        public async Task<IActionResult> GetAll()
        {
            return Ok( await Task.Run(() =>_userService.GetAll()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUserId = User.Identity.Name;
            if (id != currentUserId && !User.IsInRole(nameof(Role.Admin)))
                return Forbid();

            var user = await _userService.GetUserById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userService.GetAllRoles();
            return Ok(roles);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateModel model)
        {
            if (CheckIfModelIsValid(model))
            {
                try
                {
                    await _userService.Update(model);
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(new { message = e.Message });
                }
            }

            return BadRequest(new { message = "There was a problem during updating user. Try again later" });
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.Delete(id);
            return Ok();
        }
    }
}