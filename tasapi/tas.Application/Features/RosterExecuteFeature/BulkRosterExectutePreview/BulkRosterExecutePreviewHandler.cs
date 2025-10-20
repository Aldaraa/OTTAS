using AutoMapper;
using MediatR;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectutePreview;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterExecuteFeature.BulkRosterExecutePreview
{
    public sealed class BulkRosterExecutePreviewHandler : IRequestHandler<BulkRosterExecutePreviewRequest, List<BulkRosterExecutePreviewResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRosterExecuteRepository _RosterExecuteRepository;
        private readonly IMapper _mapper;

        public BulkRosterExecutePreviewHandler(IUnitOfWork unitOfWork, IRosterExecuteRepository RosterExecuteRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RosterExecuteRepository = RosterExecuteRepository;
            _mapper = mapper;
        }

        public async Task<List<BulkRosterExecutePreviewResponse>>  Handle(BulkRosterExecutePreviewRequest request, CancellationToken cancellationToken)
        {
           return   await _RosterExecuteRepository.ExecuteBulkRosterPreview(request, cancellationToken);
        }
    }
}
