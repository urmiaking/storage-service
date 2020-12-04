using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StorageService.Data;
using StorageService.DTOs;
using StorageService.Models;
using File = StorageService.Models.File;

namespace StorageService.Services
{
    public class FileRepository : IFileRepository
    {
        private readonly string _filePath;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public FileRepository(AppDbContext context, UserManager<User> userManager)
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _context = context;
            _userManager = userManager;
        }

        public async Task<FileState> UploadFileAsync(IFormFile file, User user)
        {
            if (file is null)
                return FileState.Failure;

            var fileSizeInMb = file.Length / 1024;

            if (user.StorageSize < fileSizeInMb)
                return FileState.UserVolumeLimit;

            var fileSavePath = Path.Combine(_filePath, "files", user.UserName);

            var fileObject = new File
            {
                UserId = user.Id,
                FileName = file.FileName,
                Size = Convert.ToInt32(fileSizeInMb),
                IsPublic = false,
                Path = Path.Combine(user.UserName, file.FileName)
            };

            if (!Directory.Exists(fileSavePath))
            {
                Directory.CreateDirectory(fileSavePath);
            }

            await using (var fs = System.IO.File.Create(fileObject.Path))
            {
                await file.CopyToAsync(fs);
                await fs.FlushAsync();
            }

            await _context.Files.AddAsync(fileObject);
            await _context.SaveChangesAsync();

            user.StorageSize -= Convert.ToInt32(fileSizeInMb);
            var result = await _userManager.UpdateAsync(user);

            return FileState.Success;
        }
    }
}
