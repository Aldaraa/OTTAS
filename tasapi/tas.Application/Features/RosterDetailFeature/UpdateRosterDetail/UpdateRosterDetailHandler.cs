using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterDetailFeature.UpdateRosterDetail
{
    public class UpdateRosterDetailHandler : IRequestHandler<UpdateRosterDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterDetailRepository _RosterDetailRepository;
        private readonly IMapper _mapper;

        public UpdateRosterDetailHandler(IUnitOfWork unitOfWork, IRosterDetailRepository RosterDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterDetailRepository = RosterDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRosterDetailRequest request, CancellationToken cancellationToken)
        {

            await _RosterDetailRepository.UpdateData(request, cancellationToken);
            // await _RosterDetailRepository.UpdateReorderSequeensNumber(request.Rosterid, request.Id, request.SeqNumber, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
