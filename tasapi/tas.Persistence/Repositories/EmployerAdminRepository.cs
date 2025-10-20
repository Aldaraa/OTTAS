using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployerAdminFeature.AddEmployerAdmin;
using tas.Application.Features.EmployerAdminFeature.DeleteEmployerAdmin;
using tas.Application.Features.EmployerAdminFeature.GetEmployerAdmin;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{


    public class EmployerAdminRepository : BaseRepository<EmployerAdmin>, IEmployerAdminRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        public EmployerAdminRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
        }

        public async Task AddEmployerAdmin(AddEmployerAdminRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.EmployerAdmin.Where(x => x.EmployerId == request.EmployerId && x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync(cancellationToken);
            if (currentData == null)
            {
                EmployerAdmin newRecord = new EmployerAdmin()
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _userRepository.LogCurrentUser()?.Id,
                    EmployerId = request.EmployerId,
                    EmployeeId = request.EmployeeId
                };

                Context.EmployerAdmin.Add(newRecord);


                var currentUser = await Context.Employee.Where(x => x.Id == request.EmployerId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _userRepository.ClearRoleCache(currentUser.ADAccount);
                }
            }
            else
            {
                throw new BadRequestException("Employee is already registered in this Employer.");
            }
        }

        public async Task DeleteEmployerAdmin(DeleteEmployerAdminRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.EmployerAdmin
              .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {
                Context.EmployerAdmin.Remove(currentData);
                var currentUser = await Context.Employee.AsNoTracking().Where(x => x.Id == currentData.EmployeeId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _userRepository.ClearRoleCache(currentUser.ADAccount);
                }

            }
            else
            {
                throw new BadRequestException("Record not found");
            }

        }


        public async Task<List<GetEmployerAdminResponse>> GetEmployerAdmin(GetEmployerAdminRequest request, CancellationToken cancellationToken)
        {

            return await (from sysRoleData in Context.EmployerAdmin.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                          join Employer in Context.Employer.AsNoTracking() on sysRoleData.EmployerId equals Employer.Id into depData
                          from Employer in depData.DefaultIfEmpty()
                          select new GetEmployerAdminResponse
                          {
                              Id = sysRoleData.Id,
                              EmployerId = Employer.Id,
                              Name = Employer.Description,
                              DateCreated = sysRoleData.DateCreated

                          }).ToListAsync();

        }
    }


}
