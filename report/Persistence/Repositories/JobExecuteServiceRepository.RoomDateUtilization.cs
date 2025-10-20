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
        private async Task<string> RoomDateUtilization(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            string fields = "";
            foreach (var field in FieldNames)
            {
                fields += $", {field.FieldName} AS {field.FieldCaption}";


            }
            var query = @$"SELECT c.Description Camp, rt.Description RoomType,  r.Number RoomNumber,
                            ownerEmployee.Id OwnerId,
                            ownerEmployee.SAPID 'Owner SAPID',
                            CONCAT(ownerEmployee.Firstname, ' ', ownerEmployee.Lastname) OwnerName,
                             d.Name OwnerDepartment,
                             e.Description OwnerEmployer,
                             transport.TransportDate ,
                             Concat(ProfileData.Firstname, ' ', ProfileData.Lastname, ' ', ProfileData.Id) ProfileInfo,
                             e1.Description OnsiteEmployer,
                             d1.Name  OnsiteDepartment,
                             onsitetransport.TransportDate OutDate,
                             ProfileData.SAPID OnSiteSAPID
                             {fields},
                            es.ShiftId '[STARTDATE]'
                             FROM Room r
                            left JOIN RoomType rt ON r.RoomTypeId = rt.Id 
                            left join Camp c ON r.CampId = c.Id
                            LEFT JOIN Employee ownerEmployee ON r.Id = ownerEmployee.RoomId 
                            left JOIN Department d ON ownerEmployee.DepartmentId = d.Id
                            left JOIN Employer e ON ownerEmployee.employerId = e.Id
                            left JOIN (SELECT min(t.EventDate) TransportDate, t.EmployeeId from Transport t 
                            WHERE t.Direction = 'IN' and t.EventDate >'[STARTDATE]' GROUP BY t.EmployeeId) transport
                            ON ownerEmployee.Id = transport.EmployeeId
                            left JOIN EmployeeStatus es ON es.RoomId = r.Id AND es.EventDate = '[STARTDATE]'
                            left JOIN Employee ProfileData ON es.EmployeeId = ProfileData.Id
                            left JOIN Employer e1 ON es.EmployerId = e1.Id
                            left JOIN Department d1 ON ProfileData.DepartmentId = d1.Id
                            left JOIN (SELECT min(t.EventDate) TransportDate, t.EmployeeId from Transport t 
                            WHERE t.Direction = 'OUT' and t.EventDate >'[STARTDATE]' GROUP BY t.EmployeeId) onsitetransport
                            ON ProfileData.Id = onsitetransport.EmployeeId  
                            left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo
                            [WHERECONDITION] ";


            var shiftDataQuery = @"SELECT s.Id, s.Code, s.Description, c.Code ColorCode, case WHEN s.OnSite = 1 THEN 'Yes' ELSE 'No' END AS OnSite
                 from Shift s
                 join Color c ON s.ColorId = c.Id";
            string whereCondition = "";
            DateTime startDate = DateTime.Today;

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
                                whereCondition = $" WHERE ProfileData.employerId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.employerId IN {item.FieldValue}";
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
                    }
                }
                if (ItemData.FieldName == "CurrentDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        startDate = result;
                    }

                }
            }
            List<DateTime> ColumnDates = new List<DateTime>();


            var removeFieldCurrentDdate = reportParams.Where(x => x.FieldName == "CurrentDate").FirstOrDefault();
            if (removeFieldCurrentDdate != null)
            {
                reportParams.Remove(removeFieldCurrentDdate);
            }



            reportParams.Add(new ReportParam
            {
                FieldName = "Report Period",
                FieldCaption = "Report Period",
                FieldValueCaption = $"{startDate.ToString("yyyy-MM-dd")}"
            });


            query = query.Replace("[STARTDATE]", startDate.ToString("yyyy-MM-dd"));
            query = query.Replace("[WHERECONDITION]", whereCondition);

            return await SaveRoomDateUtilizationToExcelFile(query, @"\Assets\GeneratedFiles\", reportName, reportParams, ColumnDates, shiftDataQuery);
        
    }


        private async Task<string> SaveRoomDateUtilizationToExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams, List<DateTime> columnDates, string shiftquery)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var dynamicList = await ExecuteReportQuery(queryData, reportName);

            var shiftData = await ExecuteReportQuery(shiftquery, reportName);

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
                    worksheetLegend = SetRosterShiftSetDateUtlization(worksheetLegend, shiftItemData);

                  //  worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
                    var titleCells = worksheet.Cells[1, 1];
                    titleCells.Value = reportName;
                    titleCells.Style.Font.Bold = true;
                    titleCells.Style.Font.Size = 13;
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
                        paramValueCells.Value = item.FieldValueCaption;
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

                    // To set AutoFilter on the entire column range
                    worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
                    row++;


                    string RoomNumber = string.Empty;
                    string ProfileInfo = string.Empty;
                    // Loop through each dynamic object and fill in the cells
                    foreach (var d in dynamicList)
                    {
                        column = 1;
                        foreach (var prop in (IDictionary<string, object>)d)
                        {
                            if (prop.Key == "RoomOwner") { 
                                
                            }


                            //  worksheet.Cells[row, column].Value = prop.Value;
                            if (prop.Value is DateTime dateTimeValue)
                            {
                                if (prop.Key == "OutDate" || prop.Key == "TransportDate")
                                {
                                    // worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd");
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd";
                                }
                                else
                                {
                                    // worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";

                                }
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
                    string fname = $"Roster-{Guid.NewGuid().ToString()}.xlsx";

                    var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

                    var fileInfo = new FileInfo(realPath);
                    excelPackage.SaveAs(fileInfo);

                    return $"{filePath}\\{fname}";
                }
                else
                {

                    var worksheet = excelPackage.Workbook.Worksheets.Add(reportName);
                    var titleCells = worksheet.Cells[1, 1];
                    titleCells.Value = reportName;
                    titleCells.Style.Font.Bold = true;
                    titleCells.Style.Font.Size = 13;
                    titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
                    titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));


                    var warningCells = worksheet.Cells[2, 1];
                    warningCells.Value = "No data found";
                    warningCells.Style.Font.Bold = true;
                    warningCells.Style.Font.Italic = true;
                    warningCells.Style.Font.Size = 12;
                    warningCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FF0000"));


                    string fname = $"Roster-{Guid.NewGuid().ToString()}.xlsx";

                    var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

                    var fileInfo = new FileInfo(realPath);
                    excelPackage.SaveAs(fileInfo);

                    return $"{filePath}\\{fname}";

                }
            }
        }

        private ExcelWorksheet SetRosterShiftSetDateUtlization(ExcelWorksheet worksheet, List<RosterShiftData> data)
        {
            worksheet.Cells[1, 1, 1, 3].Merge = true;
            var titleCells = worksheet.Cells[1, 1];
            titleCells.Value = "Shift Status";
            titleCells.Style.Font.Bold = true;
            titleCells.Style.Font.Size = 16;
            titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
            titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));

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





    }
}
