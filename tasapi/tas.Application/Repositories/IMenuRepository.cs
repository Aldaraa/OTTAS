using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysMenuFeature.GetAllMenu;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IMenuRepository : IBaseRepository<SysMenu>
    {
         Task<List<GetAllMenuResponse>> GetAllMenu(CancellationToken cancellationToken);
    }
}

