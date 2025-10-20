using Application.Common.Utils;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        public async Task<string> ActionsTrends(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;

            string fields = "";
            foreach (var field in FieldNames)
            {
                fields += $", {field.FieldName} AS {field.FieldCaption}";

            }

            var employeeId = await GetUserId();

            var query = @$"SELECT e.Id UserId,
                            CONCAT(e.Firstname, ' ', e.Lastname) AS ActionEmployee,
                            COALESCE(sr.Name, 'No Role') AS Role,
                            COUNT(*) AS ActionCount,
                            rdh.CurrentAction
                        FROM RequestDocumentHistory rdh
                        LEFT JOIN Employee e 
                            ON rdh.ActionEmployeeId = e.Id
                        LEFT JOIN (
                            SELECT DISTINCT EmployeeId, RoleId 
                            FROM SysRoleEmployees
                        ) sre 
                            ON rdh.ActionEmployeeId = sre.EmployeeId
                        LEFT JOIN SysRole sr 
                            ON sre.RoleId = sr.Id
                        WHERE rdh.CurrentAction IN ('Completed', 'Approved', 'Declined')
                            AND rdh.DateCreated >= '[STARTDATE]' 
                            AND rdh.DateCreated < '[ENDDATE]' 
                        GROUP BY 
                            rdh.ActionEmployeeId,
                            rdh.CurrentAction,
                            e.Firstname, 
                            e.Lastname,
                            e.Id,
                            sr.Name
                        ORDER BY 
                            ActionCount DESC;
                        ";


            string whereCondition = "";

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



            query = query.Replace("[WHERECONDITION]", whereCondition);

            query = query.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}");


            return await SaveToActionsTrendsExcelFile(query, @"\Assets\GeneratedFiles\", "TAS Request Action trends", reportParams);





        }

        private async Task<string> SaveToActionsTrendsExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var dynamicList = await ExecuteReportQuery(queryData, reportName);

            // Check if the list has data to write to Excel

            using (var excelPackage = new ExcelPackage())
            {

                if (dynamicList.Count > 0)
                {

                    reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Report Executed : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                    reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = dynamicList.Count.ToString("N") });



                    // Add a new worksheet to the empty workbook
                    var headerProps = ((IDictionary<string, object>)dynamicList[0]).Keys;
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Data");
                    // Starting row for the data

                    //       worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
                    var titleCells = worksheet.Cells[1, 1];
                    titleCells.Value = reportName;
                    titleCells.Style.Font.Bold = true;
                    titleCells.Style.Font.Size = 12;
                    //titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
                    //titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
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

                                    var multiparamSheets = excelPackage.Workbook.Worksheets.Add(sheetSubName);

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



                    int column = 1;
                    worksheet.View.FreezePanes(row + 1, column);

                    foreach (var header in headerProps)
                    {
                        worksheet.Cells[row, column].Value = UsefulUtil.AddSpacesToSentence(header, true);
                        var headerCells = worksheet.Cells[row, column];
                        headerCells.Style.Font.Bold = true;
                        headerCells.Style.Font.Size = 13;
                        headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00aaad"));
                        headerCells.Style.Font.Color.SetColor(Color.White);
                        headerCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);


                        headerCells.AutoFilter = true;
                        column++;
                    }

                    // To set AutoFilter on the entire column range
                    worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
                    row++;

                    // Loop through each dynamic object and fill in the cells
                    foreach (var d in dynamicList)
                    {
                        column = 1;
                        foreach (var prop in (IDictionary<string, object>)d)
                        {
                            worksheet.Cells[row, column].Value = prop.Value;
                           
                            column++;
                        }

                        // worksheet.Cells[row, column].AutoFitColumns();
                        row++;
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    worksheet.PrinterSettings.Scale = 45;

                    // Configure print settings to fit the sheet on one page
                    worksheet.PrinterSettings.FitToPage = true;
                    worksheet.PrinterSettings.FitToWidth = 1;
                    worksheet.PrinterSettings.FitToHeight = 0;

                    // Save the new workbook
                    string fname = reportName + " " + Guid.NewGuid().ToString() + ".xlsx";

                    var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

                    var fileInfo = new FileInfo(realPath);
                    excelPackage.SaveAs(fileInfo);

                    return $"{filePath}\\{fname}";
                }
                else
                {

                    return string.Empty;

                }
            }
        }


    }
}
