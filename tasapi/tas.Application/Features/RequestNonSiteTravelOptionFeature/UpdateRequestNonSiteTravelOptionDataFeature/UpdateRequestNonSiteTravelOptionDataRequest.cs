using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestNonSiteTravelOptionDataFeature.UpdateRequestNonSiteTravelOptionData
{
    public sealed record UpdateRequestNonSiteTravelOptionDataRequest(int Id, string optionData, DateTime DueDate,decimal Cost) : IRequest;
}
