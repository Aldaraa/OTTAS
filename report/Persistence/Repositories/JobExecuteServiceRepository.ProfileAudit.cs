using Application.Common.Utils;
using Application.Repositories;
using Domain.Entities;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using OfficeOpenXml.ConditionalFormatting;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository : BaseRepository<ReportJob>, IJobExecuteServiceRepository
    {


        public async Task<string> ProfileAudit(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;

            var userId = await GetUserId();

            var query = @$"SELECT 
                        CONVERT(VARCHAR, audit.DateCreated, 23) DateCreated,
                            audit.EmployeeId,
                            e1.SAP,
                            e1.Department,
                            e1.Employer,
                            e1.Position,
                            CONCAT(e1.Firstname, ' ', e1.Lastname) AS EmployeeName,
                            audit.NewValues,
                            audit.OldValues,
                            CONCAT(e.Firstname, ' ', e.Lastname) AS Username
                        FROM EmployeeAudit audit
                        LEFT JOIN Employee e
                            ON audit.UserId = e.Id
                        left JOIN GetReportProfileData({userId}) e1 ON audit.EmployeeId = e1.PersonNo
                        WHERE 
                            audit.DateCreated >= '[STARTDATE]'
                            AND audit.DateCreated <= '[ENDDATE]'
                        ORDER BY e1.Firstname";
            foreach (var item in reportParams)
            {

                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "StartDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        startDate = result;
                    }

                }
                if (ItemData.FieldName == "EndDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        endDate = result;
                    }

                }

            }

            query = query.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}");


            return await SaveProfileAuditToExcelFile(query, @"\Assets\GeneratedFiles\", "Profile Change Audit", reportParams);





        }




        private async Task<string> SaveProfileAuditToExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var data = await ExecuteReportQuery(queryData, reportName);

            // Check if the list has data to write to Excel
            if (data.Count == 0) return string.Empty;

            using (var package = new ExcelPackage())
            {


                reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Report Executed : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = data.Count.ToString("N") });



                // Add a new worksheet to the empty workbook
                var headerProps = ((IDictionary<string, object>)data[0]).Keys;
                var worksheet = package.Workbook.Worksheets.Add("Data");
                // Starting row for the data
                var titleCells = worksheet.Cells[1, 1];
                titleCells.Value = reportName;
                titleCells.Style.Font.Bold = true;
                titleCells.Style.Font.Size = 12;
                int row = 2;
                foreach (var item in reportParams)
                {
                    row++;
                    var paramCaptionCells = worksheet.Cells[row, 1];
                    paramCaptionCells.Value = $"{item.FieldCaption}";
                    paramCaptionCells.Style.Font.Size = 12;

                    var paramValueCells = worksheet.Cells[row, 2];
                    if (!string.IsNullOrWhiteSpace(item.FieldValueCaption))
                    {
                        if (item.FieldValueCaption.Length > 20)
                        {
                            if (item.FieldValueCaption.IndexOf(',') > -1)
                            {

                                var guid = Guid.NewGuid().ToString();
                                string sheetSubName = $"Parameters_{guid.Substring(0, 10)}";

                                var multiparamSheets = package.Workbook.Worksheets.Add(sheetSubName);

                                multiparamSheets.TabColor = ColorTranslator.FromHtml("#e37222");
                                int multiparamSheetsRow = 1;

                                foreach (var itemIndexData in item.FieldValueCaption.Split(","))
                                {
                                    multiparamSheets.Cells[multiparamSheetsRow, 1].Value = $"{itemIndexData}";
                                    multiparamSheetsRow++;
                                }
                                multiparamSheets.Workbook.CalcMode = ExcelCalcMode.Automatic;
                                multiparamSheets.Cells.AutoFitColumns();

                                paramValueCells.Hyperlink = new ExcelHyperLink($"#'{sheetSubName}'!A1", item.FieldValueCaption.Split(",")[0] + "...");


                            }
                            else
                            {
                                paramValueCells.Value = item.FieldValueCaption;
                            }
                        }
                        else
                        {
                            paramValueCells.Value = item.FieldValueCaption;
                        }



                    }
                    else
                    {
                        paramValueCells.Value = item.FieldValueCaption;
                    }


                    paramValueCells.Style.Font.Size = 12;
                    paramValueCells.Style.Font.Bold = true;
                    if (item.FieldName == "#MetaData")
                    {
                        paramValueCells.Style.Font.Italic = true;
                        paramValueCells.Style.Font.Bold = false;
                    }



                }


                row = row + 2;

                row++;

               // int row = 4; // Start adding data from row 2

                // Group data by EmployeeId and DateCreated
                var groupedByEmployee = data.GroupBy(d => d.EmployeeId);

                var skipfieldNames = new List<string>() { "PassportImage", "LoginEnabled", "Active",  };

                var evenRowColor = "#ffe699";
                var oddRowColor = "#afd2e9";
                var currentRowColor = "#afd2e9";

                foreach (var employeeGroup in groupedByEmployee)
                {
                    // Add EmployeeId header
                    worksheet.Cells[row, 1].Value = $"{employeeGroup.Key} {employeeGroup.FirstOrDefault()?.EmployeeName}";

                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1, row, 3].Merge = true;
                    worksheet.Cells[row, 1, row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 1, row, 3].Style.Font.Size = 10;
                    worksheet.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(currentRowColor));

                    worksheet.Cells[row, 1, row, 3].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Row(row).OutlineLevel = 1; // Set outline level for EmployeeGroup
                    worksheet.Row(row).Collapsed = true;
                    row++;

                    var groupedByDate = employeeGroup.GroupBy(d => d.DateCreated);
                    foreach (var dateGroup in groupedByDate)
                    {
                        // Add DateCreated header
                        worksheet.Cells[row, 1].Value = $"{dateGroup.Key} {dateGroup.FirstOrDefault()?.Username}";
                        worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 1].Style.Font.Bold = true;
                        worksheet.Cells[row, 1].Style.Font.Italic = true;

                        worksheet.Cells[row, 1, row, 3].Merge = true;
                        worksheet.Cells[row, 1, row, 3].Style.Font.Size = 10;
                        worksheet.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, 3].Style.Font.Color.SetColor(Color.Gray); // Font color
                        worksheet.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(currentRowColor));
                        worksheet.Cells[row, 1, row, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
                        worksheet.Row(row).OutlineLevel = 2; // Set outline level for EmployeeGroup
                        worksheet.Row(row).Collapsed = true;
                        // worksheet.Row(row).OutlineLevel = 2; // Set outline level for sub-grouping
                        row++;

                        var oldValueRows = row;
                        var newValueRows = row;
                        var FieldValuesRow = row;


                        // Add details for each record
                        foreach (var detail in dateGroup)
                        {

                            var oldValues = JsonSerializer.Deserialize<Dictionary<string, object>>(detail.OldValues);
                            var newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(detail.NewValues);



                            foreach (var kvp in oldValues)
                            {
                                if (!skipfieldNames.Contains(kvp.Key)) {

                                    ProfileMasterData mData = await GetMasterData(kvp.Key, Convert.ToString(kvp.Value));
                                    worksheet.Cells[FieldValuesRow, 1].Value = mData.ColumnCaption;
                                    worksheet.Cells[FieldValuesRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[FieldValuesRow, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(currentRowColor));
                                    FieldValuesRow++;


                                }

                            }


                            foreach (var kvp in oldValues)
                            {
                                if (!skipfieldNames.Contains(kvp.Key))
                                {

                                    DateTime cellDate;
                                    if (DateTime.TryParse(Convert.ToString(kvp.Value), out cellDate))
                                    {
                                        worksheet.Cells[oldValueRows, 2].Value = $"{cellDate.ToString("yyyy-MM-dd")}";
                                    }
                                    else
                                    {
                                        ProfileMasterData mData = await GetMasterData(kvp.Key, Convert.ToString(kvp.Value));
                                        worksheet.Cells[oldValueRows, 2].Value = mData.ColumnValue;
                                    }



                                    worksheet.Cells[oldValueRows, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[oldValueRows, 2].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(currentRowColor));
                                    worksheet.Cells[oldValueRows, 2].Style.Border.Left.Style = ExcelBorderStyle.Dotted;
                                    worksheet.Cells[oldValueRows, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    oldValueRows++;
                                }
                                
                               
                            }


                            foreach (var kvp in newValues)
                            {
                                if (!skipfieldNames.Contains(kvp.Key))
                                {
                                    DateTime cellDate;
                                    if (DateTime.TryParse(Convert.ToString(kvp.Value), out cellDate))
                                    {
                                        worksheet.Cells[newValueRows, 3].Value = $"→{cellDate.ToString("yyyy-MM-dd")}";
                                    }
                                    else {
                                        ProfileMasterData mData = await GetMasterData(kvp.Key, Convert.ToString(kvp.Value));
                                        worksheet.Cells[newValueRows, 3].Value = mData.ColumnValue == null ? "" : $"→{mData.ColumnValue}";
                                    }

             
         

                                    worksheet.Cells[newValueRows, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[newValueRows, 3].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(currentRowColor));
                                    worksheet.Cells[newValueRows, 3].Style.Border.Left.Style = ExcelBorderStyle.Dotted;
                                    worksheet.Cells[newValueRows, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    newValueRows++;

                                    worksheet.Row(row).OutlineLevel = 2; // Set outline level for EmployeeGroup
                                    worksheet.Row(row).Collapsed = true;

                                    row++;

                                }
                            }



                        }


                    }

                    currentRowColor = currentRowColor == evenRowColor ? oddRowColor : evenRowColor;

                }

                // Apply autofit to all columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                string fname = reportName + " " + Guid.NewGuid().ToString() + ".xlsx";

                var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

                var fileInfo = new FileInfo(realPath);
                package.SaveAs(fileInfo);

                return $"{filePath}\\{fname}";

            }
        }







   


    
    }
}
