using AutoMapper;
using MediatR;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectute;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExecute
{
    public sealed class BulkRosterExecuteHandler : IRequestHandler<BulkRosterExecuteRequest, BulkRosterExecuteResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterExecuteRepository _RosterExecuteRepository;
        private readonly IMapper _mapper;

        public BulkRosterExecuteHandler(IUnitOfWork unitOfWork, IRosterExecuteRepository RosterExecuteRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterExecuteRepository = RosterExecuteRepository;
            _mapper = mapper;
        }

        public async Task<BulkRosterExecuteResponse>  Handle(BulkRosterExecuteRequest request, CancellationToken cancellationToken)
        {
           var data =  await _RosterExecuteRepository.ExecuteBulkRoster(request, cancellationToken);
            return data;
        }
    }
}
