using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CampFeature.CreateCamp
{
    public sealed class CreateCampHandler : IRequestHandler<CreateCampRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICampRepository _CampRepository;
        private readonly IMapper _mapper;

        public CreateCampHandler(IUnitOfWork unitOfWork, ICampRepository CampRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CampRepository = CampRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateCampRequest request, CancellationToken cancellationToken)
        {
            var Camp = _mapper.Map<Camp>(request);
            _CampRepository.Create(Camp);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
