using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StorageService.Models
{
    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int Size { get; set; }
        public string Path { get; set; }
        public bool IsPublic { get; set; }
        public int UserId { get; set; }

        [NotMapped]
        public IFormFile FileObject { get; set; }
        public virtual User User { get; set; }
    }
}
