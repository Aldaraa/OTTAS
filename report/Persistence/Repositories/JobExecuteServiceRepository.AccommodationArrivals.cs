using Application.Common.Utils;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {

        private bool hasGroupMasterAccommodationArrivals = false;
        private async Task<string> AccommodationArrivals(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            var employeeId = await GetUserId();

                string fields = "";
            string fieldAliases = "";
            foreach (var field in FieldNames)
            {
                
                fields += $", {field.FieldName} AS {field.FieldCaption}";
                fieldAliases += $", {field.FieldCaption}";
            }

            var query = @$"SELECT ProfileData.Id 'Person #', CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname ) Name,
                            RoomType.Description AS RoomType, Room.Number AS Room,
                            b.Description BedNo,
                            Camp.Description AS Camp,
                            d.Name Department,
                            
                            ActiveTransport.Code AS Flight ,Schedule.ETA ArrivalTime, 
                          CASE WHEN ProfileData.RoomId = Room.Id THEN 'Yes' 
                          ELSE 'No' 
                           END AS RoomOwner,
                          DepartureTransport.EventDate DepartureDate ,
                          DepartureTransport.ETA DepartureTime,
                          Employer.Description Employer,
                          Position.Description Position,
                          r.Number RoomNumber,
                          ProfileData.SAPID,
                          case WHEN ProfileData.gender = 1 THEN 'Male' ELSE
                          'Female' END AS 'Gender',
                          ProfileData.NRN,
                           n.Description Nationality,
                          PeopleType.Code ResourceType,
                          s.Code Shift,
                          ProfileData.PersonalMobile
                          {fields} 
                          FROM Transport Transport 
                          LEFT join Employee ProfileData 
                           ON Transport.EmployeeId = ProfileData.Id
                          LEFT JOIN Position Position 
                          ON ProfileData.PositionId = Position.Id 
                          LEFT JOIN Employer Employer
                          ON ProfileData.employerId = Employer.Id 
                          LEFT JOIN PeopleType PeopleType 
                          ON ProfileData.PeopleTypeId = PeopleType.Id 
                          LEFT JOIN ReportDeparmentData d ON ProfileData.DepartmentId = d.Id
                          
                        left JOIN TransportSchedule Schedule ON Transport.ScheduleId = Schedule.id 
                        left JOIN ActiveTransport ActiveTransport ON Schedule.ActiveTransportId = ActiveTransport.Id 
                        left JOIN EmployeeStatus RoomData ON RoomData.EmployeeId = Transport.EmployeeId AND CAST(RoomData.EventDate AS DATE) = '[CURRENTDATE]' 
                        left JOIN Room Room ON RoomData.RoomId = Room.Id 
                        LEFT JOIN Bed b ON b.Id = RoomData.BedId
                        left join Nationality n ON ProfileData.NationalityId = n.Id
                        left JOIN RoomType RoomType ON Room.RoomTypeId = RoomType.Id 
                        /*left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo*/
                        inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo
                        [CAMPQUERY] 
                        
                        left JOIN(
                                SELECT 
                                    t1.EmployeeId, 
                                    MIN(t1.EventDate) AS EventDate,
                                    ts.ETA
                                FROM  
                                    Transport t1
                                LEFT JOIN 
                                    TransportSchedule ts 
                                ON 
                                    t1.ScheduleId = ts.id
                                WHERE 
                                    CAST(t1.EventDate AS DATE) > '[CURRENTDATE]' 
                                    AND t1.Direction = 'OUT' 
                                GROUP BY 
                                    t1.EmployeeId, ts.ETA
                                HAVING
                                    MIN(t1.EventDate) = (SELECT MIN(t2.EventDate) 
                                                         FROM Transport t2 
                                                         WHERE t2.EmployeeId = t1.EmployeeId
                                                           AND CAST(t2.EventDate AS DATE) > '[CURRENTDATE]' 
                                                           AND t2.Direction = 'OUT')



) DepartureTransport 
                        ON Transport.EmployeeId = DepartureTransport.EmployeeId 
                        left join Shift s ON RoomData.ShiftId = s.Id 
                        left join Room r ON ProfileData.RoomId = r.Id

                        WHERE CAST(transport.EventDate AS DATE) = '[CURRENTDATE]' AND transport.Direction = 'IN' 
                        ORDER BY ProfileData.Firstname;";


            var query2 = @$"SELECT Id 'Person #', Name, RoomType, Room, BedNo, Camp, Department,ArrivalTime,   RoomOwner, DepartureDate,  DepartureTime, Employer, Position, 
RoomNumber, SAPID, Gender, NRN, Nationality, ResourceType, Shift, PersonalMobile,[COLS] {fieldAliases}  from (SELECT ProfileData.Id, CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname ) Name,
                            RoomType.Description AS RoomType, Room.Number AS Room,
                            b.Description BedNo,
                            Camp.Description AS Camp,
                            d.Name Department,
                            
                            ActiveTransport.Code AS Flight ,Schedule.ETA ArrivalTime, 
                          CASE WHEN ProfileData.RoomId = Room.Id THEN 'Yes' 
                          ELSE 'No' 
                           END AS RoomOwner,
                          DepartureTransport.EventDate DepartureDate ,
                          DepartureTransport.ETA DepartureTime,
                          Employer.Description Employer,
                          Position.Description Position,
                          r.Number RoomNumber,
                          ProfileData.SAPID,
                          case WHEN ProfileData.gender = 1 THEN 'Male' ELSE
                          'Female' END AS 'Gender',
                          ProfileData.NRN,
                           n.Description Nationality,
                          PeopleType.Code ResourceType,
                          s.Code Shift,
                          ProfileData.PersonalMobile,
                         gm.Description AS GroupMasterDescription, 
                         gd.Description AS GroupDetailDescription
                          {fields} 
                          FROM Transport Transport 
                          LEFT join Employee ProfileData 
                           ON Transport.EmployeeId = ProfileData.Id
                          LEFT JOIN Position Position 
                          ON ProfileData.PositionId = Position.Id 
                          LEFT JOIN Employer Employer
                          ON ProfileData.employerId = Employer.Id 
                          LEFT JOIN PeopleType PeopleType 
                          ON ProfileData.PeopleTypeId = PeopleType.Id 
                          LEFT JOIN ReportDeparmentData d ON ProfileData.DepartmentId = d.Id
                          
                        left JOIN TransportSchedule Schedule ON Transport.ScheduleId = Schedule.id 
                        left JOIN ActiveTransport ActiveTransport ON Schedule.ActiveTransportId = ActiveTransport.Id 
                        left JOIN EmployeeStatus RoomData ON RoomData.EmployeeId = Transport.EmployeeId AND CAST(RoomData.EventDate AS DATE) = '[CURRENTDATE]' 
                        left JOIN Room Room ON RoomData.RoomId = Room.Id 
                        LEFT JOIN Bed b ON b.Id = RoomData.BedId
                        left join Nationality n ON ProfileData.NationalityId = n.Id
                        left JOIN RoomType RoomType ON Room.RoomTypeId = RoomType.Id 
                     /*   left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo*/

                        inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo


                        LEFT JOIN GroupMembers gmbr ON ProfileData.Id = gmbr.EmployeeId
                        LEFT JOIN GroupMaster gm ON gmbr.GroupMasterId = gm.Id
                        LEFT JOIN dbo.GroupDetail gd ON gmbr.GroupDetailId = gd.Id 
                        [CAMPQUERY] 
                        
                        left JOIN(
                                SELECT 
                                    t1.EmployeeId, 
                                    MIN(t1.EventDate) AS EventDate,
                                    ts.ETA
                                FROM  
                                    Transport t1
                                LEFT JOIN 
                                    TransportSchedule ts 
                                ON 
                                    t1.ScheduleId = ts.id
                                WHERE 
                                    CAST(t1.EventDate AS DATE) > '[CURRENTDATE]' 
                                    AND t1.Direction = 'OUT' 
                                GROUP BY 
                                    t1.EmployeeId, ts.ETA
                                HAVING
                                    MIN(t1.EventDate) = (SELECT MIN(t2.EventDate) 
                                                         FROM Transport t2 
                                                         WHERE t2.EmployeeId = t1.EmployeeId
                                                           AND CAST(t2.EventDate AS DATE) > '[CURRENTDATE]' 
                                                           AND t2.Direction = 'OUT')



) DepartureTransport 
                        ON Transport.EmployeeId = DepartureTransport.EmployeeId 
                        left join Shift s ON RoomData.ShiftId = s.Id 
                        left join Room r ON ProfileData.RoomId = r.Id

                        WHERE CAST(transport.EventDate AS DATE) = '[CURRENTDATE]' AND transport.Direction = 'IN' 
                        [GROUPDETAILDESCRIPTIONCONDITION]
                         ) x
                    PIVOT 
                    (
                        MAX(GroupDetailDescription)
                        FOR GroupMasterDescription IN ([COLS2])
                    ) p 
                    ORDER BY Name;";

            var queryMovement = $@"SELECT ProfileData.Id 'Person #', CONCAT(ProfileData.Firstname, ' ', ProfileData.Lastname ) Name,

                            yesterdaydata.Camp BeforeCamp,
                            yesterdaydata.RoomType BeforeRoomType,
                            yesterdaydata.Number BeforeRoom,
                            yesterdaydata.BedNo BeforeBedNo,
                            Camp.Description AS CurrentCamp,
                            RoomType.Description AS CurrentRoomType, Room.Number AS CurrentRoom,
                            CASE WHEN Room.VirtualRoom > 0 THEN NULL ELSE   b.Description END as CurrentBedNo,
                            CASE WHEN ProfileData.RoomId = Room.Id THEN 'Yes' 
                          ELSE 'No'  END AS RoomOwner,
                            d.Name Department,
                          Employer.Description Employer,
                          Position.Description Position,
                          case WHEN ProfileData.gender = 1 THEN 'Male' ELSE
                          'Female' END AS 'Gender',
                          ProfileData.NRN,
                           n.Description Nationality,
                          PeopleType.Code ResourceType,
                          s.Code Shift,
                          ProfileData.PersonalMobile
                           
                          FROM EmployeeStatus es 
                          LEFT join Employee ProfileData 
                           ON ES.EmployeeId = ProfileData.Id
                          LEFT JOIN Position Position 
                          ON ProfileData.PositionId = Position.Id 
                          LEFT JOIN Employer Employer
                          ON ProfileData.employerId = Employer.Id 
                          LEFT JOIN PeopleType PeopleType 
                          ON ProfileData.PeopleTypeId = PeopleType.Id 
                          LEFT JOIN ReportDeparmentData d ON ProfileData.DepartmentId = d.Id
                          left JOIN Room Room ON es.RoomId = Room.Id 
                          LEFT JOIN Bed b ON b.Id = es.BedId
                          left join Nationality n ON ProfileData.NationalityId = n.Id
                          left JOIN RoomType RoomType ON Room.RoomTypeId = RoomType.Id 
                          left join ReportProfileData Profile on ProfileData.Id = Profile.PersonNo
                         /* left JOIN Camp Camp ON Room.CampId = Camp.Id  */
                        [CAMPQUERY]
                          left join Shift s ON es.ShiftId = s.Id 

                          inner JOIN (
                          SELECT es.EmployeeId, es.RoomId, Camp.Description AS Camp, RoomType.Description AS RoomType, Room.Number, b.Description BedNo, b.id BedId  FROM EmployeeStatus es
                                                    left JOIN Room Room ON es.RoomId = Room.Id 
                                                    LEFT JOIN Bed b ON b.Id = es.BedId
                                                    left join Shift s ON es.ShiftId = s.Id
                                                    left JOIN RoomType RoomType ON Room.RoomTypeId = RoomType.Id 
                                                    /*left JOIN Camp Camp ON Room.CampId = Camp.Id  */
                                                    [CAMPQUERY]
                          
                           WHERE es.RoomId IS not NULL AND es.EventDate  = DATEADD(DAY, -1, '[CURRENTDATE]')) AS yesterdaydata
                           ON es.EmployeeId= yesterdaydata.EmployeeId AND ( es.BedId != yesterdaydata.BedId OR es.RoomId != yesterdaydata.RoomId)


						
                        left join Room r ON ProfileData.RoomId = r.Id
                        WHERE es.RoomId is not NULL AND es.EventDate = '[CURRENTDATE]'
                        ORDER BY Room.Number";




            string whereCondition = "";
            foreach (var item in reportParams)
            {
                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "CampId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        query = query.Replace("[CAMPQUERY]", $" RIGHT JOIN(SELECT Id, c.Description  from Camp c WHERE id IN  {item.FieldValue} )Camp  ON Room.CampId = Camp.Id");
                        query2 = query2.Replace("[CAMPQUERY]", $" RIGHT JOIN(SELECT Id, c.Description  from Camp c WHERE id IN  {item.FieldValue} )Camp  ON Room.CampId = Camp.Id");

                        queryMovement = queryMovement.Replace("[CAMPQUERY]", $" RIGHT JOIN(SELECT Id, c.Description  from Camp c WHERE id IN  {item.FieldValue} )Camp  ON Room.CampId = Camp.Id");
                    }
                    else {
                        query = query.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");
                        queryMovement = queryMovement.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");

                    }
                }
                if (ItemData.FieldName == "CurrentDate")
                {

                    if (DateTime.TryParseExact(item.FieldValueCaption, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        query = query.Replace("[CURRENTDATE]", result.ToString("yyyy-MM-dd"));
                        query2 = query2.Replace("[CURRENTDATE]", result.ToString("yyyy-MM-dd"));

                        queryMovement = queryMovement.Replace("[CURRENTDATE]", result.ToString("yyyy-MM-dd"));

                    }
                    else {
                        DateTime currentDdate = DateTime.Today.AddDays(1);
                        query = query.Replace("[CURRENTDATE]", currentDdate.ToString("yyyy-MM-dd"));
                        query2 = query2.Replace("[CURRENTDATE]", currentDdate.ToString("yyyy-MM-dd"));

                        queryMovement = queryMovement.Replace("[CURRENTDATE]", currentDdate.ToString("yyyy-MM-dd"));
                    }
                }
                if (ItemData.FieldName == "GroupMasterId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        var groupMasterQuery = @$"SELECT Id, gm.Description FROM GroupMaster gm WHERE gm.id IN (
                                SELECT gd.GroupMasterId FROM GroupDetail gd WHERE gd.id IN {ItemData.FieldValue})";
                        hasGroupMasterAccommodationArrivals = true;

                        var groupColumns = await ExecuteGroupColumnData(groupMasterQuery);
                        if (!string.IsNullOrWhiteSpace(groupColumns))
                        {
                            query2 = query2.Replace("[GROUPDETAILDESCRIPTIONCONDITION]", $" AND gd.id IN {ItemData.FieldValue}");
                            query2 = query2.Replace("[COLS]", groupColumns);
                            query2 = query2.Replace("[COLS2]", groupColumns);



                        }


                        //if (whereCondition == "")
                        //{
                        //    whereCondition = $" WHERE e.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";


                        //}
                        //else
                        //{
                        //    whereCondition += $" AND e.Id IN  (SELECT EmployeeId FROM GroupMembers gm WHERE gm.GroupDetailId IN  {item.FieldValue})";
                        //}


                    }

                }

            }

            if (hasGroupMasterAccommodationArrivals)
            {
                query2 = query2 + " " + whereCondition;
                queryMovement = queryMovement + " " + whereCondition;

                query2 = query2.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");
                queryMovement = queryMovement.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");


                var exlist = new List<ExcelMultiSheet>();
                exlist.Add(new ExcelMultiSheet { queryData = query2, sheetName = "Arrivals", reportName = "Arrivals" });
                exlist.Add(new ExcelMultiSheet { queryData = queryMovement, sheetName = "RoomMovement", reportName = "Room Movement" });


                return await SaveAccommodationArrivalsToExcelFile(exlist, @"\Assets\GeneratedFiles\", reportParams, "AccommodationArrivals");
            }
            else
            {


                query = query + " " + whereCondition;
                queryMovement = queryMovement + " " + whereCondition;

                query = query.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");
                queryMovement = queryMovement.Replace("[CAMPQUERY]", " left JOIN Camp Camp ON Room.CampId = Camp.Id ");


                var exlist = new List<ExcelMultiSheet>();
                exlist.Add(new ExcelMultiSheet { queryData = query, sheetName = "Arrivals", reportName = "Arrivals" });
                exlist.Add(new ExcelMultiSheet { queryData = queryMovement, sheetName = "RoomMovement", reportName = "Room Movement" });


                return await SaveAccommodationArrivalsToExcelFile(exlist, @"\Assets\GeneratedFiles\", reportParams, "AccommodationArrivals");
            }
        }



        #region SaveToExcelFile




        private async Task<string> SaveAccommodationArrivalsToExcelFile(List<ExcelMultiSheet> sheetData,  string filePath,  List<ReportParam> reportParams, string reportFileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Check if the list has data to write to Excel

            var beforeDayData = new List<string> { "BeforeRoom", "BeforeRoomType", "BeforeCamp", "BeforeBedNo" };
            var currentDayData = new List<string> { "CurrentRoom", "CurrentRoomType", "CurrentCamp", "CurrentBedNo" };


            using (var excelPackage = new ExcelPackage())
            {
                var noformatColumn = new List<string> { "SAP #" };

                var hasData = false;

                foreach (var sheetItem in sheetData)
                {
                    string reportName = sheetItem.reportName;


                    var dynamicList = await ExecuteReportQuery(sheetItem.queryData, reportName);
                    if (dynamicList.Count > 0)
                    {

                        reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Report Executed : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                        reportParams.Add(new ReportParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = dynamicList.Count.ToString("N") });



                        // Add a new worksheet to the empty workbook
                        var headerProps = ((IDictionary<string, object>)dynamicList[0]).Keys;
                        var worksheet = excelPackage.Workbook.Worksheets.Add(sheetItem.sheetName);
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
                                if (beforeDayData.IndexOf(prop.Key) > -1)
                                {
                                    worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFF00"));
                                }
                                else if (currentDayData.IndexOf(prop.Key) > -1) {
                                    worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00B050"));

                                }

                                if (prop.Value is DateTime dateTimeValue)
                                {
                                  //     worksheet.Cells[row, column].Value = dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                                    worksheet.Cells[row, column].Value = dateTimeValue;
                                    worksheet.Cells[row, column].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";

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
                    else
                    {
                        if (sheetItem.reportName == "Arrivals")
                        {
                            return string.Empty;    
                        }
                        

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
                else
                {
                    return string.Empty;
                }

            }
        }


        #endregion

    }
}
