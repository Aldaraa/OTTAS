using Application.Common.Utils;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography.Xml;
using System.Xml.Linq;
using System.Reflection.Metadata.Ecma335;
using Application.Common.Exceptions;
using Domain.CustomModel;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        #region TransportManifest

        private async Task<List<string>> TransportManifestQueryModfy(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var employeeId = await GetUserId();

            string fields = "";
            foreach (var field in FieldNames)
            {
                fields += $", {field.FieldName} AS {field.FieldCaption}";
            }


            var current = DateTime.Today;

            if (reportParams.Where(x => x.FieldName == "CurrentDate").FirstOrDefault() != null)
            {
                var dateParam = reportParams.Where(x => x.FieldName == "CurrentDate").FirstOrDefault();
                if (dateParam != null)
                {
                    var item =await GetParametervalue(dateParam);
                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        current = result;
                    }

                }
                

            }


            List<string> attachments = new List<string>();

            string transportScheduleQuery = @$"SELECT ts.Id, at.Code, ts.ETD, ts.ETA, Concat(fl.Code, tl.Code, ' ' +  ts.ETD, ' ',  at.Code, ' ', '{current.ToString("yyyy-MM-dd")}') fname, (SELECT count(*) FROM Transport t WHERE t.ScheduleId = ts.id AND t.Status = 'Confirmed')
                                            PassengerCount,(SELECT count(*) FROM Transport t WHERE t.ScheduleId = ts.id AND t.Status <> 'Confirmed')
                                            WaitPassengerCount, at.Direction, fl.Code fromLocation, tl.Code tolocation
                                            FROM TransportSchedule ts    LEFT join ActiveTransport at ON ts.ActiveTransportId = at.Id
                                            left JOIN Location fl ON at.fromLocationId =fl.Id left JOIN Location tl ON at.toLocationId =tl.Id
                                            WHERE CAST(ts.EventDate AS DATE) = '{current.ToString("yyyy-MM-dd")}'";




            if (reportParams.Where(x => x.FieldName == "TransportModeId").FirstOrDefault() != null)
            {
                var transportModeFilterValue = reportParams.Where(x => x.FieldName == "TransportModeId").Select(x => x.FieldValue).FirstOrDefault();
                if (transportModeFilterValue != "[0]" && transportModeFilterValue != null && transportModeFilterValue != null)
                {
                    string transportModeIds = transportModeFilterValue.Replace("[", "(").Replace("]", ")");
                    transportScheduleQuery = $"{transportScheduleQuery}  AND  at.TransportModeId IN {transportModeIds}";
                }

            }
            if (reportParams.Where(x => x.FieldName == "ActiveTransportId").FirstOrDefault() != null)
            {
                var activeTransportFilterValue = reportParams.Where(x => x.FieldName == "ActiveTransportId").Select(x => x.FieldValue).FirstOrDefault();
                if (activeTransportFilterValue != "[0]" && activeTransportFilterValue != "[]" && activeTransportFilterValue != null && activeTransportFilterValue != "")
                {
                    string activeTransportIds = activeTransportFilterValue.Replace("[", "(").Replace("]", ")");
                    transportScheduleQuery = $"{transportScheduleQuery}  AND  ts.ActiveTransportId IN {activeTransportIds}";
                }

            }

            if (reportParams.Where(x => x.FieldName == "LocationId").FirstOrDefault() != null)
            {
                var transportLocationFilterValue = reportParams.Where(x => x.FieldName == "LocationId").Select(x => x.FieldValue).FirstOrDefault();
                if (transportLocationFilterValue != "[0]" && transportLocationFilterValue != null && transportLocationFilterValue != "")
                {
                    string transportLocationId = transportLocationFilterValue.Replace("[", "").Replace("]", "");
                    transportScheduleQuery = $"{transportScheduleQuery}  AND   at.toLocationId = {transportLocationId}";
                }

            }
            var scheduleData = await GetTransportManifestScheduleData(transportScheduleQuery);
            foreach (var item in scheduleData)
            {


                var itemDictionary = (IDictionary<string, object>)item;

                var customReportParams = new List<ReportParam>();
                customReportParams.Add(new ReportParam { FieldCaption = "Departure Date", FieldValueCaption = current.ToString("dd/MM/yyyy") });


                if (itemDictionary.ContainsKey("Id"))
                {
                    var manifestquery = @$"SELECT   CONCAT(

                                            CASE
                                            WHEN e.gender = 1 THEN 'Mr.'
                                            ELSE 'Mrs.'
                                          END,
                                          ' ',

                                          e.Firstname, ' ', e.Lastname) PassengerName,
                                          ISNULL(ee.Description, Profile.Employer) Company,
                                          d.Name Department,
                                          p.Description Position,
                                          CONCAT(cc.Number, '-', cc.Description) CostCenter,
                                          '' Signatures,
                                          '' AS CNo, 
                                          t.Status,
                                          '' As 'Pick-upAddress'
                                
                                          {fields}  
                                         from Transport t
                                        left join Employee e ON t.EmployeeId = e.Id
                                        left JOIN Employer ee ON t.employerId = ee.Id
                                        left JOIN Department d ON e.DepartmentId = d.Id
                                        left JOIN Position p  ON e.PositionId = p.Id
                                        left JOIN CostCodes cc ON e.CostCodeId = cc.Id
                                      /*  left join ReportProfileData Profile on e.Id = Profile.PersonNo  */
                                        inner join GetReportProfileData({employeeId}) Profile on e.Id = Profile.PersonNo
                                          WHERE t.ScheduleId IN(SELECT Id
                                        FROM TransportSchedule
                                        WHERE t.ScheduleId = [scheduleId]
                                        ) ORDER BY e.Firstname";




                    string query =manifestquery.Replace("[scheduleId]", itemDictionary["Id"].ToString());

                    customReportParams.Add(new ReportParam { FieldCaption = "Transport Number", FieldValueCaption = itemDictionary["Code"].ToString() });
                    customReportParams.Add(new ReportParam { FieldCaption = "Number Of Passengers", FieldValueCaption = itemDictionary["PassengerCount"].ToString() });
                    customReportParams.Add(new ReportParam { FieldCaption = "Wait List Passenger", FieldValueCaption = itemDictionary["WaitPassengerCount"].ToString() });
                    customReportParams.Add(new ReportParam { FieldCaption = "Direction", FieldValueCaption = itemDictionary["Direction"].ToString() });

                    customReportParams.Add(new ReportParam { FieldCaption = "ETD Of Transport", FieldValueCaption = itemDictionary["ETD"].ToString() });

                    string fname =$"{itemDictionary["fname"]}";

                    var attachment = await SaveDynamicListToExcelFileTransportManifest(query, @"\Assets\GeneratedFiles\", fname, customReportParams);
                    // var attachment = await TestData(query, @"\Assets\GeneratedFiles\", reportName + $"    {itemDictionary["fromLocation"]}-{itemDictionary["tolocation"]}", customReportParams);

                    if (attachment != "" || attachment != string.Empty)
                    {
                        attachments.Add(attachment);
                    }
                }
            }



            return attachments;

        }


        public async Task<dynamic> GetTransportManifestScheduleData(string queryData)
        {
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = queryData;
                await Context.Database.OpenConnectionAsync();
                using (var result = await command.ExecuteReaderAsync())
                {
                    var dynamicList = new List<dynamic>();
                    int rowNumber = 0;
                    while (await result.ReadAsync())
                    {
                        dynamic d = new ExpandoObject();
                        d.No = ++rowNumber;
                        for (int i = 0; i < result.FieldCount; i++)
                        {
                            ((IDictionary<string, object>)d).Add(result.GetName(i), result[i]);
                        }
                        dynamicList.Add(d);
                    }

                    return dynamicList;
                }
            }
        }



        private string SanitizeFileName(string input, char additionalChar)
        {
            // Build the regular expression pattern dynamically to include the additional character
            string pattern = $"[\\s\\t{Regex.Escape(additionalChar.ToString())}]";

            // Replace matching characters with an underscore
            string sanitizedString = Regex.Replace(input, pattern, "_");

            return sanitizedString;
        }


        private async Task<string> SaveDynamicListToExcelFileTransportManifest(string queryData, string filePath, string reportName, List<ReportParam> reportParams)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var dynamicList = await ExecuteReportTransportManifestQuery(queryData, reportName);

            


            string fnamefix = SanitizeFileName(reportName, '-');


            using (var excelPackage = new ExcelPackage())
            {

                if (dynamicList.Count > 0)
                {




                    reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Report Executed : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                    reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = dynamicList.Count.ToString("N") });



                    // Add a new worksheet to the empty workbook
                    var headerProps = ((IDictionary<string, object>)dynamicList[0]).Keys.Where(c=> c != "Status").ToList();
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Data");

   

                    // Starting row for the data

                    worksheet.Cells[1, 1, 1, headerProps.Count].Merge = true;
                    var titleCells = worksheet.Cells[1, 1];
                    titleCells.Value = reportName;
                    titleCells.Style.Font.Bold = true;
                    titleCells.Style.Font.Size = 16;
                    titleCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    titleCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));
                    titleCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
                    int row = 2;
                    foreach (var item in reportParams)
                    {
                        row++;
                        var paramCaptionCells = worksheet.Cells[row, 2];
                        paramCaptionCells.Value = $"{item.FieldCaption}";
                        paramCaptionCells.Style.Font.Size = 12;

                        var paramValueCells = worksheet.Cells[row, 3];
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
                    worksheet.Cells[row, 1, row, headerProps.Count].Merge = true;
                    var descrcells = worksheet.Cells[row, 1, row, headerProps.Count];
                    descrcells.Value = "By signing below, I hold the airline responsible for my safety not OT LLC:";
                    descrcells.Style.Font.Size = 11;
                    descrcells.Style.Font.Bold = true;
                    descrcells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    descrcells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    descrcells.Style.Border.BorderAround(ExcelBorderStyle.Medium, ColorTranslator.FromHtml("#00aaad"));
                    row = row + 1;

                    try
                    {
                        FileInfo imageFile = new FileInfo(Directory.GetCurrentDirectory() + @"\Assets\image\logo.jpeg");
                        ExcelPicture excelImage = worksheet.Drawings.AddPicture("ImageName", imageFile);

                        excelImage.SetPosition(2, 0, 6, 0);
                        excelImage.SetSize(250, 125);
                    }
                    catch (Exception)
                    {

                    }





                    int column = 1;
                    worksheet.View.FreezePanes(row + 1, column);


                    /*Begin Confirmed passenngers print*/

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

                    foreach (var d in dynamicList)
                    {
                        column = 1;
                        foreach (var prop in (IDictionary<string, object>)d)
                        {
                            if (prop.Key != "Status")
                            {
                                if (prop.Value is DateTime dateTimeValue)
                                {

                                 //   worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd");

                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd";

                                }
                                else
                                {
                                    worksheet.Cells[row, column].Value = prop.Value;
                                }
                                column++;
                            }
                            
                        }
                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    // Save the new workbook
                    worksheet.PrinterSettings.Scale = 45;

                    // Configure print settings to fit the sheet on one page
                    worksheet.PrinterSettings.FitToPage = true;
                    worksheet.PrinterSettings.FitToWidth = 1;
                    worksheet.PrinterSettings.FitToHeight = 0;

                    string fname = fnamefix + ".xlsx";
                    string realPath = $"{Directory.GetCurrentDirectory()}{filePath}\\{fname}";
                    int counter = 1;

                    while (File.Exists(realPath))
                    {
                        fname = fnamefix + $"({counter++:D1}).xlsx"; 
                        realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";
                    }

                    FileInfo fileInfo = new FileInfo(realPath);
                    excelPackage.SaveAs(fileInfo);
                    return $"{filePath}\\{fname}";


                    //string fname = fnamefix + $"_{Guid.NewGuid().ToString()}.xlsx";

                    //var realPath = $@"{Directory.GetCurrentDirectory()}{filePath}\{fname}";

                    //var fileInfo = new FileInfo(realPath);
                    //excelPackage.SaveAs(fileInfo);

                    //return $"{filePath}\\{fname}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }


        private async Task<List<dynamic>> ExecuteReportTransportManifestQuery(string queryData, string queryDescr, int maxRetryCount = 3)
        {
            int attempt = 0;
            int killId = DateTime.Now.GetHashCode();


            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                if (sqlConnection == null)
                    throw new InvalidOperationException("Database connection is not of type SqlConnection.");

                while (true)
                {
                    try
                    {
                        if (sqlConnection.State == ConnectionState.Closed)
                        {
                            await sqlConnection.OpenAsync();
                        }

                        var sessionInfo = new SessionInfo { KillId = killId, SessionName = queryDescr, CreatedDate = DateTime.Now };
                        string queryProcessId = "SELECT CONVERT(INT, @@SPID)";
                        using (var commandProcessId = new SqlCommand(queryProcessId, sqlConnection))
                        {
                            int serverProcessId = (int)await commandProcessId.ExecuteScalarAsync();
                            sessionInfo.SessionId = serverProcessId;
                        }

                        await _sessionRepository.AddSessionAsync(sessionInfo);

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.CommandText = queryData;
                            command.CommandTimeout = 300;  // Consider adjusting based on typical query execution times

                            using (var result = await command.ExecuteReaderAsync())
                            {
                                var dynamicList = new List<dynamic>();
                                int rowNumber = 0;
                                while (await result.ReadAsync())
                                {
                                    dynamic d = new ExpandoObject();
                                    d.No = ++rowNumber;
                                    for (int i = 0; i < result.FieldCount; i++)
                                    {


                                        ((IDictionary<string, object>)d).Add(result.GetName(i), result[i]);
                                    }
                                    dynamicList.Add(d);
                                }
                                return dynamicList;
                            }
                        }
                    }
                    catch (SqlException ex) when (ex.Number == -2 && attempt < maxRetryCount) // SQL Server timeout exception number
                    {
                        attempt++;
                        await Task.Delay(1000 * attempt); // Exponential back-off
                        continue; // Retry the loop
                    }
                    catch (SqlException ex) when (ex.Number == 19) // Handle physical connection not usable
                    {
                        SqlConnection.ClearPool(sqlConnection);
                        throw new BadRequestException("The physical connection to the database is not usable.");
                    }
                    catch (Exception ex)
                    {
                        SqlConnection.ClearPool(sqlConnection);
                        throw new BadRequestException("A problem occurred while execute the report" + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection.State == ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                        await _sessionRepository.RemoveSessionsByKillIdLocal(killId);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Database connection is not of type SqlConnection.");
            }
        }







        #endregion

    }
}
