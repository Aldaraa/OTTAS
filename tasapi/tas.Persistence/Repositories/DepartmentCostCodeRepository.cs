using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DepartmentCostCodeFeature.AddDepartmentCostCode;
using tas.Application.Features.DepartmentCostCodeFeature.DeleteDepartmentCostCode;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class DepartmentCostCodeRepository : BaseRepository<DepartmentCostCode>, IDepartmentCostCodeRepository
    {

        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public DepartmentCostCodeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;

        }



        public async Task AddDepartmentCostCode(AddDepartmentCostCodeRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentCostCode.Where(x => x.DepartmentId == request.DepartmentId && x.CostCodeId == request.CostCodeId).FirstOrDefaultAsync();
            if (currentData == null)
            {
                var existCostCode = await Context.CostCodes.AsNoTracking().Where(x => x.Id == request.CostCodeId).FirstOrDefaultAsync();
                if (existCostCode != null)
                {
                    DepartmentCostCode newRecord = new DepartmentCostCode()
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        DepartmentId = request.DepartmentId,
                        CostCodeId = request.CostCodeId
                    };

                    Context.DepartmentCostCode.Add(newRecord);
                }
                else {
                    throw new BadRequestException("CostCode not found.");
                }


            }
            else
            {
                throw new BadRequestException("CostCode is already registered in this department.");
            }

        }



        public async Task DeleteDepartmentCostCode(DeleteDepartmentCostCodeRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentCostCode
                .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {
                Context.DepartmentCostCode.Remove(currentData);
            }
            else
            {
                throw new BadRequestException("Record not found");
            }


        }


    }

}
