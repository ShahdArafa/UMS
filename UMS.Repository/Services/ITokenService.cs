﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities.Identity;

namespace UMS.Repository.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager);
    }
}
