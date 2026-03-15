using AngularNetBase.Identity.Entities;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Identity.Services
{
    public class UserProfileReader : IUserProfileReader
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileReader(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> GetFirstNameAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.FirstName;
        }
    }
}
