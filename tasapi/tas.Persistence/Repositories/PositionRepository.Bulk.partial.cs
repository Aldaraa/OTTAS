using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PositionFeature.BulkDownloadPosition;
using tas.Domain.Entities;
using tas.Application.Features.PositioneFeature.BulkUploadPosition;
using System.Drawing;
using tas.Domain.Enums;
using tas.Application.Features.PositioneFeature.BulkUploadPreviewPosition;
using tas.Application.Features.PositionFeature.BulkDownloadPositionEmployees;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCodeEmployees;
using tas.Application.Features.PositionFeature.BulkUploadPositionEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees;
using tas.Application.Features.PositionFeature.BulkUploadPreviewPositionEmployees;

namespace tas.Persistence.Repositories
{
    public partial class PositionRepository
    {

        #region EmployeeDownload
        public async Task<BulkDownloadPositionEmployeesResponse> BulkRequestEmployeeDownload(BulkDownloadPositionEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetEmployee = package.Workbook.Worksheets.Add("Employee");
                worksheetEmployee.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);


                var worksheetPosition = package.Workbook.Worksheets.Add("Position");
                worksheetPosition.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);



                worksheetEmployee.Workbook.CalcMode = ExcelCalcMode.Automatic;

                var worksheetPositionChangeData = await ModifyPositionSheet(worksheetPosition);
                worksheetPosition = worksheetPositionChangeData.sheet;


                List<string> ActionModes = new List<string> { "NONE", "UPDATE" };

                string ActionModesList = string.Join(",", ActionModes);

                worksheetEmployee.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetEmployee.Cells["A1:G1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = await Context.Employee.Where(x => x.PositionId == request.PositionId && x.Active == 1).ToListAsync();



                worksheetEmployee.Cells[1, 1].Value = "Mode";
                worksheetEmployee.Cells[1, 2].Value = "Id";
                worksheetEmployee.Cells[1, 3].Value = "SAPID";
                worksheetEmployee.Cells[1, 4].Value = "ADAccount";
                worksheetEmployee.Cells[1, 5].Value = "Lastname";
                worksheetEmployee.Cells[1, 6].Value = "Firstname";
                worksheetEmployee.Cells[1, 7].Value = "Position";

                var CostCodeValidation = worksheetEmployee.DataValidations.AddListValidation($"G2:G{entities.Count() + 1}");
                CostCodeValidation.Formula.ExcelFormula = $"Position!$A$2:$A${worksheetPositionChangeData.sheetDatas.Count() + 1}";


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
                    worksheetEmployee.Cells[row, 7].Value = worksheetPositionChangeData.sheetDatas.FirstOrDefault(x => x.Id == entity.PositionId)?.Description;


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

                return new BulkDownloadPositionEmployeesResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }
        }

        private async Task<ModfySheetReturnData> ModifyPositionSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Position.Select(x => new { x.Id, x.Description }).ToListAsync();
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
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData { Id = entity.Id, Description = entity.Description });
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


        #region EmployeesUpload
        public async Task BulkRequestEmployeesUpload(BulkUploadPositionEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var allPositionData = await Context.Position.AsNoTracking().ToListAsync();
            using (var stream = new MemoryStream())
            {
                request.BulkPositionFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var extracIds = new List<string>();
                    extracIds.Add("Position");

                    var BulkUploadCodeEmployeess = _bulkImportExcelService.GetList<BulkUploadPositionEmployees>(package.Workbook.Worksheets[0], extracIds);

                    foreach (var item in BulkUploadCodeEmployeess.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdatePositionEmployees(item);
                            if (validation.validationStatus)
                            {
                                var PositionData = item.Position;
                                var Id = item.Id;

                                var currentEmployee = await Context.Employee.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                                
                                if (currentEmployee != null)
                                {
                                    currentEmployee.PositionId = allPositionData.Where(x=> x.Description == PositionData).FirstOrDefault()?.Id;
                                    currentEmployee.DateUpdated = DateTime.Now;
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


        private async Task<ValidateBulkPositionRow> ValidateUpdatePositionEmployees(BulkUploadPositionEmployees item)
        {
            var Id = item.Id;
            var Fitstname = item.Firstname;
            var Lastname = item.Lastname;
            var positionData = item.Position;
            var returnData = new ValidateBulkPositionRow();
            var errormessages = new List<string>();

            if (Id == null || Id == 0)
            {
                errormessages.Add("Employee is an invalid value");
            }
            var currentPosition =await Context.Position.AnyAsync(x => x.Description == positionData);
            if (!currentPosition)
            {
                errormessages.Add("Position is not found");
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


        #region BulkEmployeeUploadPreview


        public async Task<BulkUploadPreviewPositionEmployeesResponse> BulkRequestEmployeesPreview(BulkUploadPreviewPositionEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkUploadPreviewPositionEmployeesResponse();
            var returnDataFailedRows = new List<PositionEmployeesBulkFailedRow>();
            int UpdatedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkPositionFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var extracIds = new List<string>();
                    extracIds.Add("Position");
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadPositionEmployees>(package.Workbook.Worksheets[0], extracIds);
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdatePositionEmployees(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new PositionEmployeesBulkFailedRow
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



        public async Task<BulkDownloadPositionResponse> BulkRequestDownload(BulkDownloadPositionRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetPosition = package.Workbook.Worksheets.Add("Position");
                worksheetPosition.Workbook.CalcMode = ExcelCalcMode.Automatic;
                List<string> ActionModes = new List<string> { "NONE", "ADD", "UPDATE", "DELETE" };

                string ActionModesList = string.Join(",", ActionModes);

                worksheetPosition.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetPosition.Cells["A1:D1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = Context.Position.Where(x => request.PositionIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.Code, x.Description });



                worksheetPosition.Cells[1, 1].Value = "Mode";
                worksheetPosition.Cells[1, 2].Value = "Id";
                worksheetPosition.Cells[1, 3].Value = "Code";
                worksheetPosition.Cells[1, 4].Value = "Description";

                var validation = worksheetPosition.DataValidations.AddListValidation($"A2:A{entities.Count() + 1}");
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

                    worksheetPosition.Cells[row, 1].Value = ActionModes[2];
                    worksheetPosition.Cells[row, 2].Value = entity.Id;
                    worksheetPosition.Cells[row, 3].Value = entity.Code;
                    worksheetPosition.Cells[row, 4].Value = entity.Description;



                    string cellRange = $"A{row}:D{row}";

                    var f = worksheetPosition.ConditionalFormatting.AddExpression(worksheetPosition.Cells[cellRange]);
                    f.Address = worksheetPosition.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeAddColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"ADD\"";

                    f = worksheetPosition.ConditionalFormatting.AddExpression(worksheetPosition.Cells[cellRange]);
                    f.Address = worksheetPosition.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"UPDATE\"";


                    f = worksheetPosition.ConditionalFormatting.AddExpression(worksheetPosition.Cells[cellRange]);
                    f.Address = worksheetPosition.Cells[cellRange];

                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeDeleteColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"DELETE\"";
                    f.Address = worksheetPosition.Cells[cellRange];

                    row++;
                }

                worksheetPosition.Cells["A1:D1"].AutoFilter = true;
                worksheetPosition.Cells.AutoFitColumns();
                package.Save();

                return new BulkDownloadPositionResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }


        }




        public async Task BulkRequestUpload(BulkUploadPositionRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                request.BulkPositionFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; 
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadPosition>(package.Workbook.Worksheets[0], new List<string>());

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "ADD")
                        {
                            var validation = await ValidateAddPosition(item);
                            if (validation.validationStatus)
                            {

                                var Code = item.Code;
                                var Description = item.Description;
                                var newData = new Position
                                {
                                    Active = 1,
                                    Code = Code,
                                    Description = Description,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                                };
                                Context.Position.Add(newData);
                            }

                        }
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdatePosition(item);
                            if (validation.validationStatus)
                            {
                                var Code = item.Code;
                                var Description = item.Description;
                                var Id = item.Id;
                                var newData = new Position
                                {
                                    Id = (int)Id,
                                    Active = 1,
                                    Code = Code,
                                    Description = Description,
                                    DateUpdated = DateTime.Now,
                                    UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id
                                };

                                Context.Position.Update(newData);
                            }
                        }
                        if (item.Mode == "DELETE")
                        {
                            var validation = await ValidateDeletePosition(item);
                            if (validation.validationStatus)
                            {
                                var Id = item.Id;
                                var deleteData = await Context.Position.FirstOrDefaultAsync(x => x.Id == Id);
                                deleteData.Active = 0;
                                Context.Position.Update(deleteData);
                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }


        private async Task<ValidateBulkPositionRow> ValidateAddPosition(BulkUploadPosition item)
        {
            var Code = item.Code;
            var Description = item.Description;
            var returnData = new ValidateBulkPositionRow();
            var errormessages = new List<string>();



            if (Description?.Length > 100)
            {
                errormessages.Add("Description is an invalid value.");
            }


            var CodeDescrCheck =await Context.Position.AnyAsync(x => x.Description == Description && x.Code == Code);
            if (CodeDescrCheck)
            {
                errormessages.Add("Code and Description  is duplicated");
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

        private async Task<ValidateBulkPositionRow> ValidateUpdatePosition(BulkUploadPosition item)
        {
            var Id = item.Id;
            var Code = item.Code;
            var Description = item.Description;
            var returnData = new ValidateBulkPositionRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Id is an invalid value");
            }
            if (Code == null)
            {
                errormessages.Add("Code is an invalid value.");
            }
            var CurrentPosition = Context.Position.Any(x => x.Id == Id);
            if (!CurrentPosition)
            {
                errormessages.Add("Position is not found");
            }

            var NumberCheck =await Context.Position.AnyAsync(x => x.Code == Code && x.Description == Description && x.Id != Id);
            if (NumberCheck)
            {
                errormessages.Add("Code and Description  is duplicated");
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

        private async Task<ValidateBulkRoomRow> ValidateDeletePosition(BulkUploadPosition item)
        {
            var Id = item.Id;

            var returnData = new ValidateBulkRoomRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Id is an invalid value");
            }

            var CurrentRoom =await Context.Position.AnyAsync(x => x.Id == Id);
            if (!CurrentRoom)
            {
                errormessages.Add("Position is not found");
            }

            if (Id != null && Id > 0)
            {
                var empStatsus =await Context.Employee.AnyAsync(x => x.Id == Id && x.PositionId == Id && x.Active == 1);
                if (empStatsus)
                {
                    errormessages.Add($"There is a person in the Position with {Id}. Cannot be deleted");
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

        #region BulkUploadPreview
        public async Task<BulkRequestUploadPreviewPositionResponse> BulkRequestUploadPreview(BulkRequestUploadPreviewPositionRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkRequestUploadPreviewPositionResponse();
            var returnDataFailedRows = new List<PositionBulkFailedRow>();
            int AddedRows = 0;
            int UpdatedRows = 0;
            int DeletedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkPositionFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadPosition>(package.Workbook.Worksheets[0], new List<string>());
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").ToList().Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "ADD")
                        {
                            var validation = await ValidateAddPosition(item);
                            if (validation.validationStatus)
                            {
                                AddedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new PositionBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }

                        }
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdatePosition(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new PositionBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }
                        }
                        if (item.Mode == "DELETE")
                        {
                            var validation = await ValidateDeletePosition(item);
                            if (validation.validationStatus)
                            {
                                DeletedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new PositionBulkFailedRow
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
    }


    public sealed class ValidateBulkPositionRow
    {
        public bool validationStatus { get; set; }

        public List<string> ErrorMessage { get; set; }
    }

    public sealed class BulkUploadPosition
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }


    }

    public sealed class BulkUploadPositionEmployees
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? SAPID { get; set; }

        public string? ADAccount { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Position { get; set; }


    }
}
