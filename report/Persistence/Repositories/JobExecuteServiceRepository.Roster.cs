using Application.Common.Utils;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FluentValidation.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Diagnostics;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {

        private bool hasGroupMasterRoster = false;

        private async Task<string> Roster(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {

            var employeeId = await GetUserId();

            string fields = "";
            string fieldAliases = "";

            bool showOnsite = false;
            bool showTravelTime = false;
            foreach (var field in FieldNames)
            {
                fields += $", {field.FieldName} AS {field.FieldCaption}";
                fieldAliases += $", {field.FieldCaption}";
            }




            //var query = @$"SELECT *
            //                FROM (
            //                SELECT  
            //                      ProfileData.Id as 'Person #',
            //                      ProfileData.Firstname,
            //                      ProfileData.Lastname,
            //                      ProfileData.SAPID,
            //                      Department.Name DepartmentName,
            //                      Employer.Description	EmployerName,
            //                      PeopleType.Code	PeopleType,
            //                      Location.Description Commutebase,
            //                      Roster.Name Roster
            //                      {fields}, 
            //                      es.ShiftId, es.EventDate FROM Employee ProfileData [WHERECONDITION]
            //                  [ONSITEJOINSTATUS] JOIN (SELECT es.ShiftId, es.EventDate, es.EmployeeId from EmployeeStatus es
            //                     WHERE es.EventDate >= '[STARTDATE]' AND es.EventDate <= '[ENDDATE]' [ONSITESTATUS]) es
            //                  on es.EmployeeId = ProfileData.Id
            //                  LEFT JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id
            //                  LEFT JOIN Employer Employer ON ProfileData.employerId = Employer.Id 
            //                  LEFT JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id 
            //                  LEFT JOIN Location Location On ProfileData.LocationId = Location.Id
            //                  LEFT JOIN Roster Roster on ProfileData.RosterId = Roster.Id
            //                  left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo  
            //                  WHERE  ProfileData.Active = 1
            //                  )
            //                  AS SourceTable
            //                PIVOT (
            //                    MAX(SourceTable.ShiftId) FOR SourceTable.EventDate IN 
            //                        ([CONTINUESDAYS])
            //                ) AS PivotTable;";


            var query = @$"SELECT *
                            FROM (
                            SELECT 
                                  ProfileData.Id as 'Person #',
                                  ProfileData.Firstname,
                                  ProfileData.Lastname,
                                  ProfileData.SAPID,
                                  Department.Name DepartmentName,
                                  Employer.Description	EmployerName,
                                  PeopleType.Code	PeopleType,
                                  Location.Description Commutebase,
                                  Roster.Name Roster
                                  {fields}, 
                                  es.ShiftId, es.EventDate FROM EmployeeStatus es
                              RIGHT JOIN (SELECT e.Id,  e.DepartmentId, e.employerId, e.CostCodeId, e.PeopleTypeId, e.LocationId, e.RosterId,
                                 e.SAPID AS SAPID, e.Firstname, e.Lastname
 
                              from Employee e [WHERECONDITION]
                                ) ProfileData
                              on es.EmployeeId = ProfileData.Id
                              LEFT JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id
                              LEFT JOIN Employer Employer ON ProfileData.employerId = Employer.Id 
                              LEFT JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id 
                              LEFT JOIN Location Location On ProfileData.LocationId = Location.Id
                              LEFT JOIN Roster Roster on ProfileData.RosterId = Roster.Id
                            /*  left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo  */
                             inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                              WHERE es.EventDate >= '[STARTDATE]' AND es.EventDate <= '[ENDDATE]' [ONSITESTATUS])
                              AS SourceTable
                            PIVOT (
                                MAX(SourceTable.ShiftId) FOR SourceTable.EventDate IN 
                                    ([CONTINUESDAYS])
                            ) AS PivotTable;";


            var query2 = $@"SELECT *
                            FROM (


                            SELECT 
                                PersonId 'Person #',
                                Firstname, 
                                Lastname,
                                SAPID,
                                DepartmentName
                                EmployerName,
                                PeopleType,
                                Commutebase,
                                Roster
                                {fieldAliases}, 
                                ShiftId,
                                EventDate,
                                [COLS]
  
from
                            (
                            SELECT 
                                  ProfileData.Id as PersonId,
                                  ProfileData.Firstname,
                                  ProfileData.Lastname,
                                  ProfileData.SAPID,
                                  Department.Name DepartmentName,
                                  Employer.Description	EmployerName,
                                  PeopleType.Code	PeopleType,
                                  Location.Description Commutebase,
                                  Roster.Name Roster
                                                {fields},  
                                  es.ShiftId, es.EventDate,
                                  gm.Description AS GroupMasterDescription, 
                                   gd.Description AS GroupDetailDescription
                                  FROM EmployeeStatus es
                              RIGHT JOIN (SELECT e.Id,  e.DepartmentId, e.employerId, e.CostCodeId, e.PeopleTypeId, e.LocationId, e.RosterId,
                                 e.SAPID AS SAPID, e.Firstname, e.Lastname
 
                              from Employee e [WHERECONDITION]
                                ) ProfileData
                              on es.EmployeeId = ProfileData.Id
                              LEFT JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id
                              LEFT JOIN Employer Employer ON ProfileData.employerId = Employer.Id 
                              LEFT JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id 
                              LEFT JOIN Location Location On ProfileData.LocationId = Location.Id
                              LEFT JOIN Roster Roster on ProfileData.RosterId = Roster.Id
                             /* left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo  */
                             inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                
                              LEFT JOIN GroupMembers gmbr ON ProfileData.Id = gmbr.EmployeeId
                              LEFT JOIN GroupMaster gm ON gmbr.GroupMasterId = gm.Id
                              LEFT JOIN dbo.GroupDetail gd ON gmbr.GroupDetailId = gd.Id 

                              WHERE es.EventDate >= '[STARTDATE]' AND es.EventDate <= '[ENDDATE]' [ONSITESTATUS]  [GROUPDETAILDESCRIPTIONCONDITION]
                              
 ) x
                    PIVOT 
                    (
                        MAX(GroupDetailDescription)
                        FOR GroupMasterDescription IN ([COLS2])
                    ) p 

                              )
                              AS SourceTable
                            PIVOT (
                                MAX(SourceTable.ShiftId) FOR SourceTable.EventDate IN 
                                    ([CONTINUESDAYS])
                            ) AS PivotTable";



            var shiftDataQuery = @"SELECT s.Id, s.Code, s.Description, c.Code ColorCode, case WHEN s.OnSite = 1 THEN 'Yes' ELSE 'No' END AS OnSite
                 from Shift s
                 join Color c ON s.ColorId = c.Id";
            string whereCondition = "";
            int continuesDay = 5;
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today;

            foreach (var item in reportParams)
            {

                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "EmployerId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE e.EmployerId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND e.EmployerId IN {item.FieldValue}";
                        }
                    }


                }

                if (ItemData.FieldName == "OnSite")
                {

                    string fieldValue = ItemData.FieldValue?.Trim('\"').Trim();
                    if (!string.IsNullOrEmpty(fieldValue) && fieldValue.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                    {
                        showOnsite = true;
                    }

                }


                if (ItemData.FieldName == "TravelTime")
                {
                    string fieldValue = ItemData.FieldValue?.Trim('\"').Trim();
                    if (!string.IsNullOrEmpty(fieldValue) && fieldValue.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                    {
                     
                        showTravelTime = true;
                    }


                }


                if (ItemData.FieldName == "PeopleTypeId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE e.PeopleTypeId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND e.PeopleTypeId IN {item.FieldValue}";
                        }
                    }
                }

                if (ItemData.FieldName == "DepartmentId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE e.DepartmentId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND e.DepartmentId IN {item.FieldValue}";
                        }
                    }
                }
                if (ItemData.FieldName == "RosterId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE e.RosterId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND e.RosterId IN {item.FieldValue}";
                        }
                    }
                }

                if (ItemData.FieldName == "NationalityId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE e.NationalityId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND e.NationalityId IN {item.FieldValue}";
                        }
                    }
                }

                //if (ItemData.FieldName == "GroupMasterId")
                //{
                //    if (ItemData.FieldValue != "ALL")
                //    {
                //        if (whereCondition == "")
                //        {
                //            if (ItemData.FieldValue != "ALL")
                //            {
                //                whereCondition = $" WHERE e.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";
                //            }

                //        }
                //        else
                //        {
                //            whereCondition += $" AND e.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";
                //        }
                //    }
                //}
                if (ItemData.FieldName == "GroupMasterId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        var groupMasterQuery = @$"SELECT Id, gm.Description FROM GroupMaster gm WHERE gm.id IN (
                                SELECT gd.GroupMasterId FROM GroupDetail gd WHERE gd.id IN {ItemData.FieldValue})";
                        hasGroupMasterRoster = true;

                        var groupColumns = await ExecuteGroupColumnData(groupMasterQuery);
                        if (!string.IsNullOrWhiteSpace(groupColumns))
                        {
                            query2 = query2.Replace("[GROUPDETAILDESCRIPTIONCONDITION]", $" AND gd.id IN {ItemData.FieldValue}");
                            query2 = query2.Replace("[COLS]", groupColumns);
                            query2 = query2.Replace("[COLS2]", groupColumns);



                        }


                        if (whereCondition == "")
                        {
                            whereCondition = $" WHERE e.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";


                        }
                        else
                        {
                            whereCondition += $" AND e.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";
                        }


                    }

                }




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
            string continuesDaysColumn = "";
            List<DateTime> ColumnDates = new List<DateTime>();

            for (DateTime sDate = startDate; sDate <= endDate; sDate = sDate.AddDays(1))
            {
                if (continuesDaysColumn == "")
                {
                    continuesDaysColumn = $"[{sDate.ToString("yyyy-MM-dd")}]";
                    ColumnDates.Add(sDate);
                }
                else
                {
                    continuesDaysColumn += $",[{sDate.ToString("yyyy-MM-dd")}]";
                    ColumnDates.Add(sDate);
                }
            }


            var removeFieldCurrentDdate = reportParams.Where(x => x.FieldName == "CurrentDate").FirstOrDefault();
            if (removeFieldCurrentDdate != null)
            {
                reportParams.Remove(removeFieldCurrentDdate);
            }

            var removeFieldContinues = reportParams.Where(x => x.FieldName == "ContinuesDay").FirstOrDefault();
            if (removeFieldContinues != null)
            {
                reportParams.Remove(removeFieldContinues);
            }

            reportParams.Add(new ReportParam
            {
                FieldName = "Report Period",
                FieldCaption = "Report Period",
                FieldValueCaption = $"{startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}"
            });

            if (!hasGroupMasterRoster)
            {
                query = query.Replace("[STARTDATE]", startDate.ToString("yyyy-MM-dd"));
                query = query.Replace("[ENDDATE]", endDate.ToString("yyyy-MM-dd"));
                query = query.Replace("[CONTINUESDAYS]", continuesDaysColumn);
                query = query.Replace("[WHERECONDITION]", whereCondition);

                if (showOnsite)
                {
                    query = query.Replace("[ONSITESTATUS]", " AND es.ShiftId IN (SELECT Id FROM Shift s WHERE s.OnSite = 1)");
                    query = query.Replace("[ONSITEJOINSTATUS]", "INNER");

                }
                else
                {
                    query = query.Replace("[ONSITESTATUS]", "");
                    query = query.Replace("[ONSITEJOINSTATUS]", "LEFT");
                }

                return await SaveRosterToExcelFile(query, @"\Assets\GeneratedFiles\", reportName, reportParams, ColumnDates, shiftDataQuery, startDate, endDate,  showTravelTime);
            }
            else
            {
                query2 = query2.Replace("[STARTDATE]", startDate.ToString("yyyy-MM-dd"));
                query2 = query2.Replace("[ENDDATE]", endDate.ToString("yyyy-MM-dd"));
                query2 = query2.Replace("[CONTINUESDAYS]", continuesDaysColumn);
                query2 = query2.Replace("[WHERECONDITION]", whereCondition);

                if (showOnsite)
                {
                    query2 = query2.Replace("[ONSITESTATUS]", " AND es.ShiftId IN (SELECT Id FROM Shift s WHERE s.OnSite = 1)");

                }
                else
                {
                    query2 = query2.Replace("[ONSITESTATUS]", "");
                }

                return await SaveRosterToExcelFile(query2, @"\Assets\GeneratedFiles\", reportName, reportParams, ColumnDates, shiftDataQuery, startDate, endDate, showTravelTime);
            }

        }



        private async Task<string> SaveRosterToExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams, List<DateTime> columnDates, string shiftquery, DateTime startDate, DateTime endDate, bool showTravelTime = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var dynamicList = await ExecuteReportQuery(queryData, reportName);

            stopwatch.Stop();
            Console.WriteLine($"main query execute ==================> {stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();


            var shiftData = await ExecuteReportQuery(shiftquery, reportName);

            stopwatch.Stop();
            Console.WriteLine($"shift query execute ==================> {stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();

            List<RosterShiftData> shiftItemData = GetRosterShiftData(shiftData);

            List<TransportData> transportData = showTravelTime ? await GetTransportLoadData(startDate, endDate) : new List<TransportData>();

            stopwatch.Stop();
            Console.WriteLine($"transport direction data query execute ==================> {stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();





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
                    var worksheetLegend = excelPackage.Workbook.Worksheets.Add("Legend");
                    worksheetLegend = SetRosterShiftSet(worksheetLegend, shiftItemData);


                    //  worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
                    var titleCells = worksheet.Cells[1, 1];
                    titleCells.Value = reportName;
                    titleCells.Style.Font.Bold = true;
                    titleCells.Style.Font.Size = 12;
                    //titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
                    //titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
                    int row = 2;





                   // worksheet.IgnoredErrors = ExcelIgnoreErrors.NumberStoredAsText;


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

                        paramValueCells.Style.Font.Size = 11;
                        paramValueCells.Style.Font.Bold = true;
                        if (item.FieldName == "#MetaData")
                        {
                            paramValueCells.Style.Font.Italic = true;
                            paramValueCells.Style.Font.Bold = false;

                        }

                    }



                    stopwatch.Stop();
                    Console.WriteLine($"report params write ==================> {stopwatch.ElapsedMilliseconds}");
                    stopwatch.Restart();


                    var headerDayNameIndex = headerProps.Count() - columnDates.Count() + 1;
                    //row++;
                    //foreach (var item in columnDates)
                    //{
                    //    worksheet.Cells[row, headerDayNameIndex].Value = item.ToString("ddd");
                    //    var headerCells = worksheet.Cells[row, headerDayNameIndex];
                    //    headerCells.Style.Font.Bold = true;
                    //    headerCells.Style.Font.Size = 13;
                    //    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //    headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
                    //    headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
                    //    headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    headerDayNameIndex++;
                    //}




                    row = row + 2;

                    int column = 1;
                    worksheet.View.FreezePanes(row + 1, column);

                    worksheet.Workbook.CalcMode = ExcelCalcMode.Manual;

                    List<int> MainColumns = new List<int>();
                    List<int> ShiftColumns = new List<int>();

                    List<string> ShifCodes = new List<string>();


                    foreach (var header in headerProps)
                    {
                        var headerValue = UsefulUtil.AddSpacesToSentence(header, true);


                        if (DateTime.TryParseExact(headerValue, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            // the value is a DateTime, convert it to a string with the desired format
                            worksheet.Cells[row, column].Value = result.ToString("dd-MMM");
                            ExcelComment comment = worksheet.Cells[row, column].AddComment($"TAS SYSTEM : {result.ToString("ddd")}");
                            worksheet.Cells[row, column].Style.TextRotation = 75;
                            worksheet.Cells[row, column].Style.Font.Size = 13;
                            worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00aaad"));
                            worksheet.Cells[row, column].Style.Font.Color.SetColor(Color.White);
                            worksheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[row, column].AutoFilter = true;
                            worksheet.Cells[row, column].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                            ShiftColumns.Add(column);
                        }
                        else
                        {
                            worksheet.Cells[row, column].Value = UsefulUtil.AddSpacesToSentence(header, true);
                            var headerCells = worksheet.Cells[row, column];
                            headerCells.Style.Font.Bold = true;
                            headerCells.Style.Font.Size = 13;

                            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00aaad"));
                            headerCells.Style.Font.Color.SetColor(Color.White);
                            headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            headerCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                            headerCells.AutoFilter = true;

                            MainColumns.Add(column);
                        }


                        column++;
                    }


                    stopwatch.Stop();
                    Console.WriteLine($"report header prop write ==================> {stopwatch.ElapsedMilliseconds}");
                    stopwatch.Restart();


                    // To set AutoFilter on the entire column range
                    worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
                    row++;

                    // Loop through each dynamic object and fill in the cells
                    foreach (var d in dynamicList)
                    {
                        int employeeId = 0;
                        column = 1;
                        foreach (var prop in (IDictionary<string, object>)d)
                        {
                            //  worksheet.Cells[row, column].Value = prop.Value;
                            if (prop.Value is DateTime dateTimeValue)
                            {
                                if (prop.Key == "OutDate" || prop.Key == "TransportDate")
                                {
                                    //   worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd");
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd";
                                }
                                else
                                {
                                    //worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";


                                }
                                // If the value is a DateTime, convert it to a string with the desired format

                            }
                            else
                            {
                                if (DateTime.TryParseExact(prop.Key, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                                {
                                  //  worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    if (!string.IsNullOrWhiteSpace(Convert.ToString(prop.Value)))
                                    {
         

                                        if (showTravelTime)
                                        {
                                            var travelData = transportData.Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date == result.Date).FirstOrDefault();
                                            if (travelData != null)
                                            {

                                                string travelTime = $"{travelData.Time?.ToString("HH:mm")}";

                                                if (travelData.TransportMode == "Airplane" || travelData.TransportMode == "Plane")
                                                {
                                                    travelTime = "🛫 " + travelTime;
                                                }
                                                else if (travelData.TransportMode == "Bus")
                                                {
                                                    travelTime = "🚌 " + travelTime;
                                                }
                                                else
                                                {
                                                    travelTime = "🚙 " + travelTime;
                                                }



                                                var shift = shiftItemData.Where(x => x.Id == Convert.ToInt32(prop.Value)).FirstOrDefault();
                                                if (shift != null)
                                                {
                                                    //        worksheet.Cells[row, column].Style.Font.Size = 10;
                                                    worksheet.Cells[row, column].Value = $"{travelTime} {shift.Code}";
                                                    if (!ShifCodes.Contains(shift.Code))
                                                    {
                                                        ShifCodes.Add(shift.Code);
                                                    }
                                             //       worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                 //   worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(shift.ColorCode));

                                                }
                                                else
                                                {
                                                    //     worksheet.Cells[row, column].Style.Font.Size = 10;
                                                    worksheet.Cells[row, column].Value = $"{travelTime}";
                                                }


                                            }
                                            else
                                            {
                                                var shift = shiftItemData.Where(x => x.Id == Convert.ToInt32(prop.Value)).FirstOrDefault();
                                                if (shift != null)
                                                {
                                                    worksheet.Cells[row, column].Value = $"{shift.Code}";
                                                    if (!ShifCodes.Contains(shift.Code))
                                                    {
                                                        ShifCodes.Add(shift.Code);
                                                    }
                                                    //        worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                               //       worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(shift?.ColorCode));
                                                }
                                            }


                                        }
                                        else
                                        {



                                            var shift = shiftItemData.Where(x => x.Id == Convert.ToInt32(prop.Value)).FirstOrDefault();
                                            if (shift != null)
                                            {
                                                worksheet.Cells[row, column].Value = $"{shift.Code}";
                                                if (!ShifCodes.Contains(shift.Code))
                                                {
                                                    ShifCodes.Add(shift.Code);
                                                }
                                                //    worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                              //      worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(shift.ColorCode));

                                            }
                                        }


                                    }
                                    else
                                    {

                                        if (showTravelTime)
                                        {
                                            var travelData = transportData.Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date == result.Date).FirstOrDefault();
                                            if (travelData != null)
                                            {

                                                string travelTime = $"{travelData.Time?.ToString("HH:mm")}";

                                                if (travelData.TransportMode == "Airplane" || travelData.TransportMode == "Plane")
                                                {
                                               //     travelTime = "PL " + travelTime;

                                                    travelTime = "🛫 " + travelTime;

                                                }
                                                else if (travelData.TransportMode == "Bus")
                                                {

                                                    travelTime = "🚌 " + travelTime;
                                                }
                                                else
                                                {
                                                    travelTime = "🚙 " + travelTime;
                                                }


                                                var shift = shiftItemData.Where(x => x.Code == "RR").FirstOrDefault();
                                                if (shift != null)
                                                {
                                                //    worksheet.Cells[row, column].Style.Font.Size = 10;
                                                    worksheet.Cells[row, column].Value = $"{travelTime} {shift.Code}";
                                                    if (!ShifCodes.Contains(shift.Code))
                                                    {
                                                        ShifCodes.Add(shift.Code);
                                                    }
                                                      //  worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                   //     worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(shift.ColorCode));
                                                }
                                                else
                                                {
                                                    worksheet.Cells[row, column].Style.Font.Size = 10;
                                                    worksheet.Cells[row, column].Value = $"{travelTime}";
                                                }


                                            }

                                        }


                                    }

                                }
                                else
                                {
                                    if (prop.Key == "Person #")
                                    {
                                        worksheet.Cells[row, column].Value = prop.Value;
                                        employeeId = Convert.ToInt32(prop.Value);
                                    }
                                    else
                                    {
                                        worksheet.Cells[row, column].Value = prop.Value;
                                    }


                                }
                            }
                            column++;
                        }
                        row++;
                    }

                    Console.WriteLine($"EXCEL CALCULATED {stopwatch.ElapsedMilliseconds}");



                    foreach (var item in MainColumns)
                    {
                        Console.WriteLine($"--CILUMNS ====================> {item}");

                        worksheet.Column(item).AutoFit();
                    }


          
                    // Save the new workbook
                    string fname = $"Roster-{Guid.NewGuid().ToString()}.xlsx";

                    var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

                //    worksheet.Workbook.CalcMode = ExcelCalcMode.Automatic;


                    var fileInfo = new FileInfo(realPath);
                    excelPackage.Compression = CompressionLevel.BestSpeed;
                    excelPackage.SaveAs(fileInfo);

                    excelPackage.Dispose();


                    stopwatch.Stop();
                    Console.WriteLine($"transport excel fininshed {stopwatch.ElapsedMilliseconds}");


                    return $"{filePath}\\{fname}";


                }
                else
                {

                    return string.Empty;
                }
            }
        }






        private ExcelWorksheet SetRosterShiftSet(ExcelWorksheet worksheet, List<RosterShiftData> data)
        {
            //worksheet.Cells[1, 1, 1, 3].Merge = true;
            var titleCells = worksheet.Cells[1, 1];
            titleCells.Value = "Shift Status";
            titleCells.Style.Font.Bold = true;
            titleCells.Style.Font.Size = 12;
            //titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
            //titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));

            worksheet.Cells[2, 1].Value = "Code";
            worksheet.Cells[2, 2].Value = "Description";
            worksheet.Cells[2, 3].Value = "Onsite";

            var headerCells = worksheet.Cells[2, 1, 2, 3];

            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00aaad"));
            headerCells.Style.Font.Color.SetColor(Color.White);
            headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
            headerCells.AutoFilter = true;

            int row = 3;

            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = item.Code;
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(item.ColorCode));

                worksheet.Cells[row, 2].Value = item.Description;
                worksheet.Cells[row, 3].Value = item.Onsite;

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return worksheet;
        }





        private List<RosterShiftData> GetRosterShiftData(List<dynamic> dynamicList)
        {

            var returnData = new List<RosterShiftData>();
            foreach (var dynamicItem in dynamicList)
            {

                returnData.Add(new RosterShiftData
                {
                    Id = dynamicItem.Id,
                    Code = dynamicItem.Code,
                    Description = dynamicItem.Description,
                    ColorCode = dynamicItem.ColorCode,
                    Onsite = dynamicItem.OnSite,
                });


            }

            return returnData;
        }


        private async Task<TransportData?> GetTransportData(int EmployeeId, DateTime startDate, DateTime endDate)
        {
            var returnData = new TransportData();
            var transportQuery = @$"SELECT top(1) t.EmployeeId, t.EventDateTime, tm.Code, t.Direction from Transport t
                                  left join ActiveTransport at ON t.ActiveTransportId = at.Id
                                  left JOIN TransportMode tm ON at.TransportModeId = tm.Id
  
                                 WHERE t.EventDate >= '{startDate.ToString("yyyy-MM-dd")}' and t.EventDate <= '{endDate.ToString("yyyy-MM-dd")}' AND at.Direction IN ('IN', 'OUT')";

            var transportData = await ExecuteReportQuery(transportQuery, "Roster transport");
            if (transportData.Count > 0)
            {
                returnData.TransportMode = transportData[0].Code;
                returnData.Time = transportData[0].EventDateTime;
                returnData.Direction = transportData[0].Direction;
                return returnData;
            }
            else
            {
                return null;
            }

        }


        private async Task<List<TransportData>> GetTransportLoadData(DateTime startDate, DateTime endDate)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var returnData = new List<TransportData>();
            var transportQuery = @$"SELECT  t.EmployeeId, t.EventDateTime Time, tm.Code TransportMode, t.Direction, t.EventDate from Transport t
                                  left join ActiveTransport at ON t.ActiveTransportId = at.Id
                                  left JOIN TransportMode tm ON at.TransportModeId = tm.Id
  
                                 WHERE t.EventDate >= '{startDate.ToString("yyyy-MM-dd")}' and t.EventDate <= '{endDate.ToString("yyyy-MM-dd")}' AND at.Direction in('IN', 'OUT')";

            var transportData = await ExecuteReportQuery(transportQuery, "Roster transport");
            foreach (var item in transportData)
            {

                returnData.Add(new TransportData
                {
                    Direction = item.Direction,
                    EmployeeId = item.EmployeeId,
                    Time = item.Time,
                    TransportMode = item.TransportMode,
                    EventDate = item.EventDate,

                });
            }


            stopwatch.Stop();
            Console.WriteLine($"transportData ==================> {stopwatch.ElapsedMilliseconds}");






            return returnData;

        }

    }




        public class RosterShiftData
    { 
        public int Id { get; set; }
        public string?  Code { get; set; }
        public string? Description { get; set; }

        public string? ColorCode { get; set; }

        public string? Onsite { get; set; }



    }

    public class TransportData
    {
        public int EmployeeId { get; set; }
        public string? TransportMode { get; set; }
        public DateTime? Time { get; set; }

        public string? Direction { get; set; }

        public DateTime? EventDate { get; set; }





    }



}
