using Application.Common.Utils;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Reflection.Metadata;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        #region MandaysData

        private async Task<string> ManDays(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddMonths(1);
            string fields = "";

            var employeeId = await GetUserId();

            foreach (var field in FieldNames)
            {
                    fields += $", {field.FieldName} AS {field.FieldCaption}";
               

            }
            //var query = @$"SELECT  
            //           /* Concat(CostCode.Number, ' ', CostCode.Description) CostCentre, */
            //            COALESCE(Concat(CostCode.Number, ' ', CostCode.Description), Concat(ProfileCostCode.Number, ' ', ProfileCostCode.Description)) AS CostCentre, 
            //            d.Name Department, Employer.Description CompanyName,
            //         ProfileData.Id PersonId,  Concat(ProfileData.Firstname, ' ', ProfileData.Lastname) EmployeeName, SAPID 'EmployeeNumber' {fields}, p.Description JobTitle, 
            //         empStatus.OnsiteDays DaysOnsite, empStatus.OnsiteDays * 12 TotalHours,
            //         PeopleType.Code PassengerType FROM Employee ProfileData 
            //         RIGHT JOIN (SELECT  es.EmployeeId, es.CostCodeId,  Count(*) OnsiteDays FROM EmployeeStatus es  WHERE es.RoomId is NOT NULL 
            //          AND es.EventDate >= '[STARTDATE]' AND es.EventDate <= '[ENDDATE]'
            //         GROUP BY es.EmployeeId, es.CostCodeId) empStatus 
            //        ON ProfileData.Id = empStatus.EmployeeId
            //        left join ReportDeparmentData d ON ProfileData.DepartmentId = d.Id
            //        left JOIN CostCodes CostCode ON empStatus.CostCodeId = CostCode.Id
            //        left JOIN Employer Employer ON ProfileData.EmployerId = Employer.Id
            //        left JOIN Position p ON ProfileData.PositionId = p.Id
            //        left JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id
            //        left join CostCodes ProfileCostCode on ProfileData.CostCodeId = ProfileCostCode.Id
            //        left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo
            //        WHERE ProfileData.Active = 1
            //        ORDER BY CostCentre
            //        ";


            var query = @$"SELECT  
                            CASE 
                            WHEN CostCode.Number IS NOT NULL AND CostCode.Description IS NOT NULL 
                                THEN CONCAT(CostCode.Number, ' ', CostCode.Description) 
                            ELSE Profile.Costcode 
                        END AS CostCentre,
                        d.Name Department,
                        Employer.Description CompanyName,
	                    ProfileData.Id PersonId,  Concat(ProfileData.Firstname, ' ', ProfileData.Lastname) EmployeeName, SAPID 'EmployeeNumber' {fields}, p.Description JobTitle, 
	                    empStatus.OnsiteDays DaysOnsite, empStatus.OnsiteDays * 12 TotalHours,
	                    PeopleType.Code PassengerType FROM Employee ProfileData 
	                    RIGHT JOIN (SELECT  es.EmployeeId, es.CostCodeId, es.DepId, es.EmployerId,  Count(*) OnsiteDays FROM EmployeeStatus es  WHERE es.RoomId is NOT NULL 
		                    AND es.EventDate >= '[STARTDATE]' AND es.EventDate <= '[ENDDATE]'
	                    GROUP BY es.EmployeeId, es.CostCodeId, es.DepId, es.EmployerId) empStatus 
                    ON ProfileData.Id = empStatus.EmployeeId
                    left join ReportDeparmentData d ON empStatus.DepId = d.Id
                    left JOIN CostCodes CostCode ON empStatus.CostCodeId = CostCode.Id
                    left JOIN Employer Employer ON empStatus.EmployerId = Employer.Id

                    left JOIN Position p ON ProfileData.PositionId = p.Id
                    left JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id

                    inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                    ORDER BY CostCentre";

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
                else if(ItemData.FieldName == "EndDate")
                {
                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        endDate = result;
                    }
                }
            }

            query = query.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}");
            return await ManDaysToExcelFile(query, @"\Assets\GeneratedFiles\", reportName, reportParams);
        }

        #endregion


        #region SaveToExcel

        private async Task<string> ManDaysToExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams)
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

                    Dictionary<string, int> columnIndexByFieldName = new Dictionary<string, int>();


                    int columnIndex = 1;
                    var headerProps = ((IDictionary<string, object>)dynamicList[0]).Keys.Where(x=> x != "No").ToList();
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Data");

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
                        paramValueCells.Value = item.FieldValueCaption;
                        paramValueCells.Style.Font.Size = 12;
                        paramValueCells.Style.Font.Bold = true;


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
                        if (header != "No")
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
                            if (header == "TotalHours")
                            {
                                ExcelComment comment = worksheet.Cells[row, column].AddComment("TAS SYSTEM : Onsite days are calculated by multiplying 12 hours");
                                comment.AutoFit = true; 
                                comment.Fill.Color.SetColor(System.Drawing.Color.Yellow);
                            }


                            column++;
                        }

                    }

                    worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
                    row++;

                    string? currentCostCenter = "";
                    int daysOnsite = 0;
                    foreach (var d in dynamicList)
                    {
                        column = 1;

          
                        foreach (var prop in (IDictionary<string, object>)d)
                        {
                            if (prop.Key == "DaysOnsite")
                            {
                                daysOnsite = daysOnsite + Convert.ToInt32(prop.Value);
                            }


                            if (prop.Key != "No") // Check if prop.Key is not equal to "No"
                            {
                                if (prop.Value is DateTime dateTimeValue)
                                {
                                    // worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";

                                    column++;
                                }
                                else
                                {
                                    if (prop.Key == "DaysOnsite" || prop.Key == "TotalHours")
                                    {
                                        worksheet.Cells[row, column].Style.Numberformat.Format = "0.00";
                                    }
                                    worksheet.Cells[row, column].Value = prop.Value;
                                    column++;
                                }
                            }



                        }
                        row++;
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    // Save the new workbook
                    string fname = reportName + "-" + Guid.NewGuid().ToString() + ".xlsx";

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
