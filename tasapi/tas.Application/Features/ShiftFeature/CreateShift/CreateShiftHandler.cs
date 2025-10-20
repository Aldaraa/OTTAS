using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ShiftFeature.CreateShift
{
    public sealed class CreateShiftHandler : IRequestHandler<CreateShiftRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShiftRepository _ShiftRepository;
        private readonly IMapper _mapper;

        public CreateShiftHandler(IUnitOfWork unitOfWork, IShiftRepository ShiftRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ShiftRepository = ShiftRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateShiftRequest request, CancellationToken cancellationToken)
        {
            var Shift = _mapper.Map<Shift>(request);
            await _ShiftRepository.CheckDuplicateData(Shift, c => c.Code, c => c.Description);
            _ShiftRepository.Create(Shift);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
