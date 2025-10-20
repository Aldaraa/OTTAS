using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ShiftFeature.UpdateShift
{
    public class UpdateShiftHandler : IRequestHandler<UpdateShiftRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShiftRepository _ShiftRepository;
        private readonly IMapper _mapper;

        public UpdateShiftHandler(IUnitOfWork unitOfWork, IShiftRepository ShiftRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ShiftRepository = ShiftRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateShiftRequest request, CancellationToken cancellationToken)
        {
            var Shift = _mapper.Map<Shift>(request);
            await _ShiftRepository.CheckDuplicateData(Shift, c => c.Code, c => c.Description);
            _ShiftRepository.Update(Shift);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
