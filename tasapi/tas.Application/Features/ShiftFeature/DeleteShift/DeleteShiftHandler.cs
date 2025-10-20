using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ShiftFeature.DeleteShift
{

    public sealed class DeleteShiftHandler : IRequestHandler<DeleteShiftRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShiftRepository _ShiftRepository;
        private readonly IMapper _mapper;

        public DeleteShiftHandler(IUnitOfWork unitOfWork, IShiftRepository ShiftRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ShiftRepository = ShiftRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteShiftRequest request, CancellationToken cancellationToken)
        {
            var Shift = _mapper.Map<Shift>(request);
            _ShiftRepository.Delete(Shift);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
