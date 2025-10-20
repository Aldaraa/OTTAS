using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.BusstopFeature.DeleteBusstop
{

    public sealed class DeleteBusstopHandler : IRequestHandler<DeleteBusstopRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBusstopRepository _BusstopRepository;
        private readonly IMapper _mapper;

        public DeleteBusstopHandler(IUnitOfWork unitOfWork, IBusstopRepository BusstopRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _BusstopRepository = BusstopRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteBusstopRequest request, CancellationToken cancellationToken)
        {
            await   _BusstopRepository.DeletForce(request,cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
