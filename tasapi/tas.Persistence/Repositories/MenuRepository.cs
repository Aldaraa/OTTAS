using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysMenuFeature.GetAllMenu;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class MenuRepository : BaseRepository<SysMenu>, IMenuRepository
    {
        public MenuRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public async Task<List<GetAllMenuResponse>> GetAllMenu(CancellationToken cancellationToken)
        {
            //var applications = await Context.SysApplication.ToListAsync(cancellationToken);

            var result = new List<GetAllMenuResponse>();

            //foreach (var application in applications)
            //{
            //    var menus = PopulateGetMenus(Context, application.Id, application.Name);
            //    result.Add(new GetAllMenuResponse
            //    {
            //        Id = application.Id,
            //        Name = application.Name,
            //        OrderIndex = application.OrderIndex,
            //        Route = application.Route,
            //        Menus = menus
            //    });
            //}

            return result;
        }


    }
}
