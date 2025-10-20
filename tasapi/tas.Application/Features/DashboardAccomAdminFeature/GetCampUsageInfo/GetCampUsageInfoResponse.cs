using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetCampUsageInfo
{

    public sealed record GetCampUsageInfoResponse
    {
        public List<GetCampUsageInfoCamp> Camp { get; set; }

        public List<GetCampUsageInfoSenior> Senior { get; set; }

        public List<GetCampUsageInfoGender> Gender { get; set; }


    }


    public sealed record GetCampUsageInfoCamp
    { 
        public string? Camp { get; set; }

        public int? ActualRoom { get; set; }

        public int? ActualBed { get; set; }

        public int? AssignedPeople { get; set; }

        public int? OnsitePeople { get; set; }

        public decimal? UsagePercentage { get; set; }

        public decimal? AvailableBedOnsite { get; set; }

        public decimal? VacantRoomOwner { get; set; }

        public int? VacantRoomNoOwner { get; set; }

        public int? VacantBedNoOwner { get; set; }





    }



    public sealed record GetCampUsageInfoSenior
    {
        public string? Category { get; set; }

        public int? ActualRoom { get; set; }

        public int? ActualBed { get; set; }

        public int? AssignedPeople { get; set; }

        public int? OnsitePeople { get; set; }

        public decimal? UsagePercentage { get; set; }

        public decimal? AvailableBedOnsite { get; set; }

        public decimal? VacantRoomOwner { get; set; }

        public int? VacantRoomNoOwner { get; set; }

        public int? VacantBedNoOwner { get; set; }
    }

    public sealed record GetCampUsageInfoGender
    {
        public string? Gender { get; set; }

        public int? ActualRoom { get; set; }

        public int? ActualBed { get; set; }

        public int? AssignedPeople { get; set; }

        public int? OnsitePeople { get; set; }

        public decimal? UsagePercentage { get; set; }

        public decimal? AvailableBedOnsite { get; set; }

        public decimal? VacantRoomOwner { get; set; }

        public int? VacantRoomNoOwner { get; set; }

        public int? VacantBedNoOwner { get; set; }
    }










}
