using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.ExternalReferences;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.AuditFeature.GetEmployeeAudit;
using tas.Application.Features.AuditFeature.GetGroupMembersAudit;
using tas.Application.Features.AuditFeature.GetMasterAudit;
using tas.Application.Features.AuditFeature.GetRoomAudit;
using tas.Application.Features.AuditFeature.GetRoomAuditByRoom;
using tas.Application.Features.AuditFeature.GetTransportAudit;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class AuditRepository : BaseRepository<Audit>, IAuditRepository
    {
        private readonly IConfiguration _configuration;
        public AuditRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
        }


        #region RoomAuditByRoom 
        public async Task<GetRoomAuditByRoomResponse> GetRoomAuditByRoom(GetRoomAuditByRoomRequest request, CancellationToken cancellationToken) 
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var startDate = request.startDate.Date;
            var endDate = request.endDate.Date.AddDays(1).AddSeconds(-1);

            try
            {

                var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId).FirstOrDefaultAsync();

                if (currentRoom == null)
                {
                    throw new BadRequestException("Room not found");
                }

                var data = await (from auditData in Context.RoomAudit
                                  where auditData.RoomId == request.RoomId
                                        && auditData.DateCreated.Value.Date >= startDate.Date
                                        && auditData.DateCreated.Value.Date <= endDate.Date

                                  join emp in Context.Employee on auditData.EmployeeId equals emp.Id into empData
                                  from employee in empData.DefaultIfEmpty()

                                  join dep in Context.Department on employee.DepartmentId equals dep.Id into depData
                                  from department in depData.DefaultIfEmpty()

                                  join shift in Context.Shift on auditData.ShiftId equals shift.Id into shiftData
                                  from shift in shiftData.DefaultIfEmpty()

                                  join room in Context.Room on auditData.RoomId equals room.Id into roomData
                                  from room in roomData.DefaultIfEmpty()

                                  join bed in Context.Bed on auditData.BedId equals bed.Id into bedData
                                  from bed in bedData.DefaultIfEmpty()

                                  join roomtype in Context.RoomType on room.RoomTypeId equals roomtype.Id into roomTypeData
                                  from roomtype in roomTypeData.DefaultIfEmpty()

                                  join camp in Context.Camp on room.CampId equals camp.Id into campData
                                  from camp in campData.DefaultIfEmpty()


                                  join oldroom in Context.Room on auditData.OldRoomId equals oldroom.Id into oldroomData
                                  from oldroom in oldroomData.DefaultIfEmpty()

                                  join oldbed in Context.Bed on auditData.OldBedId equals oldbed.Id into oldbedData
                                  from oldbed in oldbedData.DefaultIfEmpty()

                                  join oldroomtype in Context.RoomType on oldroom.RoomTypeId equals oldroomtype.Id into oldroomTypeData
                                  from oldroomtype in oldroomTypeData.DefaultIfEmpty()

                                  join oldcamp in Context.Camp on oldroom.CampId equals oldcamp.Id into oldcampData
                                  from oldcamp in oldcampData.DefaultIfEmpty()

                                  join emplog in Context.Employee on auditData.UserIdCreated equals emplog.Id into emplogData
                                  from userCreated in emplogData.DefaultIfEmpty()

                                  select new
                                  {
                                      EventDate = auditData.EventDate.HasValue ? auditData.EventDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : string.Empty,
                                      ResourceNumber = auditData.EmployeeId,
                                      EmployeeName = employee != null ? $"{employee.Firstname} {employee.Lastname}" : string.Empty,
                                      DepartmentName = department != null ? department.Name : string.Empty,
                                      RoomNumber = room != null ? room.Number : string.Empty,
                                      BedCode = bed != null ? bed.Description : string.Empty,
                                      RoomType = roomtype != null ? roomtype.Description : string.Empty,
                                      CampName = camp != null ? camp.Description : string.Empty,

                                      OldRoomNumber = oldroom != null ? oldroom.Number : string.Empty,
                                      OldBedCode = oldbed != null ? oldbed.Description : string.Empty,
                                      oldRoomType = oldroomtype != null ? oldroomtype.Description : string.Empty,
                                      OldCampName = oldcamp != null ? oldcamp.Description : string.Empty,


                                      Shift = shift != null ? shift.Code : string.Empty,
                                      UserInfo = userCreated != null ? $"{userCreated.Firstname} {userCreated.Lastname}" : string.Empty,
                                      CreatedDate = auditData.DateCreated != null ? auditData.DateCreated.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : string.Empty,
                                      Reason = auditData.UpdateSource != null ? auditData.UpdateSource : string.Empty,
                                  }).OrderBy(x => x.ResourceNumber)
                                  .AsNoTracking()
                                  .ToListAsync<dynamic>();
                if (data.Count > 0)
                {

                    return new GetRoomAuditByRoomResponse
                    {
                        ExcelFile = ExcelExport($"Room Audit {currentRoom.Number}", data, request.startDate, request.endDate)
                    };

                }
                else
                {

                    return new GetRoomAuditByRoomResponse
                    {
                        ExcelFile = null
                    };
                }


            }
            catch (Exception ex)
            {

                throw new BadRequestException(ex.Message.ToString());
            }
        }


        #endregion


        #region RoomAudit


        public async Task<GetRoomAuditResponse> GetRoomAudit(GetRoomAuditRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var startDate = request.startDate.Date;
            var endDate = request.endDate.Date.AddDays(1).AddSeconds(-1);

            try
            {

                var data = await (from auditData in Context.RoomAudit
                                  where request.employeeIds.Contains(auditData.EmployeeId.Value)
                                        && auditData.DateCreated.Value.Date >= startDate.Date
                                        && auditData.DateCreated.Value.Date <= endDate.Date
                                        && auditData.RoomId != null

                                  join emp in Context.Employee on auditData.EmployeeId equals emp.Id into empData
                                  from employee in empData.DefaultIfEmpty()

                                  join dep in Context.Department on employee.DepartmentId equals dep.Id into depData
                                  from department in depData.DefaultIfEmpty()

                                  join shift in Context.Shift on auditData.ShiftId equals shift.Id into shiftData
                                  from shift in shiftData.DefaultIfEmpty()

                                  join room in Context.Room on auditData.RoomId equals room.Id into roomData
                                  from room in roomData.DefaultIfEmpty()

                                  join bed in Context.Bed on auditData.BedId equals bed.Id into bedData
                                  from bed in bedData.DefaultIfEmpty()

                                  join roomtype in Context.RoomType on room.RoomTypeId equals roomtype.Id into roomTypeData
                                  from roomtype in roomTypeData.DefaultIfEmpty()

                                  join camp in Context.Camp on room.CampId equals camp.Id into campData
                                  from camp in campData.DefaultIfEmpty()


                                  join oldroom in Context.Room on auditData.OldRoomId equals oldroom.Id into oldroomData
                                  from oldroom in oldroomData.DefaultIfEmpty()

                                  join oldbed in Context.Bed on auditData.OldBedId equals oldbed.Id into oldbedData
                                  from oldbed in oldbedData.DefaultIfEmpty()

                                  join oldroomtype in Context.RoomType on oldroom.RoomTypeId equals oldroomtype.Id into oldroomTypeData
                                  from oldroomtype in oldroomTypeData.DefaultIfEmpty()

                                  join oldcamp in Context.Camp on oldroom.CampId equals oldcamp.Id into oldcampData
                                  from oldcamp in oldcampData.DefaultIfEmpty()

                                  join emplog in Context.Employee on auditData.UserIdCreated equals emplog.Id into emplogData
                                  from userCreated in emplogData.DefaultIfEmpty()

                                  select new
                                  {
                                      EventDate = auditData.EventDate.HasValue ? auditData.EventDate.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : string.Empty,
                                      ResourceNumber = auditData.EmployeeId,
                                      EmployeeName = employee != null ? $"{employee.Firstname} {employee.Lastname}" : string.Empty,
                                      DepartmentName = department != null ? department.Name : string.Empty,
                                      RoomNumber = room != null ? room.Number : string.Empty,
                                      BedCode = bed != null ? bed.Description : string.Empty,
                                      RoomType = roomtype != null ? roomtype.Description : string.Empty,
                                      CampName = camp != null ? camp.Description : string.Empty,

                                      OldRoomNumber = oldroom != null ? oldroom.Number : string.Empty,
                                      OldBedCode = oldbed != null ? oldbed.Description : string.Empty,
                                      oldRoomType = oldroomtype != null ? oldroomtype.Description : string.Empty,
                                      OldCampName = oldcamp != null ? oldcamp.Description : string.Empty,


                                      Shift = shift != null ? shift.Code : string.Empty,
                                      UserInfo = userCreated != null ? $"{userCreated.Firstname} {userCreated.Lastname}" : string.Empty,
                                      CreatedDate = auditData.DateCreated != null ? auditData.DateCreated.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : string.Empty,
                                      Reason = auditData.UpdateSource != null ? auditData.UpdateSource : string.Empty,
                                  }).OrderBy(x => x.ResourceNumber)
                                  .AsNoTracking()
                                  .ToListAsync<dynamic>();
                if (data.Count > 0)
                {

                    return new GetRoomAuditResponse
                    {
                        ExcelFile = ExcelExport("Room Audit", data, request.startDate, request.endDate)
                    };

                }
                else {

                    return new GetRoomAuditResponse
                    {
                        ExcelFile = null
                    };
                }


            }
            catch (Exception ex)
            {

                throw new BadRequestException(ex.Message.ToString());
            }

        }

        #endregion


        #region TransportAudit





        public async Task<GetTransportAuditResponse> GetTransportAudit(GetTransportAuditRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                var startDate = request.startDate.Date;
                var endDate = request.endDate.Date.AddDays(1);

                var data = await (from auditData in Context.TransportAudit.AsNoTracking()
                                  where request.employeeIds.Contains(auditData.EmployeeId.Value)
                                        && auditData.DateCreated >= startDate
                                        && auditData.DateCreated <= endDate

                                  join emp in Context.Employee.AsNoTracking() on auditData.EmployeeId equals emp.Id into empData
                                  from employee in empData.DefaultIfEmpty()

                                  join dep in Context.Department.AsNoTracking() on employee.DepartmentId equals dep.Id into depData
                                  from department in depData.DefaultIfEmpty()

                                  join schedule in Context.TransportSchedule.AsNoTracking() on auditData.ScheduleId equals schedule.Id into scheduleData
                                  from schedule in scheduleData.DefaultIfEmpty()



                                  join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id into activetransportData
                                  from activetransport in activetransportData.DefaultIfEmpty()

                                  join transportMode in Context.TransportMode.AsNoTracking() on activetransport.TransportModeId equals transportMode.Id into modeData
                                  from transportMode in modeData.DefaultIfEmpty()


                                  join oldschedule in Context.TransportSchedule.AsNoTracking() on auditData.OldScheduleId equals oldschedule.Id into OldscheduleData
                                  from oldschedule in OldscheduleData.DefaultIfEmpty()

                                  join oldactivetransport in Context.ActiveTransport.AsNoTracking() on oldschedule.ActiveTransportId equals oldactivetransport.Id into oldactivetransportData
                                  from oldactivetransport in oldactivetransportData.DefaultIfEmpty()

                                  join oldtransportMode in Context.TransportMode.AsNoTracking() on oldactivetransport.TransportModeId equals oldtransportMode.Id into oldmodeData
                                  from oldtransportMode in oldmodeData.DefaultIfEmpty()



                                  join emplog in Context.Employee on auditData.UserIdCreated equals emplog.Id into emplogData
                                  from userCreated in emplogData.DefaultIfEmpty()

                                  select new
                                  {
                                      UserName = userCreated != null ? $"{userCreated.Firstname} {userCreated.Lastname}" : string.Empty,
                                      Department = department != null ? department.Name : string.Empty,
                                      EmployeeName = employee != null ? $"{employee.Firstname} {employee.Lastname}" : string.Empty,
                                      Description = schedule != null ? schedule.Description : string.Empty,
                                      ETA = schedule != null ? schedule.ETA : string.Empty,
                                      ETD = schedule != null ? schedule.ETD : string.Empty,
                                      CreatedDate = auditData.DateCreated != null ? auditData.DateCreated.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : string.Empty,
                                      TransportMode = transportMode != null ? transportMode.Code : string.Empty,
                                      Direction = auditData.Direction,
                                      ResNumber = auditData.EmployeeId,
                                      EventDate = auditData.EventDate != null ? auditData.EventDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                                      Reason = auditData.UpdateSource,

                                      TransportCode = activetransport != null ? activetransport.Code : string.Empty,
                                      OldETA = schedule != null ? oldschedule.ETA : string.Empty,
                                      OldETD = schedule != null ? oldschedule.ETD : string.Empty,
                                      OldDescription = schedule != null ? oldschedule.Description : string.Empty,
                                      OldDirection = auditData.Direction,
                                      OldTransportMode = oldtransportMode != null ? oldtransportMode.Code : string.Empty,
                                      OldTransportCode = oldactivetransport != null ? oldactivetransport.Code : string.Empty,
                                      OldTransportDate = auditData.OldTransportDate


                                  })
                                  .AsNoTracking()
                                  .ToListAsync<dynamic>();
                if (data.Count > 0)
                {

                    return new GetTransportAuditResponse
                    {
                        ExcelFile = ExcelExport("Transport Audit", data, request.startDate, request.endDate)
                    };

                }
                else
                {

                    return new GetTransportAuditResponse
                    {
                        ExcelFile = null
                    };
                }


            }
            catch (Exception ex)
            {

                throw new BadRequestException(ex.Message.ToString());
            }

        }

        #endregion


        #region ExcelExport

        private List<IDictionary<string, object>> ConvertToDictionaryList(List<dynamic> dynamicList)
        {
            var dictionaryList = new List<IDictionary<string, object>>();

            foreach (var item in dynamicList)
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var property in item.GetType().GetProperties())
                {
                    dictionary[property.Name] = property.GetValue(item, null);
                }
                dictionaryList.Add(dictionary);
            }

            return dictionaryList;
        }





        private byte[] ExcelExport(string sheetName, List<dynamic> objectData, DateTime startDate, DateTime endDate)
        {
            using (var package = new ExcelPackage())
            {

                var data = ConvertToDictionaryList(objectData);
                var headerProps = ((IDictionary<string, object>)data[0]).Keys;
                var auditParams = new List<ExcelAuditParam>();

                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Name ", FieldValueCaption = sheetName });
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Audit Executed date ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Result Count ", FieldValueCaption = data.Count.ToString("N") });
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Date range", FieldValueCaption = $"{startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}" });


                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                int row = 3;
                foreach (var item in auditParams)
                {
                    row++;
                    var paramCaptionCells = worksheet.Cells[row, 1];
                    paramCaptionCells.Value = $"{item.FieldCaption}";
                    paramCaptionCells.Style.Font.Size = 12;

                    var paramValueCells = worksheet.Cells[row, 2];
                    paramValueCells.Value = item.FieldValueCaption;
                    paramValueCells.Style.Font.Size = 12;
                    paramValueCells.Style.Font.Bold = true;
                    paramValueCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    if (item.FieldName == "#MetaData")
                    {
                        paramValueCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2233e3"));
                    }
                    else
                    {
                        paramValueCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));

                    }
                    paramValueCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));


                }


                row = row + 2;

                int column = 1;
                worksheet.View.FreezePanes(row + 1, column);
                List<string> mainColumns = new List<string>();
                mainColumns.Add("UserName");
                mainColumns.Add("Department");
                mainColumns.Add("EmployeeName");
                mainColumns.Add("Description");
                mainColumns.Add("ETA");
                mainColumns.Add("ETD");
                mainColumns.Add("CreatedDate");
                mainColumns.Add("TransportMode");
                mainColumns.Add("Direction");
                mainColumns.Add("ResNumber");
                mainColumns.Add("ResourceNumber");
                mainColumns.Add("EventDate");
                mainColumns.Add("Reason");




                foreach (var header in headerProps)
                {
                    worksheet.Cells[row, column].Value = AddSpacesToSentence(header, true);
                    var headerCells = worksheet.Cells[row, column];
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Font.Size = 13;
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    if (mainColumns.IndexOf(header) > -1)
                    {
                        headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));

                        headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
                    }
                    else {
                        headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ACBDCB"));
                        headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#201b2e"));

                    }


                    headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    headerCells.AutoFilter = true;
                    column++;
                }

                // To set AutoFilter on the entire column range
                worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
                row++;

                // Loop through each dynamic object and fill in the cells
                foreach (var d in data)
                {
                    column = 1;
                    foreach (var prop in (IDictionary<string, object>)d)
                    {
                        if (prop.Value is DateTime dateTimeValue)
                        {
                            if (prop.Key == "EventDate")
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
                        else {
                            worksheet.Cells[row, column].Value = prop.Value;
                        }



                       

                        column++;
                    }
                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();


                package.Save();
                return package.GetAsByteArray();
            }
        }


        private static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    {
                        newText.Append(' ');
                    }
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        #endregion


        #region EmployeeAudit

        public async Task<GetEmployeeAuditResponse> GetEmployeeAudit(GetEmployeeAuditRequest request, CancellationToken cancellationToken)
        {

            var returnData = new GetEmployeeAuditResponse();

            DateTime startDate = DateTime.Today.AddMonths(-6);
            DateTime enddate = DateTime.Today.AddDays(1);

            if (request.startDate.HasValue)
            {
                startDate = request.startDate.Value;
            }
            if (request.endDate.HasValue)
            {
                enddate = request.endDate.Value.AddDays(1);
            }

            var currentEmployee = await (from employee in Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId)
                                         join createdEmployee in Context.Employee.AsNoTracking()
                                         on employee.UserIdCreated equals createdEmployee.Id into createdEmployeeData
                                         from createdEmployee in createdEmployeeData.DefaultIfEmpty()
                                         select new
                                         {
                                             EmployeeId = createdEmployee != null ? createdEmployee.Id : (int?)null,
                                             Fullname = createdEmployee != null ? $"{createdEmployee.Firstname} {createdEmployee.Lastname}" : null,
                                             CreatedDate = employee.DateCreated
                                         }).FirstOrDefaultAsync();


            var data = await (from audit in Context.EmployeeAudit.Where(x => x.EmployeeId == request.EmployeeId && x.DateCreated >= startDate && x.DateCreated <= enddate)
                              join user in Context.Employee.AsNoTracking() on audit.UserId equals user.Id into userData
                              from user in userData.DefaultIfEmpty()
                              select new GetEmployeeAuditResponseData
                              {
                                  Id = audit.Id,
                                  DateCreated = audit.DateCreated,
                                  EmployeeId = audit.EmployeeId,
                                  Username = $"{user.Firstname} {user.Lastname}",
                                  UserId = user.Id,
                                  NewValues = audit.NewValues,
                                  OldValues = audit.OldValues,
                              }).OrderByDescending(x => x.DateCreated).ToListAsync();

            returnData.CreatedFullName = currentEmployee?.Fullname;
            returnData.CreatedUserId = currentEmployee?.EmployeeId;
            returnData.CreatedDate = currentEmployee?.CreatedDate;

            returnData.AuditData = data;


            return returnData;
        }

        #endregion


        #region MasterAudit

        public async Task<List<GetMasterAuditResponse>> GetMasterAudit(GetMasterAuditRequest request, CancellationToken cancellationToken)
        {
            DateTime startDate = DateTime.Today.AddMonths(-6);
            DateTime enddate = DateTime.Today.AddDays(1);

            if (request.startDate.HasValue)
            {
                startDate = request.startDate.Value;
            }
            if (request.endDate.HasValue)
            {
                enddate = request.endDate.Value.AddDays(1);
            }

            if (request.recordId.HasValue)
            {
                string Id = Convert.ToString(request.recordId.Value);

                var data = await (from audit in Context.Audit.Where(x => x.TableName == request.TableName && x.PrimaryKey == Id && x.DateCreated >= startDate && x.DateCreated <= enddate)
                                  join user in Context.Employee.AsNoTracking() on audit.UserId equals user.Id into userData
                                  from user in userData.DefaultIfEmpty()
                                  select new GetMasterAuditResponse
                                  {
                                      Id = audit.Id,
                                      DateCreated = audit.DateCreated,
                                      Username = $"{user.Firstname} {user.Lastname}",
                                      UserId = user.Id,
                                      NewValues = audit.NewValues,
                                      OldValues = audit.OldValues,
                                  }).OrderByDescending(x => x.DateCreated).ToListAsync();

                return data;
            }
            else {
                var data = await (from audit in Context.Audit.Where(x => x.TableName == request.TableName && x.DateCreated >= startDate && x.DateCreated <= enddate)
                                  join user in Context.Employee.AsNoTracking() on audit.UserId equals user.Id into userData
                                  from user in userData.DefaultIfEmpty()
                                  select new GetMasterAuditResponse
                                  {
                                      Id = audit.Id,
                                      DateCreated = audit.DateCreated,
                                      Username = $"{user.Firstname} {user.Lastname}",
                                      UserId = user.Id,
                                      NewValues = audit.NewValues,
                                      OldValues = audit.OldValues,
                                  }).OrderByDescending(x => x.DateCreated).ToListAsync();

                return data;
            }


        }



        #endregion


        #region GroupMembersAudit

        public async Task<List<GetGroupMembersAuditResponse>> GetGroupMembersAudit(GetGroupMembersAuditRequest request, CancellationToken cancellationToken)
        {
            var returnData = await (from audit in Context.GroupMembersAudit.AsNoTracking().Where(x => x.GroupMasterId == request.GroupMasterId && x.EmployeeId == request.EmployeeId)
                                    join groupdetail in Context.GroupDetail.AsNoTracking() on audit.GroupDetailId equals groupdetail.Id into groupdetailData
                                    from groupdetail in groupdetailData.DefaultIfEmpty()
                                    join changeuser in Context.Employee.AsNoTracking() on audit.UserIdCreated equals changeuser.Id into userdata
                                    from changeuser in userdata.DefaultIfEmpty()
                                    select new GetGroupMembersAuditResponse
                                    {
                                        Id = audit.Id,
                                        Action = audit.Action,
                                        CreatedDate = audit.DateCreated,
                                        ChangedUser = $"{changeuser.Firstname} {changeuser.Lastname}",
                                        Description = groupdetail.Description
                                    }).OrderByDescending(x => x.CreatedDate).ToListAsync();
            return returnData;
        }



        #endregion



    }


    #region Classes



    public class ExcelAuditParam
    {
        public string FieldName { get; set; }

        public string FieldCaption { get; set; }


        public string FieldValue { get; set; }


        public string FieldValueCaption { get; set; }

    }

    #endregion



}
