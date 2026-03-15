using System;
using System.Collections.Generic;
using System.Text;

namespace AngularNetBase.Practice.Services
{
    public interface IUserProfileReader
    {
        Task<string?> GetFirstNameAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
