using Application.Common.Utils;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Exceptions;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        private async Task<string> RoomOccupancy(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            string fields = "";
            foreach (var field in FieldNames)
            {
                    fields += $", {field.FieldName} AS {field.FieldCaption} ";
            }

            var employeeId = await GetUserId();

            var query = $@"WITH TransportIn AS (
                                    SELECT 
                                        MAX(t.EventDateTime) AS TransportInDate, 
                                        t.EmployeeId 
                                    FROM 
                                        Transport t 
                                    WHERE 
                                        t.Direction = 'IN' AND t.EventDate <= '[ENDDATE]'  
                                    GROUP BY 
                                        t.EmployeeId
                                ),
                                TransportOut AS (
                                    SELECT 
                                        MIN(t.EventDateTime) AS TransportDate, 
                                        t.EmployeeId 
                                    FROM 
                                        Transport t 
                                    WHERE 
                                        t.Direction = 'OUT' AND t.EventDate >= '[STARTDATE]'
                                    GROUP BY 
                                        t.EmployeeId
                                ),
                                EmployeeStatusFiltered AS (
                                    SELECT 
                                        es.RoomId, 
                                        es.EmployeeId, 
                                        es.ShiftId, 
                                        es.EventDate,
                                        es.EmployerId,
                                        es.BedId
                                    FROM 
                                        EmployeeStatus es
                                    WHERE 
                                        es.EventDate >= '[STARTDATE]' AND es.EventDate <= '[ENDDATE]'
                                )

                                SELECT * 
                                FROM (
                                    SELECT 
                                        c.Description AS Camp, 
                                        rt.Description AS RoomType,  
                                        r.Number AS RoomNumber,
                                        b.Description BedNo,
                                        r.BedCount,
                                        case WHEN  ProfileData.RoomId = r.Id THEN 'True' ELSE 'False' END AS 'RoomOwner', 
                                        CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname) AS ProfileInfo,
                                        ProfileData.Id AS ProfilePersonId,
                                        e1.Description AS OnsiteEmployer,
                                        d1.Name AS OnsiteDepartment,
                                        onsitetransport.TransportDate AS OutDate,
                                        ProfileData.SAPID AS OnSiteSAPID
                                        [FIELDS] ,
                                        es.ShiftId,
                                        es.EventDate
                                        
                                    FROM 
                                        Room r
                                    LEFT JOIN 
                                        RoomType rt ON r.RoomTypeId = rt.Id 
                                    LEFT JOIN 
                                        Camp c ON r.CampId = c.Id
                                    LEFT JOIN 
                                        EmployeeStatusFiltered es ON es.RoomId = r.Id
                                    LEFT  JOIN bed b ON es.BedId = b.Id
                                    LEFT JOIN 
                                        Employee ProfileData ON es.EmployeeId = ProfileData.Id
                                    LEFT JOIN 
                                        Employer e1 ON es.EmployerId = e1.Id
                                    LEFT JOIN 
                                        ReportDeparmentData d1 ON ProfileData.DepartmentId = d1.Id
                                    LEFT JOIN 
                                        TransportOut onsitetransport ON ProfileData.Id = onsitetransport.EmployeeId  
                                 /*   INNER JOIN 
                                        ReportProfileData Profile ON ProfileData.Id = Profile.PersonNo*/
                                    inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo

                                   [WHERECONDITION]
                                ) AS SourceTable
                                PIVOT (
                                    MAX(SourceTable.ShiftId) 
                                    FOR SourceTable.EventDate IN ([CONTINUESDAYS])
                                ) AS PivotTable
                                ORDER BY PivotTable.RoomNumber
                                ";


            string ownerQuery = @"SELECT c.Description Camp, rt.Description RoomType,  r.Number RoomNumber,  r.BedCount,
                            ownerEmployee.Id OwnerId,
                            ownerEmployee.SAPID 'Owner SAPID',
                            CONCAT(ownerEmployee.Firstname, ' ', ownerEmployee.Lastname) OwnerName,
                             d.Name OwnerDepartmentName,
                             e.Description OwnerEmployer,
                             transport.TransportDate
                             FROM Room r
                            left JOIN RoomType rt ON r.RoomTypeId = rt.Id 
                            left join Camp c ON r.CampId = c.Id
                            LEFT JOIN Employee ownerEmployee ON r.Id = ownerEmployee.RoomId 
                            left JOIN ReportDeparmentData     d ON ownerEmployee.DepartmentId = d.Id
                            left JOIN Employer e ON ownerEmployee.employerId = e.Id
                            left JOIN (SELECT min(t.EventDate) TransportDate, t.EmployeeId from Transport t 
                            WHERE t.Direction = 'IN' and t.EventDate >'[STARTDATE]' GROUP BY t.EmployeeId) transport
                            ON ownerEmployee.Id = transport.EmployeeId
                            [WHERECONDITION]
                            ORDER BY r.Number
                            ";


            var shiftDataQuery = @"SELECT s.Id, s.Code, s.Description, c.Code ColorCode, case WHEN s.OnSite = 1 THEN 'Yes' ELSE 'No' END AS OnSite
                 from Shift s
                 join Color c ON s.ColorId = c.Id";
            string whereCondition = "";
            string whereConditionOwner = "";

            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1);


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
                                whereCondition = $" WHERE e1.Id IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND e1.Id IN {item.FieldValue}";
                        }
                    }


                }
                if (ItemData.FieldName == "GroupMasterId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";
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
                                whereCondition = $" WHERE ProfileData.DepartmentId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.DepartmentId IN {item.FieldValue}";
                        }
                    }
                }

                if (ItemData.FieldName == "CampId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE r.CampId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND r.CampId IN {item.FieldValue}";
                        }


                        if (whereConditionOwner == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereConditionOwner = $" WHERE r.CampId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereConditionOwner += $" AND r.CampId IN {item.FieldValue}";
                        }


                    }
                }
                if (ItemData.FieldName == "RoomTypeId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE rt.Id IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND rt.Id IN {item.FieldValue}";
                        }


                        if (whereConditionOwner == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereConditionOwner = $" WHERE rt.Id IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereConditionOwner += $" AND rt.Id IN {item.FieldValue}";
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


            if ((endDate - startDate).TotalDays > 30)
            {
                throw new BadRequestException("The duration between start date and end date cannot be greater than 30 days.");
            }


            query = query.Replace("[STARTDATE]", startDate.ToString("yyyy-MM-dd"));
            query = query.Replace("[ENDDATE]", endDate.ToString("yyyy-MM-dd"));

            ownerQuery = ownerQuery.Replace("[STARTDATE]", startDate.ToString("yyyy-MM-dd"));


            query = query.Replace("[FIELDS]", fields);
            query = query.Replace("[WHERECONDITION]", whereCondition);
            query = query.Replace("[CONTINUESDAYS]", continuesDaysColumn);

            ownerQuery = ownerQuery.Replace("[WHERECONDITION]", whereConditionOwner);

            return await SaveRoomOccupancyToExcelFile(query, ownerQuery, @"\Assets\GeneratedFiles\", reportName, reportParams, ColumnDates, shiftDataQuery);
        }



        private async Task<string> SaveRoomOccupancyToExcelFile(string queryData, string ownerQueryData, string filePath, string reportName, List<ReportParam> reportParams, List<DateTime> columnDates, string shiftquery)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var dynamicList = await ExecuteReportQuery(queryData, reportName);



            var shiftData = await ExecuteReportQuery(shiftquery, reportName);

            var ownerData = await ExecuteReportQuery(ownerQueryData, reportName);



            List<RosterShiftData> shiftItemData = GetRosterShiftData(shiftData);





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
                    var worksheetOwner = excelPackage.Workbook.Worksheets.Add("RoomOwners");

                    worksheetLegend = SetRosterShiftSet(worksheetLegend, shiftItemData);
                   worksheetOwner =  SetRoomOwners(worksheetOwner, ownerData);

                   // worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
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

                        paramValueCells.Style.Font.Size = 11;
                        paramValueCells.Style.Font.Bold = true;
                        if (item.FieldName == "#MetaData")
                        {
                            paramValueCells.Style.Font.Italic = true;
                            paramValueCells.Style.Font.Bold = false;

                        }

                    }

                    var headerDayNameIndex = headerProps.Count() - columnDates.Count() + 1;


                    row = row + 2;

                    int column = 1;
                    worksheet.View.FreezePanes(row + 1, column);

                    Dictionary<string, int> columnIndices = new Dictionary<string, int>();

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
                        }

                        columnIndices[header] = column;
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
                            //  worksheet.Cells[row, column].Value = prop.Value;
                            if (prop.Value is DateTime dateTimeValue)
                            {


                                //        worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                                worksheet.Cells[row, column].Value = dateTimeValue;
                                worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";

                                // If the value is a DateTime, convert it to a string with the desired format

                            }
                            else
                            {
                                if (DateTime.TryParseExact(prop.Key, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                                {
                                    if (!string.IsNullOrWhiteSpace(Convert.ToString(prop.Value)))
                                    {

                                        var shift = shiftItemData.Where(x => x.Id == Convert.ToInt32(prop.Value)).FirstOrDefault();
                                        if (shift != null)
                                        {
                                            worksheet.Cells[row, column].Value = shift.Code;
                                            worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(shift.ColorCode));

                                        }
                                    }

                                }
                                else
                                {
                                    if (prop.Key == "RoomOwner" && prop.Value.ToString() == "True")
                                    {
                                        if (columnIndices.TryGetValue("RoomNumber", out int roomNumberCol))
                                        {
                                            worksheet.Cells[row, roomNumberCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, roomNumberCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("Camp", out int campCol))
                                        {
                                            worksheet.Cells[row, campCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, campCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("No", out int NoCol))
                                        {
                                            worksheet.Cells[row, NoCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, NoCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("RoomType", out int RoomTypeCol))
                                        {
                                            worksheet.Cells[row, RoomTypeCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, RoomTypeCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("ProfileInfo", out int ProfileInfoCol))
                                        {
                                            worksheet.Cells[row, ProfileInfoCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, ProfileInfoCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("TransportInDate", out int TransportInDateCol))
                                        {
                                            worksheet.Cells[row, TransportInDateCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, TransportInDateCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("RoomOwner", out int RoomOwnerCol))
                                        {
                                            worksheet.Cells[row, RoomOwnerCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, RoomOwnerCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        //ProfilePersonId
                                        if (columnIndices.TryGetValue("OnsiteEmployer", out int OnsiteEmployerCol))
                                        {
                                            worksheet.Cells[row, OnsiteEmployerCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, OnsiteEmployerCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("ProfilePersonId", out int ProfilePersonIdCol))
                                        {
                                            worksheet.Cells[row, ProfilePersonIdCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, ProfilePersonIdCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("BedCount", out int BedCountCol))
                                        {
                                            worksheet.Cells[row, BedCountCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, BedCountCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }
                                        if (columnIndices.TryGetValue("BedNo", out int BedNoCol))
                                        {
                                            worksheet.Cells[row, BedNoCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            worksheet.Cells[row, BedNoCol].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                                        }

                                    }



                                    worksheet.Cells[row, column].Value = prop.Value;
                                }



                            }
                            column++;
                        }

                        // worksheet.Cells[row, column].AutoFitColumns();
                        row++;
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    // Save the new workbook
                    string fname = $"RoomOccupancy-{Guid.NewGuid().ToString()}.xlsx";

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

        private ExcelWorksheet SetRoomOwners(ExcelWorksheet worksheet, List<dynamic> data)
        {

            var headerProps = ((IDictionary<string, object>)data[0]).Keys;

            int row = 1;
            int column = 1;
            worksheet.View.FreezePanes(row + 1, column);

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
                }


                column++;
            }


            worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
            row++;

            // Loop through each dynamic object and fill in the cells
            foreach (var d in data)
            {
                column = 1;
                foreach (var prop in (IDictionary<string, object>)d)
                {
                    //  worksheet.Cells[row, column].Value = prop.Value;
                    if (prop.Value is DateTime dateTimeValue)
                    {
                        //    worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");

                        worksheet.Cells[row, column].Value = dateTimeValue;
                        worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";


                        // If the value is a DateTime, convert it to a string with the desired format

                    }
                    else
                    {
                        worksheet.Cells[row, column].Value = prop.Value;

                    }
                    column++;
                }

                // worksheet.Cells[row, column].AutoFitColumns();
                row++;
            }
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();


            return worksheet;
        }

    }
}
