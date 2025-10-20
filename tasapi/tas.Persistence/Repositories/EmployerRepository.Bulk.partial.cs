using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Application.Features.EmployerFeature.BulkDownloadEmployer;
using tas.Application.Features.EmployerFeature.BulkUploadEmployer;
using Microsoft.EntityFrameworkCore;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployee;
using tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployer;
using tas.Application.Features.EmployerFeature.BulkDownloadEmployerEmployees;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees;
using tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployerEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCodeEmployees;
using tas.Application.Features.EmployerFeature.BulkUploadEmployerEmployees;

namespace tas.Persistence.Repositories
{
    public partial class EmployerRepository
    {

        #region EmployeeDownload
        public async Task<BulkDownloadEmployerEmployeesResponse> BulkRequestEmployeesDownload(BulkDownloadEmployerEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetEmployee = package.Workbook.Worksheets.Add("Employee");
                worksheetEmployee.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);

                var worksheetEmployer = package.Workbook.Worksheets.Add("Employer");
                worksheetEmployer.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);



                worksheetEmployee.Workbook.CalcMode = ExcelCalcMode.Automatic;

                var worksheetEmployerChangeData = await ModifyEmployerSheet(worksheetEmployer);
                worksheetEmployer = worksheetEmployerChangeData.sheet;

                List<string> ActionModes = new List<string> { "NONE", "UPDATE" };

                string ActionModesList = string.Join(",", ActionModes);

                worksheetEmployee.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetEmployee.Cells["A1:G1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = await Context.Employee.Where(x => x.EmployerId == request.EmployerId && x.Active == 1).ToListAsync();



                worksheetEmployee.Cells[1, 1].Value = "Mode";
                worksheetEmployee.Cells[1, 2].Value = "Id";
                worksheetEmployee.Cells[1, 3].Value = "SAPID";
                worksheetEmployee.Cells[1, 4].Value = "ADAccount";
                worksheetEmployee.Cells[1, 5].Value = "Lastname";
                worksheetEmployee.Cells[1, 6].Value = "Firstname";
                worksheetEmployee.Cells[1, 7].Value = "Employer";

                var CostCodeValidation = worksheetEmployee.DataValidations.AddListValidation($"G2:G{entities.Count() + 1}");
                CostCodeValidation.Formula.ExcelFormula = $"Employer!$A$2:$A${worksheetEmployerChangeData.sheetDatas.Count() + 1}";


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
                    worksheetEmployee.Cells[row, 7].Value = worksheetEmployerChangeData.sheetDatas.FirstOrDefault(x => x.Id == entity.EmployerId)?.Description;


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

                return new BulkDownloadEmployerEmployeesResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }
        }


        private async Task<ModfySheetReturnData> ModifyEmployerSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Employer.AsNoTracking().Select(x => new { x.Id, x.Description }).ToListAsync();
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

        #region BulkEmployeeUploadPreview


        public async Task<BulkEmployerUploadPreviewEmployeesResponse> BulkRequestPreviewEmployees(BulkEmployerUploadPreviewEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkEmployerUploadPreviewEmployeesResponse();
            var returnDataFailedRows = new List<EmployerBulkFailedRowEmployees>();
            int UpdatedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkEmployerFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var extracIds = new List<string>();
                    extracIds.Add("Employer");
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadEmployerEmployees>(package.Workbook.Worksheets[0], extracIds);
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateEmployerEmployees(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new EmployerBulkFailedRowEmployees
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


        private async Task<ValidateBulkEmployerRow> ValidateUpdateEmployerEmployees(BulkUploadEmployerEmployees item)
        {
            var Id = item.Id;
            var Fitstname = item.Firstname;
            var Lastname = item.Lastname;
            var EmployerData = item.Employer;
            var returnData = new ValidateBulkEmployerRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Employee is an invalid value");
            }
            var CurrentCostCode = await Context.Employer.AnyAsync(x => x.Description == EmployerData);
            if (!CurrentCostCode)
            {
                errormessages.Add("Employer is not found");
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

        #region BulkUploadEmployees


        public async Task BulkRequestEmployeesUpload(BulkUploadEmployerEmployeesRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var allEmployerData = await Context.Employer.AsNoTracking().ToListAsync();
            using (var stream = new MemoryStream())
            {
                request.BulkEmployerFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var extracIds = new List<string>();
                    extracIds.Add("Employer");

                    var BulkUploadCodeEmployeess = _bulkImportExcelService.GetList<BulkUploadEmployerEmployees>(package.Workbook.Worksheets[0], extracIds);

                    foreach (var item in BulkUploadCodeEmployeess.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateEmployerEmployees(item);
                            if (validation.validationStatus)
                            {
                                var EmployerData = item.Employer;
                                var Id = item.Id;

                                var currentEmployee = await Context.Employee.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                                if (currentEmployee != null)
                                {
                                    currentEmployee.EmployerId = allEmployerData.Where(x=> x.Description == EmployerData).FirstOrDefault()?.Id;
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



        #endregion

        #region BulkDownload

        public async Task<BulkDownloadEmployerResponse> BulkRequestDownload(BulkDownloadEmployerRequest request, CancellationToken cancellationToken)
        {
           ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetEmployer = package.Workbook.Worksheets.Add("Employer");
                worksheetEmployer.Workbook.CalcMode = ExcelCalcMode.Automatic;
                List<string> ActionModes = new List<string> { "NONE", "ADD", "UPDATE", "DELETE" };

                string ActionModesList = string.Join(",", ActionModes);
                worksheetEmployer.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);
                worksheetEmployer.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetEmployer.Cells["A1:D1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = Context.Employer.Where(x => request.EmployerIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.Code,  x.Description });



                worksheetEmployer.Cells[1, 1].Value = "Mode";
                worksheetEmployer.Cells[1, 2].Value = "Id";
                worksheetEmployer.Cells[1, 3].Value = "Code";
                worksheetEmployer.Cells[1, 4].Value = "Description";

                var validation = worksheetEmployer.DataValidations.AddListValidation($"A2:A{entities.Count() + 1}");
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

                    worksheetEmployer.Cells[row, 1].Value = ActionModes[2];
                    worksheetEmployer.Cells[row, 2].Value = entity.Id;
                    worksheetEmployer.Cells[row, 3].Value = entity.Code;
                    worksheetEmployer.Cells[row, 4].Value = entity.Description;



                    string cellRange = $"A{row}:D{row}";

                    var f = worksheetEmployer.ConditionalFormatting.AddExpression(worksheetEmployer.Cells[cellRange]);
                    f.Address = worksheetEmployer.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeAddColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"ADD\"";

                    f = worksheetEmployer.ConditionalFormatting.AddExpression(worksheetEmployer.Cells[cellRange]);
                    f.Address = worksheetEmployer.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"UPDATE\"";


                    f = worksheetEmployer.ConditionalFormatting.AddExpression(worksheetEmployer.Cells[cellRange]);
                    f.Address = worksheetEmployer.Cells[cellRange];

                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeDeleteColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"DELETE\"";
                    f.Address = worksheetEmployer.Cells[cellRange];

                    row++;
                }

                worksheetEmployer.Cells["A1:D1"].AutoFilter = true;
                worksheetEmployer.Cells.AutoFitColumns();

                package.Save();

                return new BulkDownloadEmployerResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }


        }




        //public async Task BulkRequestUpload(BulkUploadEmployerRequest request, CancellationToken cancellationToken)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (var stream = new MemoryStream())
        //    {
        //        request.BulkEmployerFile.CopyTo(stream);
        //        using (var package = new ExcelPackage(stream))
        //        {
        //            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
        //            var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadEmployer>(package.Workbook.Worksheets[0], new List<string>());

        //            foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
        //            {
        //                if (item.Mode == "ADD")
        //                {
        //                    var validation = await ValidateAddEmployer(item);
        //                    if (validation.validationStatus)
        //                    {

        //                        var Code = item.Code;
        //                        var Description = item.Description;
        //                        var Email = item.Email;
        //                        var newData = new Employer
        //                        {
        //                            Active = 1,
        //                            Code = Code,
        //                            Description = Description,
        //                            Email = Email,
        //                            DateCreated = DateTime.Now,
        //                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
        //                        };
        //                        Context.Employer.Add(newData);
        //                    }

        //                }
        //                if (item.Mode == "UPDATE")
        //                {
        //                    var validation = await ValidateUpdateEmployer(item);
        //                    if (validation.validationStatus)
        //                    {
        //                        var Code = item.Code;
        //                        var Description = item.Description;
        //                        var Email = item.Email;
        //                        var Id = item.Id;
        //                        var newData = new Employer
        //                        {
        //                            Id = (int)Id,
        //                            Active = 1,
        //                            Code = Code,
        //                            Description = Description,
        //                            Email = Email,
        //                            DateUpdated = DateTime.Now,
        //                            UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id
        //                        };

        //                        Context.Employer.Update(newData);
        //                    }
        //                }
        //                if (item.Mode == "DELETE")
        //                {
        //                    var validation = await ValidateDeleteEmployer(item);
        //                    if (validation.validationStatus)
        //                    {
        //                        var Id = item.Id;
        //                        var deleteData = await Context.Employer.FirstOrDefaultAsync(x => x.Id == Id);
        //                        deleteData.Active = 0;
        //                        Context.Employer.Update(deleteData);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    await Task.CompletedTask;
        //}


        private async Task<ValidateBulkEmployerRow> ValidateAddEmployer(BulkUploadEmployer item)
        {
            var Code = item.Code;
            var Description = item.Description;
            var returnData = new ValidateBulkEmployerRow();
            var errormessages = new List<string>();


            if (Description?.Length > 100)
            {
                errormessages.Add("Description is an invalid value.");
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

        private async Task<ValidateBulkEmployerRow> ValidateUpdateEmployer(BulkUploadEmployer item)
        {
            var Id = item.Id;
            var Code = item.Code;
            var Email = item.Email;
            var Description = item.Description;
            var returnData = new ValidateBulkEmployerRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Id is an invalid value");
            }
            if (Code == null)
            {
                errormessages.Add("Code is an invalid value.");
            }
            var CurrentEmployer =await Context.Employer.AnyAsync(x => x.Id == Id);
            if (!CurrentEmployer)
            {
                errormessages.Add("Employer is not found");
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

        private async Task<ValidateBulkRoomRow> ValidateDeleteEmployer(BulkUploadEmployer item)
        {
            var Id = item.Id;

            var returnData = new ValidateBulkRoomRow();
            var errormessages = new List<string>();
            if (Id == null || Id == 0)
            {
                errormessages.Add("Id is an invalid value");
            }

            var CurrentRoom =await Context.Employer.AnyAsync(x => x.Id == Id);
            if (!CurrentRoom)
            {
                errormessages.Add("Employer is not found");
            }

            if (Id != null && Id > 0)
            {
                var empStatsus =await Context.Employee.AnyAsync(x => x.Id == Id && x.EmployerId == Id && x.Active == 1);
                if (empStatsus)
                {
                    errormessages.Add($"There is a person in the Employer with {Id}. Cannot be deleted");
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


        #region BulkUpload
        public async Task BulkRequestUpload(BulkUploadEmployerRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                request.BulkEmployerFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadEmployer>(package.Workbook.Worksheets[0], new List<string>());

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "ADD")
                        {
                            var validation = await ValidateAddEmployer(item);
                            if (validation.validationStatus)
                            {

                                var Code = item.Code;
                                var Description = item.Description;
                                var Email = item.Email;
                                var newData = new Employer
                                {
                                    Active = 1,
                                    Code = Code,
                                    Description = Description,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                                };
                                Context.Employer.Add(newData);
                            }

                        }
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateEmployer(item);
                            if (validation.validationStatus)
                            {
                                var Code = item.Code;
                                var Description = item.Description;
                                var Email = item.Email;
                                var Id = item.Id;
                                var newData = new Employer
                                {
                                    Id = (int)Id,
                                    Active = 1,
                                    Code = Code,
                                    Description = Description,
                                    DateUpdated = DateTime.Now,
                                    UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id
                                };

                                Context.Employer.Update(newData);
                            }
                        }
                        if (item.Mode == "DELETE")
                        {
                            var validation = await ValidateDeleteEmployer(item);
                            if (validation.validationStatus)
                            {
                                var Id = item.Id;
                                var deleteData = await Context.Employer.FirstOrDefaultAsync(x => x.Id == Id);
                                deleteData.Active = 0;
                                Context.Employer.Update(deleteData);
                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }


        #endregion

        #region BulkuploadPreview
        //BulkUploadPreviewEmployerRequest
        public async Task<BulkEmployerUploadPreviewResponse> BulkRequestPreview(BulkEmployerUploadPreviewRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkEmployerUploadPreviewResponse();
            var returnDataFailedRows = new List<EmployerBulkFailedRow>();
            int AddedRows = 0;
            int UpdatedRows = 0;
            int DeletedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkEmployerFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadEmployer>(package.Workbook.Worksheets[0], new List<string>());
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "ADD")
                        {
                            var validation = await ValidateAddEmployer(item);
                            if (validation.validationStatus)
                            {
                                AddedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new EmployerBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }

                        }
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateEmployer(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new EmployerBulkFailedRow
                                {
                                    Error = validation.ErrorMessage,
                                    ExcelRowIndex = item.ExcelRowIndex,
                                });
                            }
                        }
                        if (item.Mode == "DELETE")
                        {
                            var validation = await ValidateDeleteEmployer(item);
                            if (validation.validationStatus)
                            {
                                DeletedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new EmployerBulkFailedRow
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

    public sealed class ValidateBulkEmployerRow
    {
        public bool validationStatus { get; set; }

        public List<string> ErrorMessage { get; set; }
    }


    public sealed class BulkUploadEmployer
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? Email { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }


    }

    public sealed class BulkUploadEmployerEmployees
    {
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? SAPID { get; set; }

        public string? ADAccount { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Employer { get; set; }


    }
}
