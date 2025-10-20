using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Repositories
{

    public interface ICustomRoleProvider
    {
        Task<IEnumerable<string>> GetRolesAsync(string userIdentifier);
    }

}
