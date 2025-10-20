
namespace tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployee
{ 
    public sealed record BulkUploadPreviewEmployeeResponse
    {

        public int? NoneRow { get; set; }
        public int? AddRow { get; set; }

        public int? UpdateRow { get; set; }
        public int? DeleteRow { get; set; }
        
        public List<EmployeeBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record EmployeeBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public int? PersonId { get; set; }


        public string? Fullname { get; set; }

        public int? SAPID { get; set; }



        public List<string> Error { get; set; }


    }

}