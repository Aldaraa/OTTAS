using Application.Common.Utils;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        #region Workflow_request

        public async Task<string> NonSiteHotelQueryModfy(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;
            string fields = "";

            foreach (var field in FieldNames)
            {


                fields += $", {field.FieldName} AS {field.FieldCaption}";


            }

            var employeeId = await GetUserId();
            var query = @$"SELECT
                              rdh.DateCreated 'RequestCompletedDate',
                              CASE  WHEN e.gender = 1 THEN 'MR' 
                              ELSE 'MS' END AS Gender,

                             e.Lastname 'SureName', Concat(e.Firstname, ' ', e.Lastname)  Firstname,
                                rd.Id 'RequestNumber',
                              e1.Description Employer, d.Name Department,
                              CONCAT(Approver.Firstname, ' ', Approver.Lastname) 'Tas Approver',
                              rtp.Description 'PurposeOfTravel',
                              CASE WHEN  rnsta.EarlyCheckIn = 1 THEN CONCAT(FORMAT(rnsta.FirstNight, 'yyyy-MM-dd'), ' 07:00') ELSE  FORMAT(rnsta.FirstNight, 'yyyy-MM-dd')   end AS  'Check In', 
                             CASE WHEN  rnsta.LateCheckOut = 1 THEN CONCAT(FORMAT(rnsta.LastNight, 'yyyy-MM-dd'), ' 15:00') ELSE  FORMAT(rnsta.LastNight, 'yyyy-MM-dd')   end AS  'Check Out', 
                              rnsta.Hotel,
                              DATEDIFF(DAY, rnsta.FirstNight, rnsta.LastNight) 'OverallStayingDays',
                              Concat(cc.Number, ' ' ,cc.Code) CostCode,
                               DATEDIFF(DAY, rnsta.FirstNight, rnsta.LastNight) *    rnsta.DayCost 'HotelCost',
                              rnsta.PaymentCondition,
                             Concat(Completed.Firstname, ' ', Completed.Lastname) 'Completed by traveller Officer',
                              ISNULL(rnsta.EarlyCheckInCost, 0) 'EarlyCheck-in',
                              ISNULL(rnsta.LateCheckOutCost, 0) 'LateCheck-out',
                              ISNULL(rnsta.AddCost , 0)'AddCost',
                              (DATEDIFF(DAY, rnsta.FirstNight, rnsta.LastNight) *  ISNULL(rnsta.DayCost, 0)) + ISNULL(rnsta.EarlyCheckInCost, 0) + ISNULL(rnsta.LateCheckOutCost, 0) + ISNULL(rnsta.AddCost, 0) 'InTotalCost',
                              rnsta.Comment
                              {fields}  
                             FROM RequestNonSiteTravelAccommodation rnsta 
                              left JOIN RequestDocument rd ON rnsta.DocumentId = rd.id
                              LEFT JOIN Employee e ON e.Id = rd.EmployeeId
                              left JOIN Employer e1 ON e.employerId = e1.Id
                              left join Department d ON e.DepartmentId = d.Id
                              left join CostCodes cc ON e.CostCodeId = cc.Id
                              left join RequestNonSiteTravel rnst ON rd.id = rnst.DocumentId
                              left JOIN RequestTravelPurpose rtp ON  rtp.Id = rnst.RequestTravelPurposeId
                          /*    left join ReportProfileData Profile on e.Id = Profile.PersonNo  */
                             inner join GetReportProfileData({employeeId}) Profile on e.Id = Profile.PersonNo


                              left JOIN (SELECT rdh.id, rdh.DocumentId, rdh.Comment,  rdh.UserIdCreated, rdh.DateCreated from RequestDocumentHistory rdh
                                WHERE rdh.CurrentAction = 'Completed' and rdh.DocumentId IN (SELECT Id FROM RequestDocument rd1 WHERE rd1.DocumentType = 'Non Site Travel'
                                  AND rd1.CurrentAction = 'Completed'
                                )
  
                              ) rdh ON rd.id = rdh.DocumentId 
                              left JOIN Employee Approver ON rd.UserIdCreated =Approver.Id
                              left JOIN Employee Completed ON rdh.UserIdCreated = Completed.Id
                            WHERE  rd.DocumentType = 'Non Site Travel'  AND rd.CurrentAction = 'Completed'
                             AND rnsta.DateCreated >= '[STARTDATE]' AND rnsta.DateCreated <= '[ENDDATE]'

                            ORDER BY rdh.DateCreated, e.Firstname;
                            ";


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


            return await SaveHotelToExcelFile(query, @"\Assets\GeneratedFiles\", "Non Site Hotel", reportParams);

        }

        #endregion


        #region SaveToExcelFile


        private async Task<string> SaveHotelToExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams)
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

                    var boldList = new List<string>() {"SureName", "Firstname", "RequestNumber" };

                    var decimalFormatColumns = new List<string>() { "InTotalCost", "AddCost", "HotelCost", "OverallStayingDays", "EarlyCheck-in", "LateCheck-out" };

                    foreach (var header in headerProps)
                    {
                        worksheet.Cells[row, column].Value = UsefulUtil.AddSpacesToSentence(header, true).ToUpper() ;
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
                                if (prop.Key == "RequestCompletedDate")
                                {
                                 //   worksheet.Cells[row, column].Value = dateTimeValue.ToString("MM/dd/yyyy");

                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "MM/dd/yyyy";


                                }
                                else
                                {
                                    //   worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                                }
                            }
                            else
                            {


                                if (boldList.IndexOf(prop.Key) > -1)
                                {
                                    worksheet.Cells[row, column].Style.Font.Bold = true;
                                }

                                if (decimalFormatColumns.IndexOf(prop.Key) > -1)
                                {
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "#,##0.00";
                                }
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


        #endregion

    }

}
