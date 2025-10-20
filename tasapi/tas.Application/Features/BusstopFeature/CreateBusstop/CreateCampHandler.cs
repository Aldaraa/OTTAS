using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.BusstopFeature.CreateBusstop
{
    public sealed class CreateBusstopHandler : IRequestHandler<CreateBusstopRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBusstopRepository _BusstopRepository;
        private readonly IMapper _mapper;

        public CreateBusstopHandler(IUnitOfWork unitOfWork, IBusstopRepository BusstopRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _BusstopRepository = BusstopRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateBusstopRequest request, CancellationToken cancellationToken)
        {
            var Busstop = _mapper.Map<Busstop>(request);
            _BusstopRepository.Create(Busstop);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
