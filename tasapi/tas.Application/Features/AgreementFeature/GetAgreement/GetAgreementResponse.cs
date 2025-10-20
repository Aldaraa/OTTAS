using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AgreementFeature.GetAgreement
{
    public sealed record GetAgreementResponse
    {
        public int Id { get; set; }

        public string? AgreementText { get; set; }

    }
}
