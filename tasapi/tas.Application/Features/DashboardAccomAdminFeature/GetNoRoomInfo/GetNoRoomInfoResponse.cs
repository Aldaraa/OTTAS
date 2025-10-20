using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetNoRoomInfo
{

    public sealed record GetNoRoomInfoResponse
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int WeekNumber { get; set; }
        public List<GetNoRoomInfoEmployeesPeopleType> PeopleType { get; set; }

        public List<GetNoRoomInfoEmployeesRoomType> RoomType { get; set; }

        public List<GetNoRoomInfoEmployeesGender> Gender { get; set; }

    }


    public sealed record GetNoRoomInfoEmployeesPeopleType
    { 
        public string? PeopleType { get; set; }

        public int? OnSiteEmployees { get; set; }


    }


    public sealed record GetNoRoomInfoEmployeesRoomType
    {
        public string? RoomType { get; set; }

        public int? OnSiteEmployees { get; set; }


    }


    public sealed record GetNoRoomInfoEmployeesGender
    {
        public string? Gender { get; set; }

        public int? OnSiteEmployees { get; set; }


    }






}
