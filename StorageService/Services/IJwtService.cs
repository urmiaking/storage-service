using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageService.Models;

namespace StorageService.Services
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}
