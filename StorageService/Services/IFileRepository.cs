using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StorageService.DTOs;
using StorageService.Models;

namespace StorageService.Services
{
    public interface IFileRepository
    {
        Task<FileState> UploadFileAsync(IFormFile file, User user, bool isPublic);

        Task<File> GetFileByRelativePathAsync(string relativePath);

        string GetFilePath(string relativePath);

        string GetFromFile(string fileName);
    }

    public enum FileState
    {
        UserVolumeLimit,
        Success,
        Failure
    }
}
