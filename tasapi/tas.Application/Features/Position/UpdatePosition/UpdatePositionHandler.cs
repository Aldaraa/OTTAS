using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.UpdatePosition
{
    public class UpdatePositionHandler : IRequestHandler<UpdatePositionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPositionRepository _PositionRepository;
        private readonly IMapper _mapper;

        public UpdatePositionHandler(IUnitOfWork unitOfWork, IPositionRepository PositionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PositionRepository = PositionRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdatePositionRequest request, CancellationToken cancellationToken)
        {
            var Position = _mapper.Map<Position>(request);
            await _PositionRepository.CheckDuplicateData(Position, c => c.Code, c => c.Description);
            _PositionRepository.Update(Position);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
