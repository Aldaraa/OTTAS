using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.BulkDownloadEmployee;
using tas.Application.Features.EmployeeFeature.BulkDownloadGroupEmployee;
using tas.Domain.Enums;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployer;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployee;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployeeGroup;
using tas.Application.Features.EmployeeFeature.BulkUploadEmployeeGroup;
using tas.Domain.Entities;
using Microsoft.AspNetCore.Components.Forms;
using tas.Application.Common.Exceptions;

namespace tas.Persistence.Repositories
{
    public partial class EmployeeRepository
    {
        #region BulkDownload


        public async Task<BulkDownloadGroupEmployeeResponse> BulkRequestGroupDownload(BulkDownloadGroupEmployeeRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {


                var groupEntities = await Context.GroupMaster.Where(x => x.Active == 1).OrderBy(x => x.OrderBy).ToListAsync();
                var entities = await Context.Employee.Where(x => request.EmpIds.Contains(x.Id) && x.Active == 1).ToListAsync();
                var validationAddedCells = new List<string>();

                var MasterSheets = new List<ModfySheetReturnData>();

                var worksheetEmployee = package.Workbook.Worksheets.Add("Employee");
                worksheetEmployee.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);
                worksheetEmployee.Row(1).Style.Font.Bold = true;
                worksheetEmployee.Cells[1, 1].Value = "Mode";
                worksheetEmployee.Cells[1, 2].Value = "Id";
                worksheetEmployee.Cells[1, 3].Value = "Lastname";
                worksheetEmployee.Cells[1, 4].Value = "Firstname";

                if (groupEntities.Count > 0)
                {
                    var currentColumnIndex = 4;
                    foreach (var item in groupEntities)
                    {
                        currentColumnIndex++;
                        var itemSheet = package.Workbook.Worksheets.Add(item.Description);

                        itemSheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
                        var itemSheetChangeData = await modifyGroupDetailSheet(itemSheet, item.Id);
                        itemSheet = itemSheetChangeData.sheet;
                        MasterSheets.Add(itemSheetChangeData);
                        worksheetEmployee.Cells[1, currentColumnIndex].Value = item.Description;

                    }





                }


                worksheetEmployee.Cells.AutoFitColumns();
                worksheetEmployee.View.FreezePanes(2, 5);
                foreach (var item in package.Workbook.Worksheets)
                {
                    if (item.Name != "Employee")
                    {
                        item.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);
                    }

                }

                List<string> ActionModes = new List<string> { "NONE", "UPDATE" };

                string ActionModesList = string.Join(",", ActionModes);
                var validation = worksheetEmployee.DataValidations.AddListValidation($"A2:A{entities.Count() + 1}");
                validation.ShowErrorMessage = true;
                validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                validation.ErrorTitle = "An invalid value was entered";
                validation.Error = "Select a value from the list";

                //CostCode

                int row = 2;
                var EmployeeGroupData = await (from member in Context.GroupMembers.Where(x => request.EmpIds.Contains(x.EmployeeId.Value))
                                               join groupDetail in Context.GroupDetail on member.GroupDetailId equals groupDetail.Id into groupDetailData
                                               from groupDetail in groupDetailData.DefaultIfEmpty()
                                               join groupMaster in Context.GroupMaster on member.GroupMasterId equals groupMaster.Id into groupMasterData
                                               from groupMaster in groupMasterData.DefaultIfEmpty()
                                               select new
                                               {
                                                   EmployeeId = member.EmployeeId,
                                                   MasterId = member.GroupMasterId,
                                                   DetailId = member.GroupDetailId,
                                                   DetailDescr = groupDetail.Description,
                                                   MasterDescr = groupMaster.Description,
                                                   MasterOrderKey = groupMaster.OrderBy
                                               }).ToListAsync();

                foreach (var item in ActionModes)
                {
                    validation.Formula.Values.Add(item);
                }
                foreach (var entity in entities)
                {
                    worksheetEmployee.Cells[row, 1].Value = ActionModes[1];
                    worksheetEmployee.Cells[row, 2].Value = entity.Id;
                    worksheetEmployee.Cells[row, 3].Value = entity.Lastname;
                    worksheetEmployee.Cells[row, 4].Value = entity.Firstname;

                    var employeeItemGroupData = EmployeeGroupData.Where(x => x.EmployeeId == entity.Id).OrderBy(x => x.MasterOrderKey).ToList();
                    var columnIndex = 5;

                    foreach (var groupItem in groupEntities)
                    {
                        var columnName = worksheetEmployee.Cells[1, columnIndex].Value;
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(columnName)))
                        {
                            var detailItem = EmployeeGroupData.Where(x => x.EmployeeId == entity.Id && x.MasterDescr == Convert.ToString(columnName)).FirstOrDefault();
                            worksheetEmployee.Cells[row, columnIndex].Value = detailItem != null ? $"{detailItem.DetailId}.{detailItem.DetailDescr}" : null;

                            var mSheet = MasterSheets.Where(x => x.sheet.Name == Convert.ToString(columnName)).FirstOrDefault();
                            if (mSheet != null)
                            {
                                if (mSheet.sheetDatas.Count > 0)
                                {
                                    string cellReference = GetColumnReference(columnIndex);
                                    if (validationAddedCells.IndexOf(cellReference) == -1)
                                    {
                                        validationAddedCells.Add(cellReference);
                                        var ItemValidation = worksheetEmployee.DataValidations.AddListValidation($"{cellReference}2:{cellReference}{entities.Count() + 1}");
                                        ItemValidation.Formula.ExcelFormula = $"'{columnName}'!$A$2:$A${mSheet.sheetDatas.Count() + 1}";
                                    }
                                }
                            }

                        }

                        columnIndex++;
                    }


                    string cellRange = $"A{row}:AJ{row}";

                    var f = worksheetEmployee.ConditionalFormatting.AddExpression(worksheetEmployee.Cells[cellRange]);

                    f = worksheetEmployee.ConditionalFormatting.AddExpression(worksheetEmployee.Cells[cellRange]);
                    f.Address = worksheetEmployee.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"UPDATE\"";

                    row++;
                }

                if (validationAddedCells.Count > 0)
                {
                    var headerCells = worksheetEmployee.Cells[$"A1:{validationAddedCells.Last()}1"];
                    headerCells.AutoFilter = true;
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Font.Size = 13;
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                    headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                }
                else
                {
                    var headerCells = worksheetEmployee.Cells[$"A1:D1"];
                    headerCells.AutoFilter = true;
                    headerCells.AutoFilter = true;
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Font.Size = 13;
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                    headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                }


                worksheetEmployee.Cells.AutoFitColumns();
                package.Save();

                return new BulkDownloadGroupEmployeeResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }

        }


        private async Task<ModfySheetReturnData> modifyGroupDetailSheet(ExcelWorksheet sheet, int groupId)
        {
            var groupDetailData = await Context.GroupDetail.Where(x => x.GroupMasterId == groupId).Select(x => new { x.Id, x.Description }).ToListAsync();
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
            foreach (var entity in groupDetailData)
            {
                sheet.Cells[row, 1].Value = $"{entity.Id}.{entity.Description}";

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



        #region BulkUploadPreview
        public async Task<BulkUploadPreviewEmployeeGroupResponse> BulkRequestUploadGroupPreview(BulkUploadPreviewEmployeeGroupRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkUploadPreviewEmployeeGroupResponse();
            var returnDataFailedRows = new List<EmployeeGroupBulkFailedRow>();
            int AddedRows = 0;
            int UpdatedRows = 0;
            int DeletedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkEmployeeFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet


                  var groupMasters = await  Context.GroupMaster.Where(x => x.Active == 1).ToListAsync();
                    var extracIds = new List<string>();

                    foreach (var group in groupMasters)
                    {
                        extracIds.Add(group.Description);
                    }

                    var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadEmployee>(package.Workbook.Worksheets[0], extracIds);
                    NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                                UpdatedRows++;
                           
                        }
                    }
                }
            }

            returnData.AddRow = AddedRows;
            returnData.UpdateRow = UpdatedRows;
            returnData.NoneRow = NoneRows;
            returnData.DeleteRow = DeletedRows;
            returnData.FailedRows = returnDataFailedRows;
            return returnData;
        }

        #endregion



        #region BulkUpload
        public async Task BulkRequestGroupUpload(BulkUploadEmployeeGroupRequest request, CancellationToken cancellationToken)
        {
            var errorCol = 0;
            var errRow = 0;
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = new MemoryStream())
                {


                    request.BulkEmployeeFile.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet

                        var groupMasters = await Context.GroupMaster.Where(x => x.Active == 1).ToListAsync();
                        var extracIds = new List<string>();

                        foreach (var group in groupMasters)
                        {
                            extracIds.Add(group.Description);
                        }

                        int columnCount = worksheet.Dimension.Columns;
                        int rowsCount = worksheet.Dimension.Rows;


                        var groupsMasters = await Context.GroupMaster.Where(x => x.Active == 1).ToListAsync();

                        for (int i = 2; i <= rowsCount; i++)
                        {
                            var EmployeeId = Convert.ToInt32(worksheet.Cells[i, 2].Value);
                            errRow = i;
                            for (int u = 5; u <= columnCount; u++)
                            {
                                string groupName = Convert.ToString(worksheet.Cells[1, u].Value);
                                var groupMasterData = groupsMasters.Where(x => x.Description == groupName).FirstOrDefault();
                                errorCol = u;
                                if (groupMasterData != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(Convert.ToString(worksheet.Cells[i, u].Value)))
                                    {
                                        var groupData = Convert.ToString(worksheet.Cells[i, u].Value).Split(".");
                                        if (groupData.Length > 0)
                                        {
                                            var currentData = await Context.GroupMembers.Where(x => x.EmployeeId == EmployeeId
                                            && x.GroupMasterId == groupMasterData.Id).FirstOrDefaultAsync();
                                            if (currentData != null)
                                            {
                                                if (currentData.GroupDetailId != Convert.ToInt32(groupData[0]))
                                                {
                                                    currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                                    currentData.DateUpdated = DateTime.Now;
                                                    currentData.GroupDetailId = Convert.ToInt32(groupData[0]);
                                                    Context.GroupMembers.Update(currentData);
                                                }

                                            }
                                            else
                                            {
                                                Context.GroupMembers.Add(new GroupMembers
                                                {
                                                    GroupMasterId = groupMasterData.Id,
                                                    EmployeeId = EmployeeId,
                                                    Active = 1,
                                                    GroupDetailId = Convert.ToInt32(groupData[0]),
                                                    DateCreated = DateTime.Now,
                                                    UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id
                                                });
                                            }

                                        }
                                    }
                                   

                                }


                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString());
            }
      

        }

        #endregion  


        static string GetColumnReference(int column)
        {
            // Convert column index to letters
            return ColumnLettersFromIndex(column);
        }

        static string ColumnLettersFromIndex(int index)
        {
            const int A = 'A';
            int dividend = index;
            string columnName = String.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(A + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }




    }



    public sealed class ValidateBulkEmployeeGroupRow
    {
        public bool validationStatus { get; set; }

        public List<string> ErrorMessage { get; set; }
    }

}
