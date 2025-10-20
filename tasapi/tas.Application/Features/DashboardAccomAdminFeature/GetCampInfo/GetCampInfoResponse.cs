using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardAccomAdminFeature.GetCampInfo
{

    public sealed record GetCampInfoResponse
    {
        public List<GetCampInfoCampData> CampInfoCampData { get; set; }

        public List<GetCampInfoCampRoomData> CampInfoCampBedRoomData { get; set; }

        public List<GetCampInfoTotalRoomTypeData> CampInfoRoomTypeData { get; set; }


    }


    public sealed record GetCampInfoCampData
    { 
        public string? Camp { get; set; }

        public int? RoomQTY { get; set; }

        public int? BedQTY { get; set; }
    }


    public sealed record GetCampInfoCampRoomData
    {
        public string? Camp { get; set; }

        public  List<GetCampInfoCampRoomRoomBedData> RoomAndBed { get; set; }
    }


    public sealed record GetCampInfoCampRoomRoomBedData 
    {
        public string? RoomType { get; set; }

        public int? RoomQTY { get; set; }

        public int? BedQTY { get; set; }

        public int? Owner { get; set; }

        public int? OnSite { get; set; }




    }
    public sealed record GetCampInfoTotalRoomTypeData
    {
        public string? RoomType { get; set; }

        public int? TotalRoomQTY { get; set; }

        public int? TotalBedQTY { get; set; }
        public int? Owned { get; set; }
        public int? OnSite { get; set; }



    }





}
