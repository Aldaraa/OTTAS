using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheckMultiple
{
    public sealed record EmployeeDeActiveDateCheckMultipleRequest(List<EmployeeDeActiveDateCheckMultipleRequestData> data) : IRequest<List<EmployeeDeActiveDateCheckMultipleResponse>>;


   public sealed record EmployeeDeActiveDateCheckMultipleRequestData
    {
        public int? EmployeeId { get; set; }

        public DateTime? EventDate { get; set; }

        public int Index { get; set; }



    }




}