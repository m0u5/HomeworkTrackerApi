using HomeworkTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HomeworkTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager; 
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            string roleName = model.Role.ToLower();
            if (roleName!=UserRoles.User && roleName!=UserRoles.Teacher)
                return BadRequest("You can choose between Teacher and User roles.");//проверка выбора роли

            var user = new ApplicationUser
            {
                UserName = model.Login,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Выбор роли при регистрации
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);

                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }

                await _signInManager.SignInAsync(user, false);
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            // Проверяем, является ли входящая строка адресом электронной почты
            var isEmail = new EmailAddressAttribute().IsValid(model.UsernameOrEmail);

            ApplicationUser user;

            if (isEmail)
            {
                user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
            }

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return Unauthorized();
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            roleName = roleName.ToLower();
            // Fetch the user
            var user = await _userManager.FindByIdAsync(userId);

            // Check if user exists
            if (user == null)
            {
                return BadRequest("Invalid user ID");
            }

            // Check if role exists
            if (!(await _roleManager.RoleExistsAsync(roleName)))
            {
                return BadRequest("Invalid role name");
            }

            // Assign role to user
            var result = await _userManager.AddToRoleAsync(user, roleName);
            
            if (result.Succeeded)
            {                
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [Authorize(Roles = "admin")]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            // Fetch the user
            var user = await _userManager.FindByIdAsync(userId);

            // Check if user exists
            if (user == null)
            {
                return BadRequest("Invalid user ID");
            }

            // Delete user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.Select(u => new
            {
                u.Id,
                u.Email,
                u.UserName,
                Roles = _userManager.GetRolesAsync(u).Result
            }).ToListAsync();

            if (users == null)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
        

    }
}
