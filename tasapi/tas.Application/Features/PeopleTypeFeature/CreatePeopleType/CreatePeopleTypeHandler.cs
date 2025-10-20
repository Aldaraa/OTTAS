using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PeopleTypeFeature.CreatePeopleType
{
    public sealed class CreatePeopleTypeHandler : IRequestHandler<CreatePeopleTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPeopleTypeRepository _PeopleTypeRepository;
        private readonly IMapper _mapper;

        public CreatePeopleTypeHandler(IUnitOfWork unitOfWork, IPeopleTypeRepository PeopleTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PeopleTypeRepository = PeopleTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreatePeopleTypeRequest request, CancellationToken cancellationToken)
        {
            var PeopleType = _mapper.Map<PeopleType>(request);
            await _PeopleTypeRepository.CheckDuplicateData(PeopleType, c=> c.Code, c => c.Description);
            _PeopleTypeRepository.Create(PeopleType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
