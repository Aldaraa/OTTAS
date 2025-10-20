using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ColorFeature.DeleteColor
{

    public sealed class DeleteColorHandler : IRequestHandler<DeleteColorRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IColorRepository _ColorRepository;
        private readonly IMapper _mapper;

        public DeleteColorHandler(IUnitOfWork unitOfWork, IColorRepository ColorRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ColorRepository = ColorRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteColorRequest request, CancellationToken cancellationToken)
        {
            var Color = _mapper.Map<Color>(request);
            _ColorRepository.Delete(Color);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
