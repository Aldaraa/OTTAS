using Application.Common.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private bool hasGroupMasterOffsiteNoFutureBooking = false;
        private async Task<string> OffsiteNoFutureBooking(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            string fields = "";
            string fieldAliases = "";
            foreach (var field in FieldNames)
            {
                if (field.FieldName == "ProfileData.gender")
                {
                    if (field.FieldName == "ProfileData.gender")
                    {
                        fields += $", CASE    WHEN ProfileData.gender = 1 THEN 'Male'    ELSE 'Female'  END AS {field.FieldCaption}";
                    }
                }
                else
                {
                    fields += $", {field.FieldName} AS {field.FieldCaption}";
                    fieldAliases += $", {field.FieldCaption}";
                }
            }

            var employeeId = await GetUserId();

            var query= @$"SELECT 
                                ProfileData.Id 'Person #',
                                ProfileData.SAPID SAPID,
                                CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname ) Name,
                                Department.Name DepartmentName,
                                Employer.Description EmployerName,
                                PeopleType.Code PeopleType,
                                CONCAT(CostCode.Number, ' ', CostCode.Description) CostCode,
                                t.LastTransportDate as 'LastTransportDate', 
                                r.Number RoomNumber, 
                                c.Description Camp,  
                                rt.Description RoomType, 
                                CASE WHEN ProfileData.RoomId = t.RoomId THEN 'Yes' ELSE 'No' END AS RoomOwner
                                {fields}
                            FROM Employee ProfileData 
                            RIGHT JOIN (


                             SELECT 
                                tt.MaxEventDate AS LastTransportDate,
                                tt.EmployeeId,
                                es.RoomId
                            FROM 
                                (SELECT 
                                t1.EmployeeId, 
                                MAX(t1.EventDate) AS MaxEventDate
                            FROM 
                                Transport t1
                            WHERE 
                                t1.Direction = 'OUT' 
                                AND t1.EventDate < '[CURRENTDATE]'
                                AND t1.EmployeeId NOT IN (
                                    SELECT DISTINCT t2.EmployeeId
                                    FROM Transport t2
                                    WHERE t2.EventDate >= '[CURRENTDATE]'
                                )
                            GROUP BY 
                                t1.EmployeeId
                      ) tt
                            LEFT JOIN 
                                EmployeeStatus es 
                                ON es.EmployeeId = tt.EmployeeId AND DATEADD(DAY, -1, tt.MaxEventDate) = es.EventDate
                            LEFT JOIN Employee e ON es.EmployeeId = e.Id
                            WHERE 
                                tt.MaxEventDate <= '[CURRENTDATE]' 
                                AND es.RoomId IS NOT NULL  and e.Active = 1
                              
    
                            ) t
                            ON ProfileData.Id = t.EmployeeId
                            LEFT JOIN Room r ON r.Id = t.RoomId
                            LEFT JOIN Camp c ON r.CampId = c.Id
                            LEFT JOIN RoomType rt ON r.RoomTypeId = rt.Id
                            LEFT JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id
                            LEFT JOIN Employer Employer ON ProfileData.employerId = Employer.Id 
                            LEFT JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id 
                            LEFT JOIN CostCodes CostCode ON ProfileData.CostCodeId = CostCode.Id
                           /* left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo  */
                             inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                                  
                          
                            ";

            var query2 = @$"SELECT Id 'Person#', SAPID, name, DepartmentName, EmployerName, PeopleType, CostCode, LastTransportDate, RoomNumber, Camp, RoomType,  RoomOwner, [COLS] {fieldAliases} from (SELECT 
                                ProfileData.Id,
                                ProfileData.SAPID SAPID,
                                CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname ) Name,
                                Department.Name DepartmentName,
                                Employer.Description EmployerName,
                                PeopleType.Code PeopleType,
                                CONCAT(CostCode.Number, ' ', CostCode.Description) CostCode,
                                t.LastTransportDate as 'LastTransportDate', 
                                r.Number RoomNumber, 
                                c.Description Camp,  
                                rt.Description RoomType, 
                                CASE WHEN ProfileData.RoomId = t.RoomId THEN 'Yes' ELSE 'No' END AS RoomOwner,
                                 gm.Description AS GroupMasterDescription, 
                                 gd.Description AS GroupDetailDescription
                                {fields}
                            FROM Employee ProfileData 
                            RIGHT JOIN (


                             SELECT 
                                tt.MaxEventDate AS LastTransportDate,
                                tt.EmployeeId,
                                es.RoomId
                            FROM 
                                (SELECT 
                                t1.EmployeeId, 
                                MAX(t1.EventDate) AS MaxEventDate
                            FROM 
                                Transport t1
                            WHERE 
                                t1.Direction = 'OUT' 
                                AND t1.EventDate < '[CURRENTDATE]'
                                AND t1.EmployeeId NOT IN (
                                    SELECT DISTINCT t2.EmployeeId
                                    FROM Transport t2
                                    WHERE t2.EventDate >= '[CURRENTDATE]'
                                )
                            GROUP BY 
                                t1.EmployeeId
                      ) tt
                            LEFT JOIN 
                                EmployeeStatus es 
                                ON es.EmployeeId = tt.EmployeeId AND DATEADD(DAY, -1, tt.MaxEventDate) = es.EventDate
                            LEFT JOIN Employee e ON es.EmployeeId = e.Id
                            WHERE 
                                tt.MaxEventDate <= '[CURRENTDATE]' 
                                AND es.RoomId IS NOT NULL  and e.Active = 1
                              
    
                            ) t
                                ON ProfileData.Id = t.EmployeeId
                                LEFT JOIN Room r ON r.Id = t.RoomId
                                LEFT JOIN Camp c ON r.CampId = c.Id
                                LEFT JOIN RoomType rt ON r.RoomTypeId = rt.Id
                                LEFT JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id
                                LEFT JOIN Employer Employer ON ProfileData.employerId = Employer.Id 
                                LEFT JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id 
                                LEFT JOIN CostCodes CostCode ON ProfileData.CostCodeId = CostCode.Id
                              /*  left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo */
                                inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                                LEFT JOIN GroupMembers gmbr ON ProfileData.Id = gmbr.EmployeeId
                                LEFT JOIN GroupMaster gm ON gmbr.GroupMasterId = gm.Id
                                LEFT JOIN dbo.GroupDetail gd ON gmbr.GroupDetailId = gd.Id 
                                [WHERECONDITION] [GROUPDETAILDESCRIPTIONCONDITION]                        

                            ) x
                        PIVOT 
                        (
                            MAX(GroupDetailDescription)
                             FOR GroupMasterDescription IN ([COLS2])
                        ) p 
                        ORDER BY Name;  
                          
                            ";





            string whereCondition = "";
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
                                whereCondition = $" WHERE ProfileData.EmployerId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.EmployerId IN {item.FieldValue}";
                        }
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
                                whereCondition = $" WHERE ProfileData.PeopleTypeId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.PeopleTypeId IN {item.FieldValue}";
                        }
                    }


                }

                if (ItemData.FieldName == "StartDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        query = query.Replace("[CURRENTDATE]", result.ToString("yyyy-MM-dd"));
                        query2 = query2.Replace("[CURRENTDATE]", result.ToString("yyyy-MM-dd"));

                    }
                    else
                    {
                        DateTime currentDdate = DateTime.Today.AddDays(-1);
                        query = query.Replace("[CURRENTDATE]", currentDdate.ToString("yyyy-MM-dd"));
                        query2 = query2.Replace("[CURRENTDATE]", currentDdate.ToString("yyyy-MM-dd"));
                    }


                }


                if (ItemData.FieldName == "GroupMasterId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        var groupMasterQuery = @$"SELECT Id, gm.Description FROM GroupMaster gm WHERE gm.id IN (
                                SELECT gd.GroupMasterId FROM GroupDetail gd WHERE gd.id IN {ItemData.FieldValue})";
                        hasGroupMasterOffsiteNoFutureBooking = true;

                        var groupColumns = await ExecuteGroupColumnData(groupMasterQuery);
                        if (!string.IsNullOrWhiteSpace(groupColumns))
                        {
                            if (whereCondition == "")
                            {
                                query2 = query2.Replace("[GROUPDETAILDESCRIPTIONCONDITION]", $" where gd.id IN {ItemData.FieldValue}");
                            }
                            else {
                                query2 = query2.Replace("[GROUPDETAILDESCRIPTIONCONDITION]", $" and gd.id IN {ItemData.FieldValue}");
                            }

                            query2 = query2.Replace("[COLS]", groupColumns);
                            query2 = query2.Replace("[COLS2]", groupColumns);



                        }


                    }

                }

            }

            if (hasGroupMasterOffsiteNoFutureBooking)
            {
                query2 = query2.Replace("[EMPLOYERQUERY]", " ");

                query2 = query2.Replace("[WHERECONDITION]", whereCondition);


                return await SaveDynamicListToExcelFile(query2, @"\Assets\GeneratedFiles\", reportName, reportParams);
            }
            else {
                query = query.Replace("[EMPLOYERQUERY]", " ");

                query = query + " " + whereCondition;

                query = query + " ORDER BY t.LastTransportDate, ProfileData.Firstname";

                return await SaveDynamicListToExcelFile(query, @"\Assets\GeneratedFiles\", reportName, reportParams);
            }

        }


        private async Task<string> OffsiteNoFutureBookingExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var dynamicList = await ExecuteReportQuery(queryData, reportName);

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

                            if (prop.Value is DateTime dateTimeValue)
                            {
                                if (prop.Key == "LastTransportDate")
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

                        // worksheet.Cells[row, column].AutoFitColumns();
                        row++;
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
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
