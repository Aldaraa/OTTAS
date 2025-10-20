using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterFeature.UpdateRoster
{
    public class UpdateRosterHandler : IRequestHandler<UpdateRosterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterRepository _RosterRepository;
        private readonly IMapper _mapper;

        public UpdateRosterHandler(IUnitOfWork unitOfWork, IRosterRepository RosterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterRepository = RosterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRosterRequest request, CancellationToken cancellationToken)
        {
            var Roster = _mapper.Map<Roster>(request);
            await _RosterRepository.CheckDuplicateData(Roster, c => c.Name, c => c.Description);
            _RosterRepository.Update(Roster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
