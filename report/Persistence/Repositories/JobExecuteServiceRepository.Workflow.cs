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
using Domain.Common;
using System.Diagnostics.Eventing.Reader;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        #region SaveSheet

        private async Task<string> SaveDynamicListToExcelFileMultiSheed(List<ExcelMultiSheet> sheetData, string filePath, List<ReportParam> reportParams, string reportFileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage())
            {

                bool hasData = false;

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
                        worksheet.TabColor = ColorTranslator.FromHtml("#6af22f");
                        // Starting row for the data

                        //worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
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
                               //     worksheet.Cells[row, column].Value = ((DateTime)prop.Value).ToString("yyyy-MM-dd");

                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd";
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

                    }




                }

                if (hasData)
                {
                    // Save the new workbook
                    string fname = reportFileName + Guid.NewGuid().ToString() + ".xlsx";

                    var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

                    var fileInfo = new FileInfo(realPath);
                    excelPackage.SaveAs(fileInfo);

                    return $"{filePath}\\{fname}";
                }
                else {
                    return string.Empty;
                }





            }

        }


        #endregion

        #region Workflow_request

        public async Task<string> WorkflowQueryModfy(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {

            var employeeId = await GetUserId();
            var startDate = DateTime.Today;
            var endDate = DateTime.Today;
            string fields = "";
            foreach (var field in FieldNames)
            {
                fields += $",{field.FieldName} AS '{field.FieldCaption}'";


            }
            string wheresubCondition = "";

            foreach (var item in reportParams)
            {

                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "EmployerId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        wheresubCondition += $" AND e.EmployerId IN {item.FieldValue}";

                    }
                }

                if (ItemData.FieldName == "PeopleTypeId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        wheresubCondition += $" AND e.PeopleTypeId IN {item.FieldValue}";
                    }


                }
                if (ItemData.FieldName == "DepartmentId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        wheresubCondition += $" AND e.DepartmentId IN {item.FieldValue}";

                    }
                }
                if (ItemData.FieldName == "RequestGroupId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        wheresubCondition += $" AND  rg.id IN {item.FieldValue}";

                    }
                }


            }


            var query = @$"SELECT rd.Id '#Document',    rd.DocumentType,    rd.CurrentAction,
                                CONCAT(e.Firstname, ' ', e.Lastname) AS Subject,   
                                CONCAT(ee.Firstname, ' ', ee.Lastname) AS Requester,
                                CONCAT(rg.Description,' ', CONCAT(e1.Firstname, ' ', e1.Lastname) ) AS Step,   
                                rhist.Comment,   
                                CONCAT(e1.Firstname, ' ', e1.Lastname) AS ActionedBy,   
                                e.Camp,    
                                rhist.DateCreated AS DateActioned
                                {fields}
                            FROM    
                                RequestDocument rd 
                            LEFT JOIN  
                                (SELECT      
                                     e.Id AS EmployeeID,      
                                     e.Lastname AS LastName,      
                                     e.Firstname AS FirstName,      
                                     c.Description AS Camp,
                                     e.DepartmentId,
                                     e.employerId,
                                     e.PeopleTypeId 
                                 FROM      
                                     Employee e     
                                 LEFT JOIN 
                                     Room r ON e.RoomId = r.Id     
                                 LEFT JOIN 
                                     Camp c ON r.CampId = c.Id) e    
                                ON e.EmployeeID = rd.EmployeeId 
                            LEFT JOIN    
                                employee ee ON ee.Id = rd.UserIdCreated 
                            LEFT JOIN    
                                RequestGroupConfig rgc ON rgc.id = rd.AssignedRouteConfigId 
                            LEFT JOIN    
                                RequestGroup rg ON rgc.GroupId = rg.id 
                            RIGHT JOIN 
                                (SELECT      
                                     rdh.DocumentId,      
                                     MAX(rdh.DateCreated) AS DateCreated,     
                                     rdh.CurrentAction,      
                                     rdh.Comment,      
                                     rdh.ActionEmployeeId   
                                 FROM      
                                     RequestDocumentHistory rdh   
                                 WHERE      
                                     rdh.DateCreated >= '[STARTDATE]'  AND     
                                     rdh.DateCreated <= '[ENDDATE]' AND    
                                     rdh.CurrentAction = '[ACTION]'   
                                 GROUP BY      
                                     rdh.DocumentId,     
                                     rdh.CurrentAction,     
                                     rdh.Comment,     
                                     rdh.ActionEmployeeId ) rhist ON rd.id = rhist.DocumentId 
                            LEFT JOIN 
                                Employee e1 ON rhist.ActionEmployeeId = e1.Id 
                            /*left join ReportProfileData Profile on e.EmployeeID = Profile.PersonNo*/
                             inner join GetReportProfileData({employeeId}) Profile on e.EmployeeID = Profile.PersonNo
                            WHERE   
                                rd.CurrentAction = '[ACTION]'  
                                [DOCTYPE]  [wheresubCondition]";

            string documentQueryCancelled = query.Replace("[ACTION]", "Cancelled");
            string documentQueryDeclined = query.Replace("[ACTION]", "Declined");

            if (reportParams.Where(x => x.FieldName == "StartDate").FirstOrDefault() != null)
            {
                var dateParam = reportParams.Where(x => x.FieldName == "StartDate").FirstOrDefault();
                if (dateParam != null)
                {
                    var item = await GetParametervalue(dateParam);
                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        startDate = result;
                    }

                }


            }

            //var ItemData = await GetParametervalue(item);
            if (reportParams.Where(x => x.FieldName == "EndDate").FirstOrDefault() != null)
            {
                var dateParam = reportParams.Where(x => x.FieldName == "EndDate").FirstOrDefault();
                if (dateParam != null)
                {
                    var item = await GetParametervalue(dateParam);
                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        endDate = result;
                    }

                }


            }





            if (reportParams.Where(x => x.FieldName == "DocumentType").FirstOrDefault() != null)
            {
                if (reportParams.Where(x => x.FieldName == "DocumentType").FirstOrDefault() != null)
                {
                    var docTypeFilterValue = reportParams.Where(x => x.FieldName == "DocumentType").Select(x => x.FieldValue).FirstOrDefault();
                    if (docTypeFilterValue != "[0]")
                    {
                        //string documentTypes = docTypeFilterValue.Replace("[", "(").Replace("]", ")");
                        //documentQueryCancelled = documentQueryCancelled.Replace("{DOCTYPE}", $"AND  rd.DocumentType IN {documentTypes}");
                        //documentQueryDeclined = documentQueryDeclined.Replace("{DOCTYPE}", $"AND  rd.DocumentType IN {documentTypes}");

                        documentQueryDeclined = documentQueryDeclined.Replace("[DOCTYPE]", "");
                        documentQueryCancelled = documentQueryCancelled.Replace("[DOCTYPE]", "");

                    }
                    else
                    {
                        documentQueryDeclined = documentQueryDeclined.Replace("[DOCTYPE]", "");
                        documentQueryCancelled = documentQueryCancelled.Replace("[DOCTYPE]", "");

                    }

                }
                else
                {
                    documentQueryDeclined = documentQueryDeclined.Replace("[DOCTYPE]", "");
                    documentQueryCancelled = documentQueryCancelled.Replace("[DOCTYPE]", "");

                }
            }
            else
            {
                documentQueryDeclined = documentQueryDeclined.Replace("[DOCTYPE]", "");
                documentQueryCancelled = documentQueryCancelled.Replace("[DOCTYPE]", "");

            }

            var exlist = new List<ExcelMultiSheet>();
            exlist.Add(new ExcelMultiSheet { queryData = documentQueryCancelled.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}").Replace("[wheresubCondition]", wheresubCondition), reportName = "Cancelled Document", sheetName = "Cancelled" });
            exlist.Add(new ExcelMultiSheet { queryData = documentQueryDeclined.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}").Replace("[wheresubCondition]", wheresubCondition), reportName = "Declined Document", sheetName = "Declined" });


            return await SaveDynamicListToExcelFileMultiSheed(exlist, @"\Assets\GeneratedFiles\", reportParams, "Workflow");


        }

        #endregion
    }
}
