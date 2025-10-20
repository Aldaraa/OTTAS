using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentFeature.BulkDownloadDepartment;
using Microsoft.EntityFrameworkCore;
using tas.Domain.Entities;
using Newtonsoft.Json;
using tas.Domain.Enums;
using System.Drawing;
using tas.Application.Features.DepartmentFeature.BulkUploadDepartment;
using tas.Application.Repositories;
using tas.Application.Features.DepartmentFeature.BulkDownloadDepartmentEmployees;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees;
using tas.Application.Features.DepartmenteFeature.BulkUploadPreviewDepartmentEmployees;
using tas.Application.Features.DepartmentFeature.BulkUploadDepartmentEmployees;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace tas.Persistence.Repositories
{
    public partial class DepartmentRepository
    {
        #region BulkEmployeeUpdate


        public async Task BulkRequestEmployeesUpload(BulkUploadDepartmentEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                request.BulkDepartmentFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var extracIds = new List<string>();
                    extracIds.Add("Department");

                    var BulkUploadEmployeess = _bulkImportExcelService.GetList<BulkUploadDepartmentEmployees>(package.Workbook.Worksheets[0], extracIds);

                    foreach (var item in BulkUploadEmployeess.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateDepartmentEmployees(item);
                            if (validation.validationStatus)
                            {
                                var departmentName = item.Department;
                                var Id = item.Id;

                                var currentDepartment =await Context.Department.AsNoTracking().Where(x=>x.Name == departmentName).FirstOrDefaultAsync();
                                if (currentDepartment != null)
                                {
                                    if (currentDepartment.Name != "ALL")
                                    {
                                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                                        if (currentEmployee != null)
                                        {
                                            currentEmployee.DepartmentId = currentDepartment.Id;
                                            currentEmployee.DateUpdated = DateTime.Now;
                                            currentEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                            Context.Employee.Update(currentEmployee);
                                        }
                                    }

                                }



                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }

        #endregion


        #region BulkEmployeeUploadPreview


        public async Task<BulkUploadPreviewDepartmentEmployeesResponse> BulkRequestEmployeesPreview(BulkUploadPreviewDepartmentEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkUploadPreviewDepartmentEmployeesResponse();
            var returnDataFailedRows = new List<DepartmentEmployeesBulkFailedRow>();
            int UpdatedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkDepartmentFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet

                    var extracIds = new List<string>();
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadDepartmentEmployees>(package.Workbook.Worksheets[0], extracIds);
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateDepartmentEmployees(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new DepartmentEmployeesBulkFailedRow
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

        private async Task<ValidateBulkDepartmentRow> ValidateUpdateDepartmentEmployees(BulkUploadDepartmentEmployees item)
        {
            var Id = item.Id;
            var Fitstname = item.Firstname;
            var Lastname = item.Lastname;
            var DepartmentName = item.Department;
            var returnData = new ValidateBulkDepartmentRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Employee is an invalid value");
            }
            var currentDepartment =await Context.Department.AnyAsync(x => x.Name == DepartmentName);
            if (!currentDepartment)
            {
                errormessages.Add("Department is not found");
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


        #region EmployeesDownload
        public async Task<BulkDownloadDepartmentEmployeesResponse> BulkRequestDownloadEmployees(BulkDownloadDepartmentEmployeesRequest request, CancellationToken cancellationToken)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetEmployee = package.Workbook.Worksheets.Add("Employee");
                worksheetEmployee.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);


                worksheetEmployee.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var worksheetDepartment = package.Workbook.Worksheets.Add("Department");
                var worksheetDepartmentChangeData = await ModifyDepartmentSheet(worksheetDepartment);
                worksheetDepartment = worksheetDepartmentChangeData.sheet;

                List<string> ActionModes = new List<string> { "NONE", "UPDATE" };

                string ActionModesList = string.Join(",", ActionModes);

                worksheetEmployee.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetEmployee.Cells["A1:G1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = await Context.Employee.AsNoTracking().Where(x => x.DepartmentId == request.DepartmentId && x.Active == 1).ToListAsync();



                worksheetEmployee.Cells[1, 1].Value = "Mode";
                worksheetEmployee.Cells[1, 2].Value = "Id";
                worksheetEmployee.Cells[1, 3].Value = "SAPID";
                worksheetEmployee.Cells[1, 4].Value = "ADAccount";
                worksheetEmployee.Cells[1, 5].Value = "Lastname";
                worksheetEmployee.Cells[1, 6].Value = "Firstname";
                worksheetEmployee.Cells[1, 7].Value = "Department";




                var CostCodeValidation = worksheetEmployee.DataValidations.AddListValidation($"G2:G{entities.Count() + 1}");
                CostCodeValidation.Formula.ExcelFormula = $"Department!$A$2:$A${worksheetDepartmentChangeData.sheetDatas.Count() + 1}";


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
                    worksheetEmployee.Cells[row, 7].Value = $"{worksheetDepartmentChangeData.sheetDatas.FirstOrDefault(x => x.Id == entity.DepartmentId)?.Description}";


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

                return new BulkDownloadDepartmentEmployeesResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }
        }

        private async Task<ModfySheetReturnData> ModifyDepartmentSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Department.Select(x => new { x.Id, x.Name }).ToListAsync();
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
                sheet.Cells[row, 1].Value = $"{entity.Name}";

                returnData.Add(new SheetData { Id = entity.Id, Description = entity.Name });
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


        #region BulkDonwload

        public async Task<BulkDownloadDepartmentResponse> BulkRequestDownload(BulkDownloadDepartmentRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        

            using (var package = new ExcelPackage())
            {
                var worksheetDepartment = package.Workbook.Worksheets.Add("Department");
                var worksheetParentDepartment = package.Workbook.Worksheets.Add("ParentDepartment");
                worksheetDepartment.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);
                worksheetParentDepartment.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);
                var worksheetParentDepartmentChangeData = await ModifyParentDepartmentSheet(worksheetParentDepartment);
                worksheetParentDepartment = worksheetParentDepartmentChangeData.sheet;

                worksheetDepartment.Workbook.CalcMode = ExcelCalcMode.Automatic;
                List<string> ActionModes = new List<string> { "NONE", "ADD", "UPDATE", "DELETE" };

                string ActionModesList = string.Join(",", ActionModes);
                worksheetDepartment.OutLineSummaryBelow = true;
                worksheetDepartment.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetDepartment.Cells["A1:D1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                var data =await Context.Department
                    .Where(x => request.DepartmentIds.
                    Contains(x.Id))
                    .Select(x => new { x.Id, x.Name, x.ParentDepartmentId }).ToListAsync();

                var allDeps = await Context.Department.AsNoTracking().ToListAsync();

    


                worksheetDepartment.OutLineSummaryBelow = true;

                worksheetDepartment.Cells[1, 1].Value = "Mode";
                worksheetDepartment.Cells[1, 2].Value = "Id";
                worksheetDepartment.Cells[1, 3].Value = "Name";
                worksheetDepartment.Cells[1, 4].Value = "ParentDepartment";

                var validation = worksheetDepartment.DataValidations.AddListValidation($"A2:A{data.Count() + 1}");
                validation.ShowErrorMessage = true;
                validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                validation.ErrorTitle = "An invalid value was entered";
                validation.Error = "Select a value from the list";

                int row = 2;


                var ParentDepatmentValidation = worksheetDepartment.DataValidations.AddListValidation($"D2:D{data.Count() + 1}");

                ParentDepatmentValidation.Formula.ExcelFormula = $"ParentDepartment!$A$2:$A${worksheetParentDepartmentChangeData.sheetDatas.Count() + 1}";
              

                foreach (var item in ActionModes)
                {
                    validation.Formula.Values.Add(item);
                }


                foreach (var entity in data)
                {

                    worksheetDepartment.Cells[row, 1].Value = ActionModes[2];
                    worksheetDepartment.Cells[row, 2].Value = entity.Id;
                    worksheetDepartment.Cells[row, 3].Value = entity.Name;
                    worksheetDepartment.Cells[row, 4].Value = allDeps.FirstOrDefault(x => x.Id == entity.ParentDepartmentId)?.Name;

                    string cellRange = $"A{row}:D{row}";

                    var f = worksheetDepartment.ConditionalFormatting.AddExpression(worksheetDepartment.Cells[cellRange]);
                    f.Address = worksheetDepartment.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeAddColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"ADD\"";

                    f = worksheetDepartment.ConditionalFormatting.AddExpression(worksheetDepartment.Cells[cellRange]);
                    f.Address = worksheetDepartment.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"UPDATE\"";


                    f = worksheetDepartment.ConditionalFormatting.AddExpression(worksheetDepartment.Cells[cellRange]);
                    f.Address = worksheetDepartment.Cells[cellRange];

                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeDeleteColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"DELETE\"";
                    f.Address = worksheetDepartment.Cells[cellRange];

                    row++;
                }

                worksheetDepartment.Cells["A1:D1"].AutoFilter = true;
                worksheetDepartment.OutLineApplyStyle = true;
                worksheetDepartment.OutLineSummaryBelow = false;
                worksheetDepartment.Cells.AutoFitColumns();

                package.Save();


                return new BulkDownloadDepartmentResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }
        }


        private async Task<ModfySheetReturnData> ModifyParentDepartmentSheet(ExcelWorksheet sheet)
        {
            var RoomTypes = await Context.Department.Select(x => new { x.Id, x.Name }).ToListAsync();
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

            foreach (var entity in RoomTypes)
            {
                sheet.Cells[row, 1].Value = $"{entity.Name}";

                returnData.Add(new SheetData { Id = entity.Id, Description = entity.Name });
                row++;
            }
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }

        #endregion


        #region BulkUpload

    
        public async Task<BulkUploadDepartmentResponse> BulkRequestUpload(BulkUploadDepartmentRequest request, CancellationToken cancellationToken)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkUploadDepartmentResponse();
            var returnDataFailedRows = new List<DepartmentBulkFailedRow>();
            int AddedRows = 0;
            int UpdatedRows = 0;
            int DeletedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkDepartmentFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    List<string> extractIds = new List<string>();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; 
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadDepartment>(package.Workbook.Worksheets[0], extractIds);
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    var alldeps = await Context.Department.ToListAsync();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "ADD")
                        {
                            var validation = await ValidateAddDepartment(item);
                            if (validation.validationStatus)
                            {

                                var name = item.Name;
                                var parentDepartment = item.ParentDepartment;
                                var newData = new Department
                                {
                                    Active = 1,
                                    Name = item.Name,
                                    ParentDepartmentId =  alldeps.Where(x=> x.Name ==  parentDepartment).FirstOrDefault()?.Id,

                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                                };
                                Context.Department.Add(newData);
                                AddedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new DepartmentBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }

                        }
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateDepartment(item);
                            if (validation.validationStatus)
                            {
                                var name = item.Name;
                                int? Id =item.Id;
                                var currentDepartment = await Context.Department.FirstOrDefaultAsync(x => x.Id == Id.Value);
                                if (currentDepartment != null)
                                {
                                    currentDepartment.Name = name;
                                    currentDepartment.ParentDepartmentId = alldeps.Where(x=> x.Name == item.ParentDepartment).FirstOrDefault()?.Id;
                                    currentDepartment.DateUpdated = DateTime.Now;
                                    currentDepartment.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                                }
                                Context.Department.Update(currentDepartment);
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new DepartmentBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }
                        }
                        if (item.Mode == "DELETE")
                        {
                            var validation = await ValidateDeleteDepartment(item);
                            if (validation.validationStatus)
                            {
                                var Id = item.Id;
                                var deleteData = await Context.Department.FirstOrDefaultAsync(x => x.Id == Id);
                                deleteData.Active = 0;
                                Context.Department.Update(deleteData);
                                DeletedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new DepartmentBulkFailedRow
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

        private async Task<ValidateBulkCostCodeRow> ValidateAddDepartment(BulkUploadDepartment item)
        {
            string? name = item.Name;
            string? parentDepartment = item.ParentDepartment;
            var returnData = new ValidateBulkCostCodeRow();
            var errormessages = new List<string>();

            if (name == null)
            {
                errormessages.Add("Name is an invalid value.");
            }


            if (name.Length > 100)
            {
                errormessages.Add("Name is an invalid value.");
            }

            if (!string.IsNullOrEmpty(parentDepartment))
            {
                var parentDepartmentCheck =await Context.Department.AnyAsync(x => x.Name == parentDepartment);
                if (!parentDepartmentCheck)
                {
                    errormessages.Add("ParentDepartment is not found");
                }
            }


            var NameCheck =await Context.Department.AnyAsync(x => x.Name == name);
            if (NameCheck)
            {
                errormessages.Add("Name is duplicated");
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


        private async Task<ValidateBulkCostCodeRow> ValidateUpdateDepartment(BulkUploadDepartment item)
        {
            string? name = item.Name;
            string? parentDepartment = item.ParentDepartment;
            var returnData = new ValidateBulkCostCodeRow();
            var Id = item.Id;
            var errormessages = new List<string>();

            if (item.Id.HasValue)
            {
                if (name == null)
                {
                    errormessages.Add("Name is an invalid value.");
                }


                if (name.Length > 100)
                {
                    errormessages.Add("Name is an invalid value.");
                }
                var DepartmentCheck = await Context.Department.AnyAsync(x => x.Id == Id);
                if (!DepartmentCheck)
                {
                    errormessages.Add("Record  not found");
                }


                if (! string.IsNullOrWhiteSpace(parentDepartment))
                {
                    var parentDepartmentCheck =await Context.Department.AnyAsync(x => x.Name == parentDepartment);
                    if (!parentDepartmentCheck)
                    {
                        errormessages.Add("ParentDepartment is not found");
                    }
                }

            }
            else {
                errormessages.Add("Id is required");
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


        private async Task<ValidateBulkCostCodeRow> ValidateDeleteDepartment(BulkUploadDepartment item)
        {
            string? name = item.Name;
            string? parentDepartmentId = item.ParentDepartment;
            var returnData = new ValidateBulkCostCodeRow();
            var Id = item.Id;
            var errormessages = new List<string>();

            if (item.Id.HasValue)
            {
                var DepartmentCheck =await Context.Department.AnyAsync(x => x.Id == Id);
                if (!DepartmentCheck)
                {
                    errormessages.Add("Record  not found");
                }
                else {
                    var employeeDepartment =await Context.Employee.AnyAsync(x => x.DepartmentId == Id && x.Active == 1);
                    if (employeeDepartment)
                    {
                        errormessages.Add("This department cannot be deleted. People are registered here.");
                    }
                }
            }
            else
            {
                errormessages.Add("Id is required");
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
    }

    public sealed class ValidateBulkDepartmentRow
    {
        public bool validationStatus { get; set; }

        public List<string> ErrorMessage { get; set; }
    }


    public sealed class BulkUploadDepartment
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? ParentDepartment { get; set; }



    }

    public sealed class BulkUploadDepartmentEmployees
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? SAPID { get; set; }

        public string? ADAccount { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Department { get; set; }


    }

}
