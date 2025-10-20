using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestLineManagerEmployeeFeature.CreateRequestLineManagerEmployee;
using tas.Application.Features.RequestLineManagerEmployeeFeature.GetRequestLineManagerEmployee;
using tas.Application.Features.RequestLineManagerEmployeeFeature.RemoveRequestLineManagerEmployee;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class RequestLineManagerEmployeeRepository : BaseRepository<RequestLineManagerEmployee>, IRequestLineManagerEmployeeRepository
    {
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestLineManagerEmployeeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _HTTPUserRepository = hTTPUserRepository;
        }



        public async Task<List<GetRequestLineManagerEmployeeResponse>> GetRequestLineManagerEmployee(GetRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken)
        {

            var returnData = new List<GetRequestLineManagerEmployeeResponse>();
           await Context.RequestLineManagerEmployee.ToListAsync();

            var result = await (from lineEmployees in Context.RequestLineManagerEmployee.AsNoTracking()
                                join employees in Context.Employee.AsNoTracking() on lineEmployees.EmployeeId equals employees.Id
                                join employeesLineManager in Context.Employee.AsNoTracking() on lineEmployees.LineManagerId equals employeesLineManager.Id
                                select new GetRequestLineManagerEmployeeResponse
                                {
                                    Id = lineEmployees.Id,
                                    EmployeeId = lineEmployees.EmployeeId,
                                    EmployeeFullName = $"{employees.Firstname} {employees.Lastname}",
                                    LineManagerEmployeeId = lineEmployees.LineManagerId,
                                    LineManagerFullName = $"{employeesLineManager.Firstname} {employeesLineManager.Lastname}"
                                }).ToListAsync();



            return result;
            
        }


        public async Task CreateRequestLineManagerEmployee(CreateRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken)
        {
          var currentData =await  Context.RequestLineManagerEmployee
                .Where(x => x.LineManagerId == request.LineManagerEmployeeId && x.EmployeeId == request.EmployeeId)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentData == null)
            {
                var newRecord = new RequestLineManagerEmployee
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    LineManagerId = request.LineManagerEmployeeId,
                    EmployeeId = request.EmployeeId,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                };
                Context.RequestLineManagerEmployee.Add(newRecord);


            }

            await Task.CompletedTask;
        }



        public async Task RemoveRequestLineManagerEmployee(RemoveRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestLineManagerEmployee
                  .Where(x => x.Id == request.Id)
                  .FirstOrDefaultAsync();
            if (currentData != null)
            {
                Context.RequestLineManagerEmployee.Remove(currentData);
            }

            await Task.CompletedTask;
        }

    }




}
