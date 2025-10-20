using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysMenuFeature.GetAllMenu
{
    public sealed record GetAllMenuResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Route { get; set; }
        public int? OrderIndex { get; set; }
        public List<Menu> Menus { get; set; }
    }

    public sealed record Menu
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Route { get; set; }
        public int? OrderIndex { get; set; }
        public string? ApplicationName { get; set; }
        public List<SubMenu> subMenus { get; set; }
    }

    public sealed record SubMenu
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Route { get; set; }
        public int? OrderIndex { get; set; }
    }
}