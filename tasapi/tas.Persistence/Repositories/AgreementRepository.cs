using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.AgreementFeature.CreateAgreement;
using tas.Application.Features.AgreementFeature.GetAgreement;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{


    public class AgreementRepository : BaseRepository<Agreement>, IAgreementRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _userRepository;    
        public AgreementRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _userRepository = hTTPUserRepository;
        }



        public async Task CreateData(CreateAgreementRequest request, CancellationToken cancellationToken)
        {
            var data = await Context.Agreement.FirstOrDefaultAsync();
            var returnData = new GetAgreementResponse();

            if (data != null)
            {
                data.AgreementText = request.AgreementText;
                data.DateUpdated = DateTime.Now;
                data.UserIdUpdated = _userRepository.LogCurrentUser()?.Id;
                Context.Agreement.Update(data);

            }
            else {

                var newRecord = new Agreement();
                newRecord.Active = 1;
                newRecord.DateCreated = DateTime.Now;
                newRecord.AgreementText = request.AgreementText;
                newRecord.UserIdCreated = _userRepository.LogCurrentUser()?.Id;

                Context.Agreement.Add(newRecord);


            }
        }
        public async Task<GetAgreementResponse> GetData(GetAgreementRequest request, CancellationToken cancellationToken)
        {
            var data = await  Context.Agreement.FirstOrDefaultAsync();
            var returnData = new GetAgreementResponse();

            if (data != null)
            {
                returnData.AgreementText = data.AgreementText;
                returnData.Id = data.Id;

            }

            return returnData;



        }


    }
}
