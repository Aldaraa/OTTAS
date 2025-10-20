using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfileMultiple
{
    public sealed record ReplaceProfileMultipleRequest(List<ReplaceProfileMultipleRequestEmployees> Employees, int EventId) : IRequest<List<ReplaceProfileMultipleResponse>>;


    public sealed record ReplaceProfileMultipleRequestEmployees
    {
       public int oldEmployeeId { get; set; }
        
       public int newEmployeeId { get; set; }
    }

}
