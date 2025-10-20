using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Application.Features.ProfileFieldFeature.GetAllProfileField;
using tas.Application.Features.ProfileFieldFeature.UpdateProfileField;
using tas.Application.Features.RequestAirportFeature.CreateRequestAirport;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class ProfileFieldRepository : BaseRepository<ProfileField>, IProfileFieldRepository
    { 
        private readonly HTTPUserRepository _hTTPUserRepository;
    

        public ProfileFieldRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _hTTPUserRepository = hTTPUserRepository;
        }


        public async Task<List<GetAllProfileFieldResponse>> GetAllData(GetAllProfileFieldRequest request, CancellationToken cancellationToken)
        {
            var query = Context.ProfileField
              .AsNoTracking() // If you're only reading the data, this can improve performance
              .Select(pf => new GetAllProfileFieldResponse
              {
                  Id = pf.Id,
                  ColumnName = pf.ColumnName,
                  Label = pf.Label,
                  Active = pf.Active,
                  DateCreated = pf.DateCreated, 
                  DateUpdated = pf.DateUpdated, 
                  FieldRequired = pf.FieldRequired,
                  FieldVisible  = pf.FieldVisible,
                  FieldReadOnly = pf.FieldReadOnly,
                  RequestRequired = pf.RequestRequired,
                  RequestVisible = pf.RequestVisible 
              });


            var list = await query.OrderBy(x=> x.ColumnName).ToListAsync(cancellationToken);

            return list;

        }

        public async Task UpdateProfileField(UpdateProfileFieldRequest request, CancellationToken cancellationToken)
        {
            var currentitem = await Context.ProfileField.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentitem != null)
            {
                currentitem.FieldRequired = request.FieldRequired == 1 ? request.FieldRequired : 0;
                currentitem.RequestRequired = request.RequestRequired == 1 ? request.RequestRequired : 0;
                currentitem.Label = request.Label;
                currentitem.FieldVisible = request.FieldVisible == 1 ? request.FieldVisible : 0;
                currentitem.RequestVisible = request.RequestVisible == 1 ? request.RequestVisible : 0;
                currentitem.FieldReadOnly = request.FielReadOnly == 1 ? request.FielReadOnly : 0;
                Context.ProfileField.Update(currentitem);

            }
            else {
                throw new BadRequestException("Profile field not found");
            }

            await Task.CompletedTask;
        }




    }
}
