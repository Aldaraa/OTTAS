using Application.Features.ReportJobFeature.TestReportJob;
using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Repositories;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Persistence.Context;
using Persistence.HostedService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Utils;
using System.Xml.Linq;
using OfficeOpenXml.Packaging.Ionic.Zip;
using Application.Features.ReportJobFeature.BuildReport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.SqlClient;
using Application.Features.ReportJobFeature.GetSessionList;
using Application.Features.ReportJobFeature.KillSession;
using System.Configuration;
using System.Data;
using Application.Common.Exceptions;
using Domain.CustomModel;
using System.Reflection.Emit;
using Newtonsoft.Json;
using static Azure.Core.HttpHeader;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Persistence.Repositories
{

    public partial class JobExecuteServiceRepository : BaseRepository<ReportJob>, IJobExecuteServiceRepository
    {
        #region Constructor

        JobHostedService _hostedService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ISessionRepository _sessionRepository;
        private readonly IReportJobRepository _jobRepository;



        public JobExecuteServiceRepository(DataContext context, IConfiguration configuration, JobHostedService hostedService, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ISessionRepository sessionRepository, IReportJobRepository reportJobRepository) : base(context, configuration)
        {
            _hostedService = hostedService;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _sessionRepository = sessionRepository;
            _jobRepository = reportJobRepository;
        }

        #endregion


        [Authorize]
        private async Task<int> GetUserId()
        {
            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {
                var roleData = await _jobRepository.GetUserRoleData(username);
                if (roleData != null)
                {
                    return roleData.EmployeeId.Value;
                }
                else
                {
                    return 0;
                    //throw new UnauthorizedAccessException("Access denied");
                }
            }
            else {
                return 0;
               // throw new UnauthorizedAccessException("Access denied");
            }

        }



        #region GetParameterData


        private async Task<ReportParam> GetParametervalue(ReportParam data)
        {
            if (data.FieldValue == "[0]" || data.FieldValue == "" || data.FieldValue == null || data.FieldValue == "[]")
            {
                data.FieldValueCaption = "ALL";
                data.FieldValue = "ALL";

            }
            else {

                if (!string.IsNullOrEmpty(data.FieldName) && data.FieldName.ToLower() == "costcodeid")
                {
                    string query = $"SELECT CONCAT(cc.Number,' - ',cc.Description) costCode from CostCodes cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption =   await ExecuteQueryGetParameterData(query, "costCode");
                }
                if (data.FieldName == "CampId")
                {
                    string query = $"SELECT cc.Description CampName from Camp cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "CampName");
                }

                if (data.FieldName == "DepartmentId")
                {

                    var departmentParam = JsonConvert.DeserializeObject<List<int>>(data.FieldValue);
                    if (departmentParam?.Count > 0)
                    {
                        List<int> departmentIds = await DepartmentHierarchyIds(departmentParam);
                        if (departmentIds.Count > 0) {

                            string departmentQueryParams = string.Join(", ", departmentIds);

                            string query = $"SELECT name  from Department cc where Id in ({departmentQueryParams})";
                            data.FieldValue = $"({departmentQueryParams})";

                            data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");

                        }
                        else {
                            data.FieldValueCaption = "ALL";
                            data.FieldValue = "ALL";

                        }
                    }
                    else {
                        data.FieldValueCaption = "ALL";
                        data.FieldValue = "ALL";

                    }



                }
                if (data.FieldName == "EmployerId")
                {
                    string query = $"SELECT CONCAT(cc.Code,' - ',cc.Description) name   from Employer cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "PositionId")
                {
                    string query = $"SELECT CONCAT(cc.Code,' - ',cc.Description) name   from Position cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "PeopleTypeId")
                {
                    string query = $"SELECT CONCAT(cc.Code,' - ',cc.Description) name   from PeopleType cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "NationalityId")
                {
                    string query = $"SELECT CONCAT(cc.Code,' - ',cc.Description) name   from Nationality cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "RosterId")
                {
                    string query = $"SELECT  name   from Roster cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "RosterId")
                {
                    string query = $"SELECT  name   from Roster cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "OnSite")
                {
                    if (data.FieldValue == "Yes")
                    {
                        data.FieldValue = data.FieldValue;

                        data.FieldValueCaption = data.FieldValue;
                    }
                    else {
                        data.FieldValue = "ALL";

                        data.FieldValueCaption = "No";
                    }
                    
                }
                if (data.FieldName == "TravelTime")
                {
                    data.FieldValue = data.FieldValue;

                    data.FieldValueCaption = data.FieldValue;
                }
                if (data.FieldName == "NationalityId")
                {
                    string query = $"SELECT CONCAT(cc.Code,' - ',cc.Description) name   from Nationality cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }

                if (data.FieldName == "GroupMasterId")
                {
                    string query = $"SELECT cc.Description name   from GroupDetail cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }

                if (data.FieldName == "StartDate" || data.FieldName == "EndDate")
                {
                    DateTime paramDate = UsefulUtil.CalculateReportParamDynamicDate(data.FieldValue, data.FieldName, data.Days);
                    data.FieldValueCaption = paramDate.ToString("yyyy-MM-dd");
                }
                if (data.FieldName == "CurrentDate")
                {
                    DateTime paramDate = UsefulUtil.CalculateReportParamDynamicDate(data.FieldValue, data.FieldName, data.Days);
                    data.FieldValueCaption = paramDate.ToString("yyyy-MM-dd");
                }
                if (data.FieldName == "LocationId")
                {
                    string query = $"SELECT CONCAT(cc.Code,' - ',cc.Description) name   from Location cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "RequestGroupId")
                {
                    string query = $"SELECT cc.Description name   from RequestGroup  cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }
                if (data.FieldName == "RoomTypeId")
                {
                    string query = $"SELECT cc.Description name   from RoomType  cc where Id in " + data.FieldValue.Replace("[", "(").Replace("]", ")");
                    data.FieldValue = data.FieldValue.Replace("[", "(").Replace("]", ")");

                    data.FieldValueCaption = await ExecuteQueryGetParameterData(query, "name");
                }



            }


            return data;





        }



        public async Task<string> ExecuteQueryGetParameterData(string queryData, string columnName)
        {

            try
            {
                using (var command = Context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = queryData;
                    await Context.Database.OpenConnectionAsync();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        var valuesList = new List<string>();
                        while (await result.ReadAsync())
                        {
                            // Assuming the column contains string data. If not, use Convert.ToString() or a similar method to ensure proper conversion.
                            valuesList.Add(Convert.ToString(result[columnName]));
                        }

                        // Use string.Join to create a comma-separated string from the valuesList

                       /* if (valuesList.Count > 3)
                        {
                            return string.Join(",", valuesList.Take(3).ToList()) + "...";
                        }
                        else {*/
                            return string.Join(",", valuesList);
                      //  }

                    }
                }
            }
            catch (Exception ex)
            {
                var aa = queryData;
                throw;
            }

        }




        #endregion

        #region GroupMasterData
        public async Task<string?> ExecuteGroupColumnData(string queryData)
        {

            try
            {
                using (var command = Context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = queryData;
                    await Context.Database.OpenConnectionAsync();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        var valuesList = new List<string>();
                        while (await result.ReadAsync())
                        {
                            valuesList.Add("[" + Convert.ToString(result["Description"]) + "]");
                        }

                        return string.Join(",", valuesList);

                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        #endregion

        #region ExecuteQuery


        private async Task<List<dynamic>> ExecuteReportQuery(string queryData, string queryDescr, int maxRetryCount = 3)
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
                                while (await result.ReadAsync())
                                {
                                    dynamic d = new ExpandoObject();
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

        #region SaveToExcelFile




        private async Task<string> SaveDynamicListToExcelFile(string queryData, string filePath, string reportName, List<ReportParam> reportParams)
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
                            else {
                                paramValueCells.Value = item.FieldValueCaption;
                            }



                        }
                        else {
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
                                if (prop.Key == "ToTravelDate" || prop.Key == "FromTravelDate" || prop.Key == "Date" || prop.Key == "LastTransportDate" || prop.Key == "EventDate")
                                {
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd";
                                }
                                else {
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
                    string fname = reportName + " " +  Guid.NewGuid().ToString() + ".xlsx";

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

        #region TestJob



        public async Task<TestReportJobResponse> TestJobSchedule(TestReportJobRequest request, CancellationToken cancellationToken)
        {
            var saveFilePaths = await ExecuteData(request.Id);
            var returnData = new TestReportJobResponse
            {
                ExcelFiles = new List<TestReportJobResponseData>()
            };


            foreach (var item in saveFilePaths.Files)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                if (string.IsNullOrWhiteSpace(item))
                    return new TestReportJobResponse();

                using (var package = new ExcelPackage())
                {
                    using (var stream = File.OpenRead(Directory.GetCurrentDirectory() + item))
                    {
                        package.Load(stream);
                    }

                    if (package.GetAsByteArray().Length > 0)
                    {

                        string reportPath = (Directory.GetCurrentDirectory() + item).Replace(item.Substring(item.LastIndexOf('\\')), "");



                        
                        returnData.ExcelFiles.Add(new TestReportJobResponseData { ExcelData = package.GetAsByteArray(), Filename = item.Substring(item.LastIndexOf('\\') + 1)});

                        await DeleteOldFiles(reportPath);
                    }

                }
            }



            return returnData;
        }


        private async Task DeleteOldFiles(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);

            // Create a list of tasks
            var tasks = new List<Task>();

            foreach (string file in files)
            {
                // Get the creation time of the file
                DateTime creationTime = File.GetCreationTime(file);


                if ((DateTime.Now - creationTime).TotalHours > 3)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            File.Delete(file);
                            Console.WriteLine($"Deleted file: {file}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                        }
                    }));
                }
            }

            // Wait for all the delete operations to complete
            await Task.WhenAll(tasks);


        }

        #endregion

      


        public async Task<List<GetSessionListResponse>> GetCurrentSessions(GetSessionListRequest request, CancellationToken cancellationToken)
        {

            var sessionList = await _sessionRepository.GetAllSessionsAsync();
            if (sessionList != null)
            {
                var returnData = sessionList.Select(x => new GetSessionListResponse
                {
                    KillId = x.KillId,
                    SessionId = x.SessionId,
                    CreatedDate = x.CreatedDate,
                    SessionName = x.SessionName
                }).ToList();

                return returnData;
            }
            else {
                return new List<GetSessionListResponse>();
            }

        }



        public async Task KillSessionForce(KillSessionRequest request, CancellationToken cancellationToken)
        {
             await  _sessionRepository.RemoveSessionsByKillIdAsync(request.killId, _configuration.GetConnectionString("DefaultConnection"));
        }


        #region BuildCommand


        public async Task<BuildReportResponse> CreateBuildCommand(BuildReportRequest request, CancellationToken cancellationToken)
        {

            var currentReportTemplate = await Context.ReportTemplate.Where(x => x.Id == request.reportTemplateId).FirstOrDefaultAsync();

            if (currentReportTemplate != null)
            {
                var reportColumns = new List<ReportCol>();

                if (request.ColumnIds != null)
                {
                    reportColumns = await Context.ReportTemplateColumn.Where(x => request.ColumnIds.Contains(x.Id))
                    .Select(x => new ReportCol
                    {
                        FieldName = x.FieldName,
                        FieldCaption = x.Caption

                    }).ToListAsync();
                }



                var paraMeterIds = request.Parameters.Select(x => x.ParameterId);

                var Params = new List<ReportParam>();
                string reportName = currentReportTemplate.Description;

                var reportParameters = await Context.ReportTemplateParameter
                    .Where(x => paraMeterIds.Contains(x.Id)).Select(x => new
                    {
                        FieldCaption = x.Caption,
                        FieldName = x.FieldName,
                        Id = x.Id

                    }).ToListAsync();

                foreach (var item in reportParameters)
                {
                    if (item != null)
                    {
                        var newParam = new ReportParam
                        {
                            FieldCaption = item.FieldCaption,
                            FieldName = item.FieldName,
                            FieldValue = request.Parameters.FirstOrDefault(x => x.ParameterId == item.Id)?.ParameterValue,
                            FieldValueCaption = item.FieldCaption,
                            Days = request.Parameters.FirstOrDefault(x => x.ParameterId == item.Id)?.Days
                        };
                        Params.Add(newParam);
                    }

                }

                var jobCode = $"{currentReportTemplate.Description} build report";



                if (currentReportTemplate.Code == "TASREPORT_109")
                {
                    var attachment = await Profilemaster(reportColumns, jobCode, Params);
                    return AttachmentToExcel(new List<string?> { attachment }, reportName);
                }

                else if (currentReportTemplate.Code == "TASREPORT_111")
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var attachmentList = await TransportManifestQueryModfy(reportColumns, jobCode, Params);
                    if (attachmentList.Count > 0)
                    {
                        return AttachmentToExcel(attachmentList, reportName);
                    }
                    else {
                        throw new BadRequestException("No data available for the report.");
                    }

                }
                else if (currentReportTemplate.Code == "TASREPORT_112")
                {
                    var attachment = await WorkflowQueryModfy(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                    
                }
                else if (currentReportTemplate.Code == "TASREPORT_105")
                {
                    var attachment = await GetMobilisationData(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }

                else if (currentReportTemplate.Code == "TASREPORT_101")
                {
                    var attachment = await AccommodationArrivals(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }

                }

                else if (currentReportTemplate.Code == "TASREPORT_115")
                {
                    var attachment = await AccommodationDepartures(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }

                }
                else if (currentReportTemplate.Code == "TASREPORT_106")
                {
                    var attachment = await OffsiteNoFutureBooking(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_108")
                {
                    var attachment = await Roster(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_104")
                {
                    var attachment = await ManDays(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_107")
                {
                    var attachment = await RoomOccupancy(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_102")
                {
                    var attachment = await WorkflowCompletedQueryModfy(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_110")
                {
                    var attachment = await TransportDetailsQueryModfy(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_113")
                {
                    var attachment = await NonSiteTravelQueryModfy(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_114")
                {
                    var attachment = await NonSiteHotelQueryModfy(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_116")
                {
                    var attachment = await RoomDateUtilization(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_117")
                {
                    var attachment = await TransportDetailsQuerySMS(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_118")
                {
                    var attachment = await FlightUtilization(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_119")
                {
                    var attachment = await SeatBlock(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_120")
                {
                    var attachment = await ProfileAudit(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }
                else if (currentReportTemplate.Code == "TASREPORT_121")
                {
                    var attachment = await ActionsTrends(reportColumns, jobCode, Params);
                    if (string.IsNullOrWhiteSpace(attachment) || attachment == string.Empty)
                    {
                        throw new BadRequestException("No data available for the report.");
                    }
                    else
                    {
                        return AttachmentToExcel(new List<string?> { attachment }, reportName);
                    }
                }

                else
                {
                    throw new BadRequestException("Report Coming soon");
                }


            }
            else {
                throw new BadRequestException("Job Schedule not available");
            }

        }


        private BuildReportResponse AttachmentToExcel(List<string?> attachments, string? fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BuildReportResponse
            {
                ExcelFiles = new List<BuildReportResponseExcelData>()
            };

            foreach (var item in attachments)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    using (var package = new ExcelPackage())
                    {
                        using (var stream = File.OpenRead(Directory.GetCurrentDirectory() + item))
                        {
                            package.Load(stream);
                        }

                        if (package.GetAsByteArray().Length > 0)
                        {
                            returnData.ExcelFiles.Add(new BuildReportResponseExcelData { ExcelData = package.GetAsByteArray(), Filename = item.Substring(item.LastIndexOf('\\') + 1) });

                         //   File.Delete(Directory.GetCurrentDirectory() +  item);
                        }
                    }

                }


            }
            returnData.ReportName = string.IsNullOrWhiteSpace(fileName) ? "TASREPORT" : fileName;

            return returnData;
        }


            #endregion

        }


   


    #region Param models



    public class ModfySheetReturnData
    {
        public ExcelWorksheet sheet { get; set; }


    }


    public class EmailReportModel
    {
        public string To { get; set; }
        public string Subject { get; set; }

        public string? JobCode { get; set; }
        public string? TemplateName { get; set; }

        public string Body { get; set; }

        public string ReportDate { get; set; }

        public int? JobId { get; set; }

        public List<string> Cc { get; set; } // List of email addresses for CC
        public List<string> Attachments { get; set; } // List of file paths for attachments
    }

    public class ReportCol
    {
        public string FieldName { get; set; }

        public string FieldCaption { get; set; }

    }


    public class ExcelMultiSheet
    {
        public string queryData { get; set; }

        public string sheetName { get; set; }
        public string reportName { get; set; }


    }



    public class ReportParam
    {
        public string? FieldName { get; set; }

        public string? FieldCaption { get; set; }


        public string? FieldValue { get; set; }


        public string? FieldValueCaption { get; set; }

        public int? Days { get; set; }

    }



    #endregion
}
