using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StorageService.Models
{
    public class User : IdentityUser<int>
    {
        public int StorageSize { get; set; }

        public virtual List<File> Files { get; set; }
    }
}
