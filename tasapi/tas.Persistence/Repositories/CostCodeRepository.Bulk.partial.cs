using Microsoft.Extensions.Configuration;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCode;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCode;
using Microsoft.EntityFrameworkCore;
using tas.Application.Features.RoomFeature.BulkUploadRoom;
using tas.Domain.Entities;
using System.Drawing;
using tas.Domain.Enums;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCode;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees;
using tas.Application.Features.EmployeeFeature.BulkDownloadEmployee;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees;

namespace tas.Persistence.Repositories
{
    public partial class CostCodeRepository
    {

        #region EmployeeDownload

        public async Task<BulkDownloadCostCodeEmployeesResponse> BulkRequestEmployeeDownload(BulkDownloadCostCodeEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetEmployee = package.Workbook.Worksheets.Add("Employee");
                worksheetEmployee.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);

                var worksheetCostCode = package.Workbook.Worksheets.Add("CostCode");
                worksheetCostCode.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);



                worksheetEmployee.Workbook.CalcMode = ExcelCalcMode.Automatic;

                var worksheetCostCodeChangeData = await ModifyCostCodeSheet(worksheetCostCode);
                worksheetCostCode = worksheetCostCodeChangeData.sheet;

                List<string> ActionModes = new List<string> { "NONE", "UPDATE" };

                string ActionModesList = string.Join(",", ActionModes);

                worksheetEmployee.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetEmployee.Cells["A1:G1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = await Context.Employee.Where(x => x.CostCodeId  ==request.CostCodeId && x.Active == 1).ToListAsync();



                worksheetEmployee.Cells[1, 1].Value = "Mode";
                worksheetEmployee.Cells[1, 2].Value = "Id";
                worksheetEmployee.Cells[1, 3].Value = "SAPID";
                worksheetEmployee.Cells[1, 4].Value = "ADAccount";
                worksheetEmployee.Cells[1, 5].Value = "Lastname";
                worksheetEmployee.Cells[1, 6].Value = "Firstname";
                worksheetEmployee.Cells[1, 7].Value = "CostCode";

                var CostCodeValidation = worksheetEmployee.DataValidations.AddListValidation($"G2:G{entities.Count() + 1}");
                CostCodeValidation.Formula.ExcelFormula = $"CostCode!$A$2:$A${worksheetCostCodeChangeData.sheetDatas.Count() + 1}";


                var validation = worksheetEmployee.DataValidations.AddListValidation($"A2:A{entities.Count() + 1}");
                validation.ShowErrorMessage = true;
                validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                validation.ErrorTitle = "An invalid value was entered";
                validation.Error = "Select a value from the list";

                //CostCode

                int row = 2;

                foreach (var item in ActionModes)
                {
                    validation.Formula.Values.Add(item);
                }

                foreach (var entity in entities)
                {

                    string cellRange = $"A{row}:G{row}";
                    worksheetEmployee.Cells[row, 1].Value = ActionModes[1];
                    worksheetEmployee.Cells[row, 2].Value = entity.Id;
                    worksheetEmployee.Cells[row, 3].Value = entity.SAPID;
                    worksheetEmployee.Cells[row, 4].Value = entity.ADAccount;
                    worksheetEmployee.Cells[row, 5].Value = entity.Lastname;
                    worksheetEmployee.Cells[row, 6].Value = entity.Firstname;
                    worksheetEmployee.Cells[row, 7].Value =worksheetCostCodeChangeData.sheetDatas.FirstOrDefault(x => x.Id == entity.CostCodeId)?.Description;


                    worksheetEmployee.Cells[row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheetEmployee.Cells[row, 7].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00aaad"));

                    var f = worksheetEmployee.ConditionalFormatting.AddExpression(worksheetEmployee.Cells[cellRange]);

                    f.Address = worksheetEmployee.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"UPDATE\"";


                    row++;
                }


                worksheetEmployee.Cells["A1:G1"].AutoFilter = true;
                worksheetEmployee.Cells.AutoFitColumns();
                worksheetEmployee.View.FreezePanes(2, 5);
                package.Save();

                return new BulkDownloadCostCodeEmployeesResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }
        }


        private async Task<ModfySheetReturnData> ModifyCostCodeSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.CostCodes.Select(x => new { x.Id, x.Number, x.Code, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Code} {entity.Description}";

                returnData.Add(new SheetData { Id = entity.Id, Description =  $"{entity.Number} {entity.Code} {entity.Description}" });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }



        #endregion



        #region BulkDownload

        public async Task<BulkDownloadCostCodeResponse> BulkRequestDownload(BulkDownloadCostCodeRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetCostCode = package.Workbook.Worksheets.Add("CostCode");
                worksheetCostCode.Workbook.CalcMode = ExcelCalcMode.Automatic;
                List<string> ActionModes = new List<string> { "NONE", "ADD", "UPDATE", "DELETE" };

                string ActionModesList = string.Join(",", ActionModes);
                worksheetCostCode.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);
                worksheetCostCode.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetCostCode.Cells["A1:E1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = Context.CostCodes.Where(x => request.CostCodeIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.Code, x.Number, x.Description });



                worksheetCostCode.Cells[1, 1].Value = "Mode";
                worksheetCostCode.Cells[1, 2].Value = "Id";
                worksheetCostCode.Cells[1, 3].Value = "Code";
                worksheetCostCode.Cells[1, 4].Value = "Number";
                worksheetCostCode.Cells[1, 5].Value = "Description";

                var validation = worksheetCostCode.DataValidations.AddListValidation($"A2:A{entities.Count() + 1}");
                validation.ShowErrorMessage = true;
                validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                validation.ErrorTitle = "An invalid value was entered";
                validation.Error = "Select a value from the list";

                int row = 2;

                foreach (var item in ActionModes)
                {
                    validation.Formula.Values.Add(item);
                }
                foreach (var entity in entities)
                {

                    worksheetCostCode.Cells[row, 1].Value = ActionModes[2];
                    worksheetCostCode.Cells[row, 2].Value = entity.Id;
                    worksheetCostCode.Cells[row, 3].Value = entity.Code;
                    worksheetCostCode.Cells[row, 4].Value = entity.Number;
                    worksheetCostCode.Cells[row, 5].Value = entity.Description;
                  


                    string cellRange = $"A{row}:F{row}";

                    var f = worksheetCostCode.ConditionalFormatting.AddExpression(worksheetCostCode.Cells[cellRange]);
                    f.Address = worksheetCostCode.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeAddColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"ADD\"";

                    f = worksheetCostCode.ConditionalFormatting.AddExpression(worksheetCostCode.Cells[cellRange]);
                    f.Address = worksheetCostCode.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"UPDATE\"";


                    f = worksheetCostCode.ConditionalFormatting.AddExpression(worksheetCostCode.Cells[cellRange]);
                    f.Address = worksheetCostCode.Cells[cellRange];

                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeDeleteColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"DELETE\"";
                    f.Address = worksheetCostCode.Cells[cellRange];

                    row++;
                }

                worksheetCostCode.Cells["A1:E1"].AutoFilter = true;
                worksheetCostCode.Cells.AutoFitColumns();

                package.Save();

                return new BulkDownloadCostCodeResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }


        }



        public async Task BulkRequestEmployeesUpload(BulkUploadCostCodeEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            var allCostCodeData = await Context.CostCodes.AsNoTracking().Select(x => new { x.Id, Description = $"{x.Code} {x.Description}" }).ToListAsync();

            using (var stream = new MemoryStream())
            {
                request.BulkCostCodeFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var extracIds = new List<string>();

                    var BulkUploadCodeEmployeess = _bulkImportExcelService.GetList<BulkUploadCostCodeEmployees>(package.Workbook.Worksheets[0], extracIds);

                    foreach (var item in BulkUploadCodeEmployeess.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateCostCodeEmployees(item);
                            if (validation.validationStatus)
                            {
                                var CostCodeData = item.CostCode;
                                var Id = item.Id;

                                var currentCostcode = allCostCodeData.Where(x => x.Description == CostCodeData).FirstOrDefault();
                       
                                var currentEmployee = await Context.Employee.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                                if (currentEmployee != null)
                                {
                                    currentEmployee.CostCodeId = currentCostcode?.Id;
                                    currentEmployee.DateUpdated= DateTime.Now;
                                    currentEmployee.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    Context.Employee.Update(currentEmployee);
                                }

                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }






        public async Task BulkRequestUpload(BulkUploadCostCodeRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                request.BulkCostCodeFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadCostCode>(package.Workbook.Worksheets[0],new List<string>());

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "ADD")
                        {
                            var validation = await ValidateAddCostCode(item);
                            if (validation.validationStatus)
                            {

                                var Code = item.Code;
                                var Description = item.Description;
                                var Number = item.Number;
                                var newData = new CostCode
                                {
                                    Active = 1,
                                    Code = Code,
                                    Description = Description,
                                    Number = Number,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                                };
                                Context.CostCodes.Add(newData);
                            }

                        }
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateCostCode(item);
                            if (validation.validationStatus)
                            {
                                var Code = item.Code;
                                var Description = item.Description;
                                var Number = item.Number;
                                var Id = item.Id;
                                var newData = new CostCode
                                {
                                    Id = (int)Id,
                                    Active = 1,
                                    Code = Code,
                                    Description = Description,
                                    Number = Number,
                                    DateUpdated = DateTime.Now,
                                    UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id
                                };
                               
                                Context.CostCodes.Update(newData);
                            }
                        }
                        if (item.Mode == "DELETE")
                        {
                            var validation = await ValidateDeleteCostCode(item);
                            if (validation.validationStatus)
                            {
                                var Id = item.Id;
                                var deleteData = await Context.CostCodes.FirstOrDefaultAsync(x => x.Id == Id);
                                deleteData.Active = 0;
                                Context.CostCodes.Update(deleteData);
                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }


        private async Task<ValidateBulkCostCodeRow> ValidateAddCostCode(BulkUploadCostCode item)
        {
            var Code = item.Code;
            var Number = item.Number;
            var Description = item.Description;
            var returnData = new ValidateBulkCostCodeRow();
            var errormessages = new List<string>();

            if (Number == null)
            {
                errormessages.Add("Number is an invalid value.");
            }


            if (Description?.Length > 100)
            {
                errormessages.Add("Description is an invalid value.");
            }


            var NumberCheck = Context.CostCodes.Any(x => x.Number == Number);
            if (NumberCheck)
            {
                errormessages.Add("Number  is duplicated");
            }

            if (errormessages.Count == 0)
            {
                returnData.validationStatus = true;
            }
            else
            {
                returnData.validationStatus = false;
                returnData.ErrorMessage = errormessages;
            }
            return returnData;
        }

        private async Task<ValidateBulkCostCodeRow> ValidateUpdateCostCodeEmployees(BulkUploadCostCodeEmployees item)
        {

            var allCostCodeData = await Context.CostCodes.AsNoTracking().Select(x => new {x.Id,Description = $"{x.Number} {x.Code} {x.Description}" }).ToListAsync();
            var Id = item.Id;
            var Fitstname = item.Firstname;
            var Lastname = item.Lastname;
            var CostCode = item.CostCode;
            var returnData = new ValidateBulkCostCodeRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Employee is an invalid value");
            }

            
            var CurrentCostCode = allCostCodeData.Any(x => x.Description == CostCode);
            if (!CurrentCostCode)
            {
                errormessages.Add("CostCode is not found");
            }

         
            if (errormessages.Count == 0)
            {
                returnData.validationStatus = true;
            }
            else
            {
                returnData.validationStatus = false;
                returnData.ErrorMessage = errormessages;
            }
            return returnData;
        }


        private async Task<ValidateBulkCostCodeRow> ValidateUpdateCostCode(BulkUploadCostCode item)
        {
            var Id = item.Id;
            var Code = item.Code;
            var Number = item.Number;
            var Description = item.Description;
            var returnData = new ValidateBulkCostCodeRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Id is an invalid value");
            }
            if (Code == null)
            {
                errormessages.Add("Code is an invalid value.");
            }
            var CurrentCostCode =await Context.CostCodes.AnyAsync(x => x.Id == Id);
            if (!CurrentCostCode)
            {
                errormessages.Add("CostCode is not found");
            }

            var NumberCheck =await Context.CostCodes.AnyAsync(x => x.Number == Number && x.Id != Id);
            if (NumberCheck)
            {
                errormessages.Add("Number  is duplicated");
            }

            if (errormessages.Count == 0)
            {
                returnData.validationStatus = true;
            }
            else
            {
                returnData.validationStatus = false;
                returnData.ErrorMessage = errormessages;
            }
            return returnData;
        }

        private async Task<ValidateBulkRoomRow> ValidateDeleteCostCode(BulkUploadCostCode item)
        {
            var Id = item.Id;

            var returnData = new ValidateBulkRoomRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Id is an invalid value");
            }

            var CurrentRoom =await Context.CostCodes.AnyAsync(x => x.Id == Id);
            if (!CurrentRoom)
            {
                errormessages.Add("CostCode is not found");
            }

            if (Id != null && Id > 0)
            {
                var empStatsus =await Context.Employee.AsNoTracking().AnyAsync(x => x.Id == Id && x.CostCodeId == Id && x.Active== 1);
                if (empStatsus)
                {
                    errormessages.Add($"There is a person in the costcode with {Id}. Cannot be deleted");
                }

            }


            if (errormessages.Count == 0)
            {
                returnData.validationStatus = true;
            }
            else
            {
                returnData.validationStatus = false;
                returnData.ErrorMessage = errormessages;
            }
            return returnData;
        }


        #endregion



        #region BulkuploadPreview
        public async Task<BulkUploadPreviewCostCodeResponse> BulkRequestPreview(BulkUploadPreviewCostCodeRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkUploadPreviewCostCodeResponse();
            var returnDataFailedRows = new List<CostCodeBulkFailedRow>();
            int AddedRows = 0;
            int UpdatedRows = 0;
            int DeletedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkCostCodeFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadCostCode>(package.Workbook.Worksheets[0], new List<string>());
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "ADD")
                        {
                            var validation = await ValidateAddCostCode(item);
                            if (validation.validationStatus)
                            {
                                AddedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new CostCodeBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }

                        }
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateCostCode(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new CostCodeBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }
                        }
                        if (item.Mode == "DELETE")
                        {
                            var validation = await ValidateDeleteCostCode(item);
                            if (validation.validationStatus)
                            {
                                DeletedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new CostCodeBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }
                        }
                    }
                }
            }

            returnData.AddRow = AddedRows;
            returnData.UpdateRow = UpdatedRows;
            returnData.DeleteRow = DeletedRows;
            returnData.FailedRows = returnDataFailedRows;
            returnData.NoneRow = NoneRows;
            return returnData;
        }


        #endregion


        #region BulkEmployeeUploadPreview


        public async Task<BulkUploadPreviewCostCodeEmployeesResponse> BulkRequestEmployeesPreview(BulkUploadPreviewCostCodeEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkUploadPreviewCostCodeEmployeesResponse();
            var returnDataFailedRows = new List<CostCodeEmployeesBulkFailedRow>();
            int UpdatedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkCostCodeFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var extractIds = new List<string>();
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadCostCodeEmployees>(package.Workbook.Worksheets[0], extractIds);
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateCostCodeEmployees(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new CostCodeEmployeesBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }
                        }
                    }
                }
            }

            returnData.UpdateRow = UpdatedRows;
            returnData.FailedRows = returnDataFailedRows;
            returnData.NoneRow = NoneRows;
            return returnData;
        }

        #endregion


    }

    public sealed class ValidateBulkCostCodeRow
    {
        public bool validationStatus { get; set; }

        public List<string> ErrorMessage { get; set; }
    }


    public sealed class BulkUploadCostCode
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? Number { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }


    }


    public sealed class BulkUploadCostCodeEmployees
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? SAPID { get; set; }

        public string? ADAccount { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? CostCode { get; set; }


    }

}
