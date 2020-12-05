using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StorageService.DTOs;
using StorageService.Models;
using StorageService.Services;

namespace StorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(IJwtService jwtService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            var newUser = new User
            {
                UserName = user.Username,
                StorageSize = 100000,
                Email = "test@test.com"
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(Register), new { id = newUser.Id }, newUser);
            }

            return NotFound();
        }

        [HttpGet(nameof(Login))]
        public async Task<IActionResult> Login([FromQuery]LoginViewModel user)
        {
            var dbUser = await _userManager.FindByNameAsync(user.Username);

            if (dbUser is null)
                return NotFound();

            var canSignIn = await _signInManager.CheckPasswordSignInAsync(dbUser, user.Password,true);

            if (canSignIn.Succeeded)
            {
                return Ok($"Generated Jwt Token {_jwtService.Generate(dbUser)}");
            }

            if (canSignIn.IsLockedOut)
                return Unauthorized();

            return NotFound();
        }

        [HttpGet(nameof(StorageSize))]
        [Authorize]
        public async Task<IActionResult> StorageSize()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            return new JsonResult(new { storageSize = user.StorageSize});
        }

        [HttpGet(nameof(MyFiles))]
        [Authorize]
        public async Task<IActionResult> MyFiles()
        {
            var user = await _userManager.Users.Include(a => a.Files)
                .FirstOrDefaultAsync(a => a.UserName.Equals(User.Identity.Name));

            if (user.Files is null)
                return new JsonResult(new { message = "You have no files!" });

            var files = new Dictionary<string, string>();

            var i = 0;
            foreach (var file in user.Files)
            {
                var fileName = $"file {i}";
                files.Add(fileName, $"{HttpContext.Request.Scheme}://{Request.Host}/api/Files/Download?filePath={file.Path}");
                i++;
            }

            return new JsonResult(files);
        }
    }
}
