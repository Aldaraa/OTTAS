using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CampFeature.DeleteCamp
{

    public sealed class DeleteCampHandler : IRequestHandler<DeleteCampRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICampRepository _CampRepository;
        private readonly IMapper _mapper;

        public DeleteCampHandler(IUnitOfWork unitOfWork, ICampRepository CampRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CampRepository = CampRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteCampRequest request, CancellationToken cancellationToken)
        {
            var Camp = _mapper.Map<Camp>(request);
            _CampRepository.Delete(Camp);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
