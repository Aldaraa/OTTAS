
namespace tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees
{ 
    public sealed record BulkUploadPreviewCostCodeEmployeesResponse
    {

        public int? NoneRow { get; set; }
        public int? UpdateRow { get; set; }
        public List<CostCodeEmployeesBulkFailedRow> FailedRows { get; set; }




            
    }

    public sealed record CostCodeEmployeesBulkFailedRow
    { 
        public int? ExcelRowIndex { get; set; }

        public List<string> Error { get; set; }


    }

}