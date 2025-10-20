using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.SafeModeEmployeeStatusFeature.CreateEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.GetEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.SetDSEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.SetRREmployeeStatus;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class SafeModeEmployeeStatusRepository : BaseRepository<EmployeeStatus>, ISafeModeEmployeeStatusRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly IMemoryCache _memoryCache;
        public SafeModeEmployeeStatusRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, ICheckDataRepository checkDataRepository, IMemoryCache memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _checkDataRepository = checkDataRepository;
            _memoryCache = memoryCache;
        }


        #region UtulFunctions


        private async Task<int?> getRoomBedId(int roomId, DateTime eventDate)
        {

            var roomBeds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == roomId).OrderBy(x => x.Id).ToListAsync();
            foreach (var item in roomBeds)
            {
                var currentData = await Context.EmployeeStatus.AsNoTracking().FirstOrDefaultAsync(x => x.BedId == item.Id && x.EventDate == eventDate);
                if (currentData == null)
                {
                    return item.Id;
                }
            }
            return null;


        }


        #endregion


        #region CreateData


        public async Task CreateEmployeeStatus(CreateEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
            var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.ShiftId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                var currentData = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == request.EventDate.Date ).FirstOrDefaultAsync();
                if (currentData != null)
                {
                    if (currentShift.OnSite == 1)
                    {
                        var oldShiftData = await Context.Shift.AsNoTracking().Where(x => x.Id == currentData.ShiftId).FirstOrDefaultAsync(x => x.Id == request.ShiftId);

                        if (oldShiftData?.OnSite == 1)
                        {
                            if (!request.RoomId.HasValue)
                            {
                                currentData.ShiftId = request.ShiftId;
                                currentData.Active = 1;
                                currentData.ChangeRoute = "Updated safemode";
                                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentData.DateUpdated = DateTime.Now;
                                Context.EmployeeStatus.Update(currentData);
                            }
                            else
                            {


                                var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId.Value).FirstOrDefaultAsync();
                                if (currentRoom != null)
                                {
                                    var bedId = await getRoomBedId(currentRoom.Id, request.EventDate.Date);
                                    currentData.ShiftId = request.ShiftId;
                                    currentData.Active = 1;
                                    currentData.BedId = bedId;
                                    currentData.RoomId = request.RoomId.Value;
                                    currentData.ChangeRoute = "Updated safemode";
                                    currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    currentData.DateUpdated = DateTime.Now;
                                    Context.EmployeeStatus.Update(currentData);
                                }
                                else
                                {
                                    throw new BadRequestException("Room not found");
                                }
                            }



                        }
                        else
                        {
                            if (request.RoomId.HasValue)
                            {
                                var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId.Value).FirstOrDefaultAsync();
                                if (currentRoom != null)
                                {
                                    var bedId = await getRoomBedId(currentRoom.Id, request.EventDate.Date);
                                    currentData.ShiftId = request.ShiftId;
                                    currentData.Active = 1;
                                    currentData.BedId = bedId;
                                    currentData.RoomId = request.RoomId.Value;
                                    currentData.ChangeRoute = "Updated safemode";
                                    currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    currentData.DateUpdated = DateTime.Now;

                                    Context.EmployeeStatus.Update(currentData);
                                }
                                else
                                {
                                    throw new BadRequestException("Room not found");
                                }
                            }
                            else
                            {
                                throw new BadRequestException("Please provide a Room when selecting an onsite shift.");
                            }
                        }



                    }
                    else
                    {
                        currentData.ShiftId = request.ShiftId;
                        currentData.RoomId = null;
                        currentData.BedId = null;
                        currentData.Active = 1;
                        currentData.ChangeRoute = "Updated safemode";
                        currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        Context.EmployeeStatus.Update(currentData);
                    }

                }
                else
                {
                    if (currentShift.OnSite == 1)
                    {
                        if (request.RoomId.HasValue)
                        {

                            var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId.Value).FirstOrDefaultAsync();
                            if (currentRoom != null)
                            {
                                var bedId = await getRoomBedId(currentRoom.Id, request.EventDate.Date);
                                var employeeStatus = new EmployeeStatus
                                {
                                    EventDate = request.EventDate.Date,
                                    EmployeeId = request.EmployeeId,
                                    ShiftId = request.ShiftId,
                                    DepId = request.DepartmentId,
                                    Active = 1,
                                    CostCodeId = request.CostCodeId,
                                    EmployerId = request.EmployerId,
                                    PositionId = request.PositionId,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    RoomId = request.RoomId,
                                    BedId = bedId,
                                    DateCreated = DateTime.Now,
                                    ChangeRoute = $"Added safemode",

                                };
                                Context.EmployeeStatus.Add(employeeStatus);

                            }
                            else
                            {
                                throw new BadRequestException("Room not found");
                            }

                        }
                        else
                        {
                            throw new BadRequestException("Please provide a Room when selecting an onsite shift.");
                        }

                    }
                    else
                    {
                        var employeeStatus = new EmployeeStatus
                        {
                            EventDate = request.EventDate.Date,
                            EmployeeId = request.EmployeeId,
                            ShiftId = request.ShiftId,
                            DepId = request.DepartmentId,
                            Active = 1,
                            CostCodeId = request.CostCodeId,
                            EmployerId = request.EmployerId,
                            PositionId = request.PositionId,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            DateCreated = DateTime.Now,
                            ChangeRoute = $"Add data safemode",

                        };
                        Context.EmployeeStatus.Add(employeeStatus);



                    }
                }

            }
            else
            {
                throw new BadRequestException("Employee not found");
            }

            

        }

        #endregion

        #region GetData


        public async Task<GetEmployeeStatusResponse> GetEmployeeStatus(GetEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
             var currentData = await Context.EmployeeStatus.AsNoTracking()
                    .Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == request.EventDate.Date)
                    .FirstOrDefaultAsync();
            if (currentData != null)
            {

                if (currentData.RoomId != null)
                {
                    var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == currentData.RoomId).FirstOrDefaultAsync();
                    var returnData = new GetEmployeeStatusResponse
                    {
                        Id = currentData.Id,
                        EmployeeId = currentData.EmployeeId,
                        CostCodeId = currentData.CostCodeId,
                        DepartmentId = currentData.DepId,
                        EmployerId = currentData.EmployerId,
                        EventDate = currentData.EventDate,
                        PositionId = currentData.PositionId,
                        RoomId = currentData.RoomId,
                        ShiftId = currentData.ShiftId,
                        RoomNumber = currentRoom?.Number
                    };

                    return returnData;
                }
                else {
                    var returnData = new GetEmployeeStatusResponse
                    {
                        Id = currentData.Id,
                        EmployeeId = currentData.EmployeeId,
                        CostCodeId = currentData.CostCodeId,
                        DepartmentId = currentData.DepId,
                        EmployerId = currentData.EmployerId,
                        EventDate = currentData.EventDate,
                        PositionId = currentData.PositionId,
                        RoomId = currentData.RoomId,
                        ShiftId = currentData.ShiftId,
                        RoomNumber = null
                    };

                    return returnData;
                }

            }
            else {
                return new GetEmployeeStatusResponse();
            }
        }

        #endregion


        #region SETRR

        public async Task<int> SetRREmployeeStatus(SetRREmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == request.EventDate.Date).FirstOrDefaultAsync();
            var currentEmployee = await Context.Employee.AsNoTracking().Where(c => c.Id == request.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                var currentshift = await Context.Shift.AsNoTracking().Where(x => x.Code == "RR").FirstOrDefaultAsync();
                if (currentshift != null)
                {
                    if (currentData != null)
                    {
                        currentData.RoomId = null;
                        currentData.BedId = null;
                        currentData.ShiftId = currentshift.Id;
                        currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        currentData.DateUpdated = DateTime.Now;
                        currentData.ChangeRoute = "Safe mode updated";
                        Context.EmployeeStatus.Update(currentData);
                        await Context.SaveChangesAsync();
                        return currentshift.Id;
                    }
                    else
                    {
                        var newdata = new EmployeeStatus
                        {
                            EmployeeId = request.EmployeeId,
                            Active = 1,
                            ChangeRoute = "Safe mode created",
                            CostCodeId = currentEmployee.CostCodeId,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository?.LogCurrentUser()?.Id,
                            EmployerId = currentEmployee.EmployerId,
                            DepId = currentEmployee.DepartmentId,
                            PositionId = currentEmployee.PositionId,
                            EventDate = request.EventDate,
                            RoomId = null,
                            ShiftId = currentshift.Id
                        };

                        Context.EmployeeStatus.Add(newdata);
                        await Context.SaveChangesAsync();

                        return currentshift.Id;


                    }
                }
                else {
                    throw new BadRequestException("Current shift not found");
                }

            }
            else {
                throw new BadRequestException("Current employee not found");
            }
        }

        #endregion


        #region DS

        public async Task<int> SetDSEmployeeStatus(SetDSEmployeeStatusRequest request, CancellationToken cancellationToken) 
        {
            var currentData = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == request.EventDate.Date).FirstOrDefaultAsync();
            var currentEmployee = await Context.Employee.AsNoTracking().Where(c => c.Id == request.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                var currentshift = await Context.Shift.AsNoTracking().Where(x=> x.Code == "DS").FirstOrDefaultAsync();
                var virtualRoom =await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                if (currentshift != null)
                {
                    if (virtualRoom != null)
                    {
                        if (currentData != null)
                        {
                            currentData.RoomId = virtualRoom.Id;
                            currentData.BedId = null;
                            currentData.ShiftId = currentshift.Id;
                            currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            currentData.DateUpdated = DateTime.Now;
                            currentData.ChangeRoute = "Safe mode updated";
                            Context.EmployeeStatus.Update(currentData);
                            await Context.SaveChangesAsync();
                            return currentshift.Id;
                        }
                        else
                        {
                            var newdata = new EmployeeStatus
                            {
                                EmployeeId = request.EmployeeId,
                                Active = 1,
                                ChangeRoute = "Safe mode created",
                                CostCodeId = currentEmployee.CostCodeId,
                                DateCreated = DateTime.Now,
                                UserIdCreated = _HTTPUserRepository?.LogCurrentUser()?.Id,
                                EmployerId = currentEmployee.EmployerId,
                                DepId = currentEmployee.DepartmentId,
                                PositionId = currentEmployee.PositionId,
                                EventDate = request.EventDate,
                                RoomId = virtualRoom.Id,
                                ShiftId = currentshift.Id
                            };

                            Context.EmployeeStatus.Add(newdata);
                            await Context.SaveChangesAsync();
                            return currentshift.Id;
                        }
                    }
                    else {
                        throw new BadRequestException("Virtual room not found");
                    }


                }
                else
                {
                    throw new BadRequestException("Current shift not found");
                }


            }
            else
            {
                throw new BadRequestException("Current employee not found");
            }

        }

        #endregion



    }
}
