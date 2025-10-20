using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace tas.Application.Features.EmployeeFeature.ChangeEmployeeLocation
{
    public sealed record ChangeEmployeeLocationRequest(List<ChangeEmployeeLocation> data) : IRequest;



    public sealed record ChangeEmployeeLocation
    {
       public int employeeId { get; set; }
       public int LocationId { get; set; }

       public DateTime startDate { get; set; }
        
    }
 

}
