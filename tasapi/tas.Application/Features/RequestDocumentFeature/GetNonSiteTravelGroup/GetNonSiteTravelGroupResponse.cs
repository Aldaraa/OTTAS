
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelGroup
{
    public sealed record GetNonSiteTravelGroupResponse
    {
         public int  Id { get; set; }

         public string? Description { get; set; }


    }



}