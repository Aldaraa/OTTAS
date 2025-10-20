using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Features.RequestGroupFeature.CreateRequestGroup;
using tas.Application.Features.RequestGroupFeature.GetAllRequestGroup;
using tas.Application.Features.RequestGroupFeature.UpdateRequestGroup;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestGroupRepository : BaseRepository<RequestGroup>, IRequestGroupRepository
    {
        public readonly HTTPUserRepository _hTTPUserRepository;
        public RequestGroupRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _hTTPUserRepository = hTTPUserRepository;
        }


        public async Task<List<GetAllRequestGroupResponse>> GetAllData(GetAllRequestGroupRequest request, CancellationToken cancellationToken)
        {
           var Data =await Context.RequestGroup.ToListAsync(cancellationToken);

            var returnData = new List<GetAllRequestGroupResponse>();
            foreach (var item in Data)
            {
                var empcount = await Context.RequestGroupEmployee.CountAsync(x => x.RequestGroupId == item.Id);
                var newRecord = new GetAllRequestGroupResponse
                {
                    Id = item.Id,
                    Description = item.Description,
                    Active = item.Active,
                    DateCreated = item.DateCreated,
                    DateUpdated = item.DateUpdated,
                    ReadOnly = item.ReadOnly,
                    EmployeeCount  = empcount
                };

                returnData.Add(newRecord);

            }

            return returnData;

        }

        public async Task CreateData(CreateRequestGroupRequest request, CancellationToken cancellationToken)
        {
            var oldData = await Context.RequestGroup.Where(x => x.Description == request.Description).FirstOrDefaultAsync();
            if (oldData == null)
            {
                var newRecord = new RequestGroup
                {
                    Description = request.Description,
                    DateCreated = DateTime.Now,
                    ReadOnly = 0,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id

                };

                Context.RequestGroup.Add(newRecord);
            }
            else {
                throw new BadRequestException("Approval group has already been registered.");
            }
        }


        public async Task UpdateData(UpdateRequestGroupRequest request, CancellationToken cancellationToken)
        {
            var oldData = await Context.RequestGroup.Where(x => x.Description == request.Description && x.Id != request.Id).FirstOrDefaultAsync();
            if (oldData == null)
            {

                    var currentData = await Context.RequestGroup
                  .Where(x => x.Id == request.Id).FirstOrDefaultAsync();

                    if (currentData != null)
                    {
                        currentData.DateUpdated = DateTime.Now;
                        currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        currentData.Description = request.Description;
                        Context.RequestGroup.Update(currentData);
                    }
                    else
                    {
                        throw new BadRequestException("Approval group not found.");
                    }
               
              
            }
            else
            {
                throw new BadRequestException("Approval group has already been registered.");
            }
        }


    }


}