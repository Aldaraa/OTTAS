using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateItineraryOption
{
    public sealed record UpdateItineraryOptionRequest(int DocumentId, string optionText, decimal AdditionalCost, string Comment) : IRequest;

}
