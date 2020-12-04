using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        public async Task<IActionResult> Upload(IFormFile file, bool isPrivate)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);

            if (user is null)
                return Unauthorized();

            var result = await _fileRepository.UploadFileAsync(file, user, isPrivate);

            if (result == FileState.UserVolumeLimit)
                return new JsonResult(new {message = "Error"}, HttpStatusCode.BadRequest);
            
            return new JsonResult(new {message = "File Uploaded Sucessfully!"});
        }
    }
}
