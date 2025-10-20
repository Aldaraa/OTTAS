using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.NationalityFeature.CreateNationality
{
    public sealed class CreateNationalityHandler : IRequestHandler<CreateNationalityRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INationalityRepository _NationalityRepository;
        private readonly IMapper _mapper;

        public CreateNationalityHandler(IUnitOfWork unitOfWork, INationalityRepository NationalityRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _NationalityRepository = NationalityRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateNationalityRequest request, CancellationToken cancellationToken)
        {
            var Nationality = _mapper.Map<Nationality>(request);
            _NationalityRepository.Create(Nationality);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
