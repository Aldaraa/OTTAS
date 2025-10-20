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

        public async Task<string> SeatBlock(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;



            var flightUtilizationQuery = @$"SELECT name 'EventName', CONCAT(e.Firstname, ' ', e.Lastname) 'RequesterName', ve.StartDate 'InEventDate',
                    CONCAT(atin.Code, ' ',  tsin.Description) 'InFlightDescription',
                    CONCAT(
                        (ve.HeadCount * 100) / 
                        (CASE WHEN tsin.Seats = 0 OR tsin.Seats IS NULL THEN 1 ELSE tsin.Seats END),
                        '%'
                    ) AS '% In Capacity Pax count',
                    ve.EndDate 'OutEventDate',
                    CONCAT(atout.Code, ' ',  tsout.Description) 'OutFlightDescription',
                    CONCAT(
                        (ve.HeadCount * 100) / 
                        (CASE WHEN tsout.Seats = 0 OR tsout.Seats IS NULL THEN 1 ELSE tsout.Seats END),
                        '%'
                    ) AS '% Out Capacity Pax count',
                    DATEDIFF(day, ve.StartDate, ve.EndDate) 'Duration (Nights)',
                    ve.HeadCount,
                    CONCAT(e.Firstname, ' ', e.Lastname) 'Coordinator'
                    FROM VisitEvent ve 
                    left JOIN Employee e 
                    ON ve.UserIdCreated = e.Id

                    left JOIN TransportSchedule tsin ON ve.InScheduleId = tsin.id
                    left JOIN TransportSchedule tsout ON ve.OutScheduleId = tsout.id 
                    left join ActiveTransport atin ON tsin.ActiveTransportId = atin.Id
                    left join ActiveTransport atout ON tsout.ActiveTransportId = atout.Id
                    WHERE ve.StartDate >= '[STARTDATE]' AND ve.StartDate <= '[ENDDATE]' 
                    ORDER BY ve.StartDate;";


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

            flightUtilizationQuery = flightUtilizationQuery.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}");



            var exlist = new List<ExcelMultiSheet>();


            exlist.Add(new ExcelMultiSheet { queryData = flightUtilizationQuery.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}"), reportName = "Seat Block Report", sheetName = "Data" });

            //   return await SaveFlightUtilizationToExcelFile(query, @"\Assets\GeneratedFiles\", "TAS Flight Utilization", reportParams);

            return await SaveSeatBlockToExcelFile(exlist, @"\Assets\GeneratedFiles\", reportParams, "TAS Seat Block");


        }



        #region SaveToExcelFile
        private async Task<string> SaveSeatBlockToExcelFile(List<ExcelMultiSheet> sheetData, string filePath, List<ReportParam> reportParams, string reportFileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage())
            {
                var noformatColumn = new List<string> { "SAP #" };
                var hasData = false;

                foreach (var sheetItem in sheetData)
                {
                    string reportName = sheetItem.reportName;

                    var itemReportparam = reportParams.Where(x => x.FieldName != "#MetaData").ToList();

                    var dynamicList = await ExecuteReportQuery(sheetItem.queryData, reportName);
                    itemReportparam.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Report Executed : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                    itemReportparam.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = dynamicList.Count.ToString("N") });

                    if (dynamicList.Count > 0)
                    {

                        // Add a new worksheet to the empty workbook
                        var headerProps = ((IDictionary<string, object>)dynamicList[0]).Keys;
                        var worksheet = excelPackage.Workbook.Worksheets.Add(sheetItem.sheetName);
                        // Starting row for the data

                        //   worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
                        var titleCells = worksheet.Cells[1, 1];
                        titleCells.Value = reportName;
                        titleCells.Style.Font.Bold = true;
                        titleCells.Style.Font.Size = 12;
                        //titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
                        //titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
                        int row = 2;
                        foreach (var item in itemReportparam)
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


                                        //  paramValueCells.Hyperlink = new ExcelHyperLink($"#'{sheetname}'!A1", UriKind.RelativeOrAbsolute);
                                        paramValueCells.Hyperlink = new ExcelHyperLink($"#'{sheetSubName}'!A1", item.FieldValueCaption.Split(",")[0] + "...");

                                        multiparamSheets.Cells.AutoFitColumns();

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




                            paramValueCells.Style.Font.Size = 11;
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
                            var value = header;

                            if (value != null && value is DateTime)
                            {
                                worksheet.Column(column).Style.Numberformat.Format = "@";
                            }

                            worksheet.Cells[row, column].Value = noformatColumn.IndexOf(header) > -1 ? header : UsefulUtil.AddSpacesToSentence(header, true);
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

                                if (prop.Value is DateTime dateTimeValue)
                                {
                                    if (prop.Key == "InEventDate" || prop.Key == "OutEventDate" || prop.Key == "Date")
                                    {
                                        worksheet.Cells[row, column].Value = dateTimeValue;
                                        worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd";
                                    }
                                    else
                                    {
                                        worksheet.Cells[row, column].Value = dateTimeValue;
                                        worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                                    }
                                }
                                else
                                {
                                    worksheet.Cells[row, column].Value = prop.Value;
                                }
                                column++;
                            }
                            row++;
                        }
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        hasData = true;

                        worksheet.PrinterSettings.Scale = 45;

                        // Configure print settings to fit the sheet on one page
                        worksheet.PrinterSettings.FitToPage = true;
                        worksheet.PrinterSettings.FitToWidth = 1;
                        worksheet.PrinterSettings.FitToHeight = 0;

                    }
                }



                if (hasData)
                {

                    string timestamp = DateTime.Now.ToString("yyMMdd_HHmmss_fff"); // fff = millisecond
                    // Save the new workbook
                    string fname = reportFileName + "_" + timestamp + ".xlsx";

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


        #endregion

    }
}
