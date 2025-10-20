using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class CheckDataRepository : BaseRepository<Bed>, ICheckDataRepository
    {
        public CheckDataRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }


        public async Task<bool> CheckProfile(int EmployeeId, CancellationToken cancellationToken)
        {
            var currentEmployeeData = await Context.Employee.AsNoTracking().Where(x => x.Active == 1 && x.Id == EmployeeId).FirstOrDefaultAsync();
            if (currentEmployeeData != null)
            {

                //var staturequestEmployee =  await Context.StatusChangesEmployeeRequest.Where(x => x.EmployeeId == EmployeeId && x.StatusType == "DEACTIVE").FirstOrDefaultAsync();
                //if (staturequestEmployee != null) {
                //    throw new BadRequestException($"This employee is expected to be inactive on {staturequestEmployee?.EventDate.Value.ToString("yyyy-MM-dd")}. Action is not possible.\r\nPlease make a request to the admin team");
                //}
            

                return true;
            }
            else
            {
                throw new BadRequestException("This employee is inactive. Would you like to contact the admin team?");
            }
        }





    }


}

