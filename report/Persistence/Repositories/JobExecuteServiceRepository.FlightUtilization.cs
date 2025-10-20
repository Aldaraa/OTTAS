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
        public async Task<string> FlightUtilization(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;



            var flightUtilizationQuery = @$"SELECT 
                            CONVERT(DATE, ts.EventDate) AS EventDate, 
                            DATEPART(WEEK, ts.EventDate) AS WeekNo,
                            DATENAME(WEEKDAY, ts.EventDate) AS WeekDay,
                            FORMAT(DATEADD(MINUTE, 
                                (CAST(LEFT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT) * 60) + 
                                CAST(RIGHT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT),
                                CAST(CONVERT(DATE, ts.EventDate) AS DATETIME)
                            ), 'hh:mm tt') ScheduleTime,

                            FORMAT(DATEADD(MINUTE, 
                                (CAST(LEFT(RIGHT('0000' + CAST(ts.RealETD AS VARCHAR(4)), 4), 2) AS INT) * 60) + 
                                CAST(RIGHT(RIGHT('0000' + CAST(ts.RealETD AS VARCHAR(4)), 4), 2) AS INT),
                                CAST(CONVERT(DATE, ts.EventDate) AS DATETIME)
                            ), 'hh:mm tt') DepartureTime,

                         CASE 
                                WHEN DATEDIFF(MINUTE, 
                                    DATEADD(MINUTE, 
                                        (CAST(LEFT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT) * 60) + 
                                        CAST(RIGHT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT),
                                        CAST(CONVERT(DATE, ts.EventDate) AS DATETIME)
                                    ), 
                                    DATEADD(MINUTE, 
                                        (CAST(LEFT(RIGHT('0000' + CAST(ts.RealETD AS VARCHAR(4)), 4), 2) AS INT) * 60) + 
                                        CAST(RIGHT(RIGHT('0000' + CAST(ts.RealETD AS VARCHAR(4)), 4), 2) AS INT),
                                        CAST(CONVERT(DATE, ts.EventDate) AS DATETIME)
                                    )
                                ) > 15 THEN 'Late'
        
                                WHEN DATEDIFF(MINUTE, 
                                    DATEADD(MINUTE, 
                                        (CAST(LEFT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT) * 60) + 
                                        CAST(RIGHT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT),
                                        CAST(CONVERT(DATE, ts.EventDate) AS DATETIME)
                                    ), 
                                    DATEADD(MINUTE, 
                                        (CAST(LEFT(RIGHT('0000' + CAST(ts.RealETD AS VARCHAR(4)), 4), 2) AS INT) * 60) + 
                                        CAST(RIGHT(RIGHT('0000' + CAST(ts.RealETD AS VARCHAR(4)), 4), 2) AS INT),
                                        CAST(CONVERT(DATE, ts.EventDate) AS DATETIME)
                                    )
                                ) < -1 THEN 'Early'

                                ELSE 'On Time'
                            END AS LateStatus,
                            c.Description Airline,
                            at.Code AircraftType,
                            ts.Seats FlightCapacity,
                            ISNULL(tr.cnt, 0) PassengerCount,
                            ISNULL(trns.cnt, 0) NoShow,
                            CONCAT(ISNULL(tr.cnt, 0) *100 / ts.seats, '%') '% Capacity percent',
                            at.Direction, 
                            CONCAT(fromLcoation.Code, '-', toLocation.Code) FlightDestination,
                                ISNULL(trgs.cnt, 0) GoShow,
                               CASE 
                                   WHEN at.Code LIKE 'AC %' THEN 'Aircraft change'
                                   WHEN at.Code LIKE 'TC %' THEN 'TC – Schedule updated'
                                   WHEN at.Code LIKE 'D %' THEN 'Schedule updated'
                                   WHEN at.Code LIKE 'AA %' THEN 'Air Ambulance'
                                   WHEN at.Code LIKE 'CH %' THEN 'Charter flight'
                                   ELSE 'Scheduled/Regular' 
                               END AS FlightType,
                          ts.Remark ReMarks
  
                        FROM 
                            TransportSchedule ts

                          left join ActiveTransport at ON ts.ActiveTransportId = at.Id
                          left JOIN Carrier c ON at.CarrierId = c.Id
                          inner JOIN TransportMode tm ON at.TransportModeId = tm.Id AND tm.Code = 'Airplane'
                          left JOIN (SELECT count(*) cnt, scheduleId FROM Transport T WHERE    CONVERT(DATE, t.EventDate) >= '[STARTDATE]'
                            AND CONVERT(DATE, t.EventDate) <= '[ENDDATE]' GROUP BY T.ScheduleId ) tr
                            ON ts.id = tr.ScheduleId
                          left JOIN (SELECT count(*) cnt, scheduleId FROM Transport T WHERE    CONVERT(DATE, t.EventDate) >= '[STARTDATE]' 
                            AND CONVERT(DATE, t.EventDate) <= '[ENDDATE]' AND t.NoShow = 1 GROUP BY T.ScheduleId ) trns
                            ON ts.id = trns.ScheduleId

                          left JOIN (SELECT count(*) cnt, scheduleId FROM Transport T WHERE    CONVERT(DATE, t.EventDate) >= '[STARTDATE]' 
                            AND CONVERT(DATE, t.EventDate) <= '[ENDDATE]' AND t.GoShow = 1 GROUP BY T.ScheduleId ) trgs
                            ON ts.id = trgs.ScheduleId
                          left JOIN Location fromLcoation ON at.fromLocationId = fromLcoation.Id
                          LEFT JOIN Location toLocation ON at.toLocationId = toLocation.Id

                        WHERE 
                            CONVERT(DATE, ts.EventDate) >= '[STARTDATE]' 
                            AND CONVERT(DATE, ts.EventDate) <= '[ENDDATE]'
	
                        ORDER BY 
                            ts.EventDate,
                            at.Code,
                            DATEADD(
                                MINUTE,
                                (CAST(LEFT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT) * 60) +
                                CAST(RIGHT(RIGHT('0000' + CAST(ts.ETD AS VARCHAR(4)), 4), 2) AS INT),
                                CAST(CONVERT(DATE, ts.EventDate) AS DATETIME)
                            );";



            var NoShowQuery = $@"SELECT
                        CONVERT(DATE, tns.EventDate) AS EventDate,
                        DATEPART(WEEK, tns.EventDate) AS WeekNo,
                        DATENAME(WEEKDAY, tns.EventDate) AS WeekDay,
                        e.SAPID, CONCAT(e.Firstname, ' ', e.Lastname) FullName,
                        emp.Description CompanyName,
                        rdd.Name Department,
                        p.Description Position,
                        CONCAT(tns.Description, ' ',  tns.Direction) Reason,
                        tns.Reason 'TRReased',
                    CONCAT(coordinator.Firstname, ' ', coordinator.Lastname) Coordinator
                     FROM TransportNoShow tns
                     left JOIN Employee e ON tns.EmployeeId = e.Id
                     left JOIN ReportDeparmentData rdd ON e.DepartmentId = rdd.Id
                     LEFT join Position p ON e.PositionId = p.Id
                     LEFT JOIN Employer emp ON e.employerId = emp.Id
                     LEFT JOIN Employee coordinator ON tns.UserIdCreated = coordinator.Id
                     WHERE
                     CONVERT(DATE, tns.EventDate) >= '[STARTDATE]'
                        AND CONVERT(DATE, tns.EventDate) <= '[ENDDATE]'

                    ORDER BY EventDate;";



            var GoShowQuery = $@"SELECT CONVERT(DATE, tns.EventDate) AS EventDate, 
                    DATEPART(WEEK, tns.EventDate) AS WeekNo,
                    DATENAME(WEEKDAY, tns.EventDate) AS WeekDay,
                    e.SAPID, CONCAT(e.Firstname, ' ', e.Lastname) FullName,
                    emp.Description CompanyName,
                    rdd.Name Department,
                    p.Description Position,
                    CONCAT(tns.Description,' ',  tns.Direction) Reason,
                    tns.Reason 'TRReased',
                CONCAT(coordinator.Firstname, ' ', coordinator.Lastname) Coordinator
                 FROM TransportGoShow tns
                 left JOIN Employee e ON tns.EmployeeId = e.Id 
                 left JOIN ReportDeparmentData rdd ON e.DepartmentId = rdd.Id
                 LEFT join Position p ON e.PositionId = p.Id
                 LEFT JOIN Employer emp ON e.employerId = emp.Id
                 LEFT  JOIN Employee coordinator ON tns.UserIdCreated = coordinator.Id 
                 WHERE
                 CONVERT(DATE, tns.EventDate) >= '[STARTDATE]' 
                    AND CONVERT(DATE, tns.EventDate) <= '[ENDDATE]'

                ORDER BY EventDate; ";



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



            List<ExcelMultiSheet> exlist = new List<ExcelMultiSheet>();
            exlist.Add(new ExcelMultiSheet()
            {
                queryData = flightUtilizationQuery.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}"),
                reportName = "Flight Ulilization Report",
                sheetName = "Flight Data"
            });
            exlist.Add(new ExcelMultiSheet()
            {
                queryData = NoShowQuery.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}"),
                reportName = "NoShow Report",
                sheetName = "NoShow"
            });
            exlist.Add(new ExcelMultiSheet()
            {
                queryData = GoShowQuery.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}"),
                reportName = "GoShow Report",
                sheetName = "GoShow"
            });


            return await SaveFlightUtilizationToExcelFile(exlist, @"\Assets\GeneratedFiles\", "TAS Flight Utilization", reportParams);





        }



        #region SaveToExcelFile



        private async Task<string> SaveFlightUtilizationToExcelFile(List<ExcelMultiSheet> sheetData, string filePath, string reportFileName, List<ReportParam> reportParams)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage())
            {
                var noformatColumn = new List<string> { "SAP #" };
                var hasData = false;

                foreach (var sheetItem in sheetData)
                {
                    string reportName = sheetItem.reportName;
                    var dynamicList = await ExecuteReportQuery(sheetItem.queryData, reportName);
                    reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Report Executed : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                    reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = dynamicList.Count.ToString("N") });

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
                                    if (prop.Key == "ToTravelDate" || prop.Key == "FromTravelDate" || prop.Key == "Date" || prop.Key == "LastTransportDate" || prop.Key == "EventDate")
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

                    // Save the new workbook
                    string fname = reportFileName + "_" + Guid.NewGuid().ToString() + ".xlsx";

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


        //private async Task<string> SaveFlightUtilizationToExcelFile(List<ExcelMultiSheet> sheetData, string filePath, string reportName, List<ReportParam> reportParams)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;





        //    var dynamicList = await ExecuteReportQuery(queryData, reportName);

        //    // Check if the list has data to write to Excel

        //    using (var excelPackage = new ExcelPackage())
        //    {

        //        if (dynamicList.Count > 0)
        //        {

        //            reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Report Executed : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
        //            reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = dynamicList.Count.ToString("N") });



        //            // Add a new worksheet to the empty workbook
        //            var headerProps = ((IDictionary<string, object>)dynamicList[0]).Keys;
        //            var worksheet = excelPackage.Workbook.Worksheets.Add("Data");
        //            // Starting row for the data

        //            //       worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
        //            var titleCells = worksheet.Cells[1, 1];
        //            titleCells.Value = reportName;
        //            titleCells.Style.Font.Bold = true;
        //            titleCells.Style.Font.Size = 12;
        //            //titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            //titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
        //            //titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
        //            int row = 2;
        //            foreach (var item in reportParams)
        //            {
        //                row++;
        //                var paramCaptionCells = worksheet.Cells[row, 1];
        //                paramCaptionCells.Value = $"{item.FieldCaption}";
        //                paramCaptionCells.Style.Font.Size = 12;

        //                var paramValueCells = worksheet.Cells[row, 2];
        //                if (!string.IsNullOrWhiteSpace(item.FieldValueCaption))
        //                {
        //                    if (item.FieldValueCaption.Length > 20)
        //                    {
        //                        if (item.FieldValueCaption.IndexOf(',') > -1)
        //                        {

        //                            var guid = Guid.NewGuid().ToString();
        //                            string sheetSubName = $"Parameters_{guid.Substring(0, 10)}";

        //                            var multiparamSheets = excelPackage.Workbook.Worksheets.Add(sheetSubName);

        //                            multiparamSheets.TabColor = ColorTranslator.FromHtml("#e37222");
        //                            int multiparamSheetsRow = 1;

        //                            foreach (var itemIndexData in item.FieldValueCaption.Split(","))
        //                            {
        //                                multiparamSheets.Cells[multiparamSheetsRow, 1].Value = $"{itemIndexData}";
        //                                multiparamSheetsRow++;
        //                            }
        //                            multiparamSheets.Workbook.CalcMode = ExcelCalcMode.Automatic;
        //                            multiparamSheets.Cells.AutoFitColumns();

        //                            paramValueCells.Hyperlink = new ExcelHyperLink($"#'{sheetSubName}'!A1", item.FieldValueCaption.Split(",")[0] + "...");


        //                        }
        //                        else
        //                        {
        //                            paramValueCells.Value = item.FieldValueCaption;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        paramValueCells.Value = item.FieldValueCaption;
        //                    }



        //                }
        //                else
        //                {
        //                    paramValueCells.Value = item.FieldValueCaption;
        //                }


        //                paramValueCells.Style.Font.Size = 12;
        //                paramValueCells.Style.Font.Bold = true;
        //                if (item.FieldName == "#MetaData")
        //                {
        //                    paramValueCells.Style.Font.Italic = true;
        //                    paramValueCells.Style.Font.Bold = false;
        //                }



        //            }


        //            row = row + 2;



        //            int column = 1;
        //            worksheet.View.FreezePanes(row + 1, column);

        //            foreach (var header in headerProps)
        //            {
        //                worksheet.Cells[row, column].Value = UsefulUtil.AddSpacesToSentence(header, true);
        //                var headerCells = worksheet.Cells[row, column];
        //                headerCells.Style.Font.Bold = true;
        //                headerCells.Style.Font.Size = 13;
        //                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00aaad"));
        //                headerCells.Style.Font.Color.SetColor(Color.White);
        //                headerCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);


        //                headerCells.AutoFilter = true;
        //                column++;
        //            }

        //            // To set AutoFilter on the entire column range
        //            worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
        //            row++;

        //            // Loop through each dynamic object and fill in the cells
        //            foreach (var d in dynamicList)
        //            {
        //                column = 1;
        //                foreach (var prop in (IDictionary<string, object>)d)
        //                {

        //                    if (prop.Value is DateTime dateTimeValue)
        //                    {
        //                        if (prop.Key == "ToTravelDate" || prop.Key == "FromTravelDate" || prop.Key == "Date" || prop.Key == "LastTransportDate" || prop.Key == "EventDate")
        //                        {
        //                            worksheet.Cells[row, column].Value = dateTimeValue;
        //                            worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd";
        //                        }
        //                        else
        //                        {
        //                            worksheet.Cells[row, column].Value = dateTimeValue;
        //                            worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        worksheet.Cells[row, column].Value = prop.Value;
        //                    }
        //                    column++;
        //                }

        //                // worksheet.Cells[row, column].AutoFitColumns();
        //                row++;
        //            }
        //            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        //            // Save the new workbook
        //            string fname = reportName + " " + Guid.NewGuid().ToString() + ".xlsx";

        //            worksheet.PrinterSettings.Scale = 45;

        //            // Configure print settings to fit the sheet on one page
        //            worksheet.PrinterSettings.FitToPage = true;
        //            worksheet.PrinterSettings.FitToWidth = 1;
        //            worksheet.PrinterSettings.FitToHeight = 0;


        //            var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

        //            var fileInfo = new FileInfo(realPath);
        //            excelPackage.SaveAs(fileInfo);

        //            return $"{filePath}\\{fname}";
        //        }
        //        else
        //        {

        //            return string.Empty;

        //        }
        //    }
        //}


        #endregion

    }
}
