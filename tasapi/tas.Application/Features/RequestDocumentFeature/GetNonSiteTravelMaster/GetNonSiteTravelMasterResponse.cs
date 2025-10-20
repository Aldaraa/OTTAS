
using MediatR;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelMaster
{
    public sealed record GetNonSiteTravelMasterResponse
    {
         public List<string> PaymentCondition { get; set; }

         public List<string> DocumentSearchCurrentStep { get; set; }

        public List<string> RequestDocumentType { get; set; }

        public List<string> RequestDocumentFavorTime { get; set; }



    }



}