using Application.Common.Utils;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {

        private async Task<string> GetMobilisationData(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddMonths(1);

            string fields = "";
            foreach (var field in FieldNames)
            {


                    fields += $", {field.FieldName} AS {field.FieldCaption}";
               

            }


            var employeeId = await GetUserId();

                var DeMobilisationQuery = @$"Select ProfileData.Id as 'Person #', ProfileData.CompletionDate,   Concat(ProfileData.Firstname,' ',ProfileData.Lastname) Name, 
                            Department.Name DepartmentName, Employer.Description EmployerName, PeopleType.Code PeopleType, SAPID as 'SAP #'

                            {fields} from Employee AS ProfileData
                            left JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id 
                            left JOIN Employer Employer ON ProfileData.employerId = Employer.Id 
                            left JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id 
                            left JOIN Room Room ON ProfileData.RoomId = Room.Id
                          /* left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo*/
                             inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo

                            ";

            var MobilisationQuery = $@"Select  ProfileData.Id as 'Person #', ProfileData.CommenceDate,   Concat(ProfileData.Firstname, ' ', ProfileData.Lastname) Name, 
                         Department.Name DepartmentName, Employer.Description EmployerName, PeopleType.Code PeopleType, SAPID as 'SAP #'

                            {fields} from Employee AS ProfileData
                            left JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id 
                            left JOIN Employer Employer ON ProfileData.employerId = Employer.Id
                            left JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id 
                            left JOIN Room Room ON ProfileData.RoomId = Room.Id 
                          /*  left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo*/
                             inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                            
                           ";



            string whereConditionMob = "";
            string whereConditionDeMob = "";


            foreach (var item in reportParams)
            {

            //   DateTime StartDate = DateTime.Now;
            //    DateTime EndDate = StartDate.AddMonths(1);

                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "CampId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        MobilisationQuery += $" LEFT JOIN Camp  ON ProfileData.RoomId in (select Id from room where CampId in {ItemData.FieldValue} )";
                        DeMobilisationQuery += $" LEFT JOIN Camp  ON ProfileData.RoomId in (select Id from room where CampId in {ItemData.FieldValue} ) ";

                    }
                    else
                    {
                        MobilisationQuery += $" LEFT JOIN Camp  ON ProfileData.RoomId in (select Id from room)";
                        DeMobilisationQuery += $" LEFT JOIN Camp  ON ProfileData.RoomId in (select Id from room) ";

                    }
                }
               if (ItemData.FieldName == "StartDate")
                {
                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        startDate = result;
                    }
                }
              if (item.FieldName == "EndDate")
                {
                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        endDate = result;
                    }
                }

                if (ItemData.FieldName == "EmployerId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {

                        whereConditionDeMob += $" AND ProfileData.EmployerId IN {item.FieldValue}";
                        whereConditionMob += $" AND ProfileData.EmployerId IN {item.FieldValue}";


                    }


                }

                if (ItemData.FieldName == "DepartmentId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereConditionDeMob == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereConditionDeMob = $" WHERE ProfileData.DepartmentId IN {item.FieldValue}";
                                whereConditionMob = $" WHERE ProfileData.DepartmentId IN {item.FieldValue}";

                            }

                        }
                        else
                        {
                            whereConditionDeMob += $" AND ProfileData.DepartmentId IN {item.FieldValue}";
                            whereConditionMob += $" AND ProfileData.DepartmentId IN {item.FieldValue}";

                        }
                    }
                }
            }
            if (whereConditionDeMob == "")
            {
                    whereConditionDeMob = $" where  profileData.Active = 0 ";
                    whereConditionMob = $" where    profileData.Active = 1 ";
            }
            else
            {
                whereConditionDeMob += $" AND profileData.Active = 0 ";
                whereConditionMob += $" AND profileData.Active = 1 ";

            }

                whereConditionDeMob += $" AND CAST(ProfileData.CompletionDate AS DATE)  >= '[STARTDATE]' AND CAST(ProfileData.CompletionDate AS DATE) <= '[ENDDATE]'";
                whereConditionMob += $" AND CAST(ProfileData.CommenceDate AS DATE)  >= '[STARTDATE]' AND CAST(ProfileData.CommenceDate AS DATE) <= '[ENDDATE]'";

           



            DeMobilisationQuery = DeMobilisationQuery + " " + whereConditionDeMob  + " order by ProfileData.FirstName";
            MobilisationQuery = MobilisationQuery + " " + whereConditionMob  + " order by ProfileData.FirstName";


            var exlist = new List<ExcelMultiSheet>();
            exlist.Add(new ExcelMultiSheet { queryData = MobilisationQuery.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}"), reportName = "Mobilisation Report", sheetName = "Mobilised" });



            exlist.Add(new ExcelMultiSheet { queryData = DeMobilisationQuery.Replace("[STARTDATE]", $"{startDate.ToString("yyyy-MM-dd")}").Replace("[ENDDATE]", $"{endDate.ToString("yyyy-MM-dd")}"), reportName = "Demobilisation Report", sheetName = "Demobilised" });


            return await SaveMobilisationExcelFile(exlist, @"\Assets\GeneratedFiles\", reportParams, "Mobilisation-DeMobilisation");


        }


        #region SaveSheet

        private async Task<string> SaveMobilisationExcelFile(List<ExcelMultiSheet> sheetData, string filePath, List<ReportParam> reportParams, string reportFileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage())
            {
                var noformatColumn = new List<string>{"SAP #" };
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
       
                            if (value != null &&  value is DateTime)
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
                                if (prop.Value is DateTime)
                                {

                                    worksheet.Cells[row, column].Value = ((DateTime)prop.Value).ToString("yyyy-MM-dd");
                        
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
                    string fname = reportFileName + "_" + Guid.NewGuid().ToString() + ".xlsx";

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
    }
}
