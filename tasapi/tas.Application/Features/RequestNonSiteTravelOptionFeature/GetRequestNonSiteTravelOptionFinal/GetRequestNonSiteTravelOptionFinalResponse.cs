using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOptionFinal
{
    public sealed record GetRequestNonSiteTravelOptionFinalResponse
    {
        public int Id { get; set; }

        public string? OptionData { get; set; }
        public int? Selected { get; set; }

        public int? SelectedUserId { get; set; }
        public string? SelectedUserName { get; set; }

        public string? SelectedUserTeam { get; set; }
        public DateTime? DateCreated { get; set; }

        public decimal? Cost { get; set; }

        public string? Comment { get; set; }

        public int? OptionIndex { get; set; }

        public string? Status { get; set; }

    }
}
