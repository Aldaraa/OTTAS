using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace tas.Application.Features.EmployeeFeature.ChangeEmployeeData
{
    public sealed record ChangeEmployeeDataRequest(ChangeEmployeeDataType changeDataType, List<ChangeEmployeeData> data) : IRequest;


    public enum ChangeEmployeeDataType
    { 
        CostCode = 0,
        Department = 1,
        Employer =  2,
        Position = 3,
        
    }


    public sealed record ChangeEmployeeData
    {
       public int employeeId { get; set; }
      public int DataId { get; set; }

       public DateTime startDate { get; set; }
        
       public DateTime? endDate { get; set; }

       public bool profileUpdate { get; set; } = false;
    }


    public record ChangeCostCodeRequest
    {
        public int CostCodeId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ProfileUpdate { get; set; }
    }

    public record ChangeDepartmentRequest
    {
        public int DepartmentId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ProfileUpdate { get; set; }
    }

    public record ChangePositionRequest
    {
        public int PositionId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ProfileUpdate { get; set; }
    }

    public record ChangeEmployerRequest
    {
        public int EmployerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ProfileUpdate { get; set; }
    }

}
