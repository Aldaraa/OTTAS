using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace tas.Application.Features.EmployeeFeature.ChangeEmployeeDataGroup
{
    public sealed record ChangeEmployeeDataGroupRequest(List<int> EmpIds, int GroupMasterId, int GroupDetailId) : IRequest;




}
