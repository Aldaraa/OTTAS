using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeFeature.DeActiveEmployee;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.DeleteStatusChangesEmployeeRequest;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestDeActive;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestReActive;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class StatusChangesEmployeeRequestRepository : BaseRepository<StatusChangesEmployeeRequest>, IStatusChangesEmployeeRequestRepository
    {
        private readonly HTTPUserRepository _HTTPUserRepository;
        public StatusChangesEmployeeRequestRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task<List<GetStatusChangesEmployeeRequestDeActiveResponse>> GetAllDeActiveData(CancellationToken cancellationToken)
        {
            var returnData =  new List<GetStatusChangesEmployeeRequestDeActiveResponse>();

            var data = await Context.StatusChangesEmployeeRequest.Where(x => x.StatusType == "DEACTIVE").ToListAsync(cancellationToken);

            foreach (var item in data)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId)
                    .Select(x => new { x.Firstname, x.Lastname }).FirstOrDefaultAsync();

                var createdEmployee = await Context.Employee.Where(x => x.Id == item.UserIdCreated)
                    .Select(x => new { x.Firstname, x.Lastname }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    var currentTermmination =await Context.RequestDeMobilisationType.Where(x => x.Id == item.TerminationTypeId).FirstOrDefaultAsync();
                    var newRecord = new GetStatusChangesEmployeeRequestDeActiveResponse
                    {
                        Id = item.Id,
                        Comment = item.Comment,
                        CreatedDate = item.DateCreated,
                        EmployeeId = item.EmployeeId,
                        EventDate = item.EventDate,
                        CreatedEmployeeName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}",
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                        TerminationTypeName = currentTermmination?.Description
                        
                    };

                    returnData.Add(newRecord);
                }


            }

            return returnData;
        }

        public async Task<List<GetStatusChangesEmployeeRequestReActiveResponse>> GetAllReActiveData(CancellationToken cancellationToken)
        {

            var returnData = new List<GetStatusChangesEmployeeRequestReActiveResponse>();

            var data = await Context.StatusChangesEmployeeRequest.Where(x => x.StatusType == "REACTIVE").ToListAsync(cancellationToken);

            foreach (var item in data)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId)
                    .Select(x => new { x.Firstname, x.Lastname }).FirstOrDefaultAsync();

                var createdEmployee = await Context.Employee.Where(x => x.Id == item.UserIdCreated)
                    .Select(x => new { x.Firstname, x.Lastname }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    var newRecord = new GetStatusChangesEmployeeRequestReActiveResponse
                    {
                        Id = item.Id,
                        CreatedDate = item.DateCreated,
                        EmployeeId = item.EmployeeId,
                        EventDate = item.EventDate,
                        CreatedEmployeeName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}",
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}"

                    };

                    returnData.Add(newRecord);
                }


            }

            return returnData;
        }


        public async Task DeleteStatusChangesEmployeeRequest(DeleteStatusChangesEmployeeRequestRequest request, CancellationToken cancellationToken)
        {
            if (_HTTPUserRepository.LogCurrentUser()?.Role == "SystemAdmin" || _HTTPUserRepository.LogCurrentUser()?.Role == "DataApproval")
            {
                var currentData = await Context.StatusChangesEmployeeRequest
                    .Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if (currentData != null) {             
                    Context.StatusChangesEmployeeRequest.Remove(currentData);
                }

                await Task.CompletedTask;
            }
            else {
                throw new BadRequestException("Sorry, only Administrator can delete");
            }
        }

    

    }

}
