using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PeopleTypeFeature.DeletePeopleType
{

    public sealed class DeletePeopleTypeHandler : IRequestHandler<DeletePeopleTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPeopleTypeRepository _PeopleTypeRepository;
        private readonly IMapper _mapper;

        public DeletePeopleTypeHandler(IUnitOfWork unitOfWork, IPeopleTypeRepository PeopleTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PeopleTypeRepository = PeopleTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeletePeopleTypeRequest request, CancellationToken cancellationToken)
        {
            var PeopleType = _mapper.Map<PeopleType>(request);
            _PeopleTypeRepository.Delete(PeopleType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
