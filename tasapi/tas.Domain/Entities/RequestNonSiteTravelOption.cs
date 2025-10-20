using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RequestNonSiteTravelOption : BaseEntity
    {
        public int DocumentId { get; set; }

        public string? OptionData { get; set; }

        public int Selected { get; set; }

        public int SelectedUserId { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal? Cost { get; set; } = 0;


        public int? UpdateItinerary { get; set; }

        public string? Comment { get; set; }


        public int? OptionIndex { get; set;}






    }
}
