using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StorageService.Models;
using StorageService.Services;

namespace StorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;
        private readonly UserManager<User> _userManager;

        public FilesController(IFileRepository fileRepository, UserManager<User> userManager)
        {
            _fileRepository = fileRepository;
            _userManager = userManager;
        }

        [HttpPost(nameof(Upload))]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] bool isPublic)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (user is null)
                return Unauthorized();

            var result = await _fileRepository.UploadFileAsync(file, user, isPublic);

            if (result == FileState.UserVolumeLimit)
                return new JsonResult(new { message = "Error" }, HttpStatusCode.BadRequest);

            return new JsonResult(new { message = "File Uploaded Sucessfully!" });
        }

        [HttpGet(nameof(Download))]
        public async Task<IActionResult> Download(string filePath)
        {
            var file = await _fileRepository.GetFileByRelativePathAsync(filePath);
            if (file is null)
                return NotFound();

            if (!file.IsPublic)
            {
                if (User.Identity == null) 
                    return new ForbidResult();

                var user = await _userManager.Users.Include(a => a.Files)
                    .FirstOrDefaultAsync(a => a.Files.Contains(file));

                if (!user.UserName.Equals(User.Identity.Name))
                    return new ForbidResult();
            }

            var path = _fileRepository.GetFilePath(file.Path);
            var contentType = _fileRepository.GetFromFile(file.FileName);

            return File(await System.IO.File.ReadAllBytesAsync(path), contentType, file.FileName, true);
        }
    }
}
