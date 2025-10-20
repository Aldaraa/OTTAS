
namespace tas.Application.Features.PositionFeature.BulkUploadPreviewPositionEmployees
{ 
    public sealed record BulkUploadPreviewPositionEmployeesResponse
    {

        public int? NoneRow { get; set; }
        public int? UpdateRow { get; set; }
        public List<PositionEmployeesBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record PositionEmployeesBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}