using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomTypeFeature.UpdateRoomType
{
    public class UpdateRoomTypeHandler : IRequestHandler<UpdateRoomTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomTypeRepository _RoomTypeRepository;
        private readonly IMapper _mapper;

        public UpdateRoomTypeHandler(IUnitOfWork unitOfWork, IRoomTypeRepository RoomTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomTypeRepository = RoomTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRoomTypeRequest request, CancellationToken cancellationToken)
        {
            var RoomType = _mapper.Map<RoomType>(request);
            await _RoomTypeRepository.CheckDuplicateData(RoomType, c => c.Description);
            _RoomTypeRepository.Update(RoomType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
