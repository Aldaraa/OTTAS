using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ColorFeature.CreateColor
{
    public sealed class CreateColorHandler : IRequestHandler<CreateColorRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IColorRepository _ColorRepository;
        private readonly IMapper _mapper;

        public CreateColorHandler(IUnitOfWork unitOfWork, IColorRepository ColorRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ColorRepository = ColorRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateColorRequest request, CancellationToken cancellationToken)
        {
            var Color = _mapper.Map<Color>(request);
            await _ColorRepository.CheckDuplicateData(Color, c => c.Code, c => c.Description);
            _ColorRepository.Create(Color);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
