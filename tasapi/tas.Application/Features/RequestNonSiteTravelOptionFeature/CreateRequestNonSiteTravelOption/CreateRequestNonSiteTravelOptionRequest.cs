using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.CreateRequestNonSiteTravelOption
{
    public sealed record CreateRequestNonSiteTravelOptionRequest(int DocumentId, List<CreateRequestNonSiteTravelOptionData> optionData, decimal? newcost) : IRequest;


    public sealed record CreateRequestNonSiteTravelOptionData
    {
        public    bool selected;
        public string optiontext;
        public DateTime? DueDate;
    }

}
