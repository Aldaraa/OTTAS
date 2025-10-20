using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLocalHotelFeature.UpdateRequestLocalHotel
{
    public class UpdateClusterHandler : IRequestHandler<UpdateRequestLocalHotelRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestLocalHotelRepository _RequestLocalHotelRepository;
        private readonly IMapper _mapper;

        public UpdateClusterHandler(IUnitOfWork unitOfWork, IRequestLocalHotelRepository RequestLocalHotelRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestLocalHotelRepository = RequestLocalHotelRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestLocalHotelRequest request, CancellationToken cancellationToken)
        {
            var RequestLocalHotel = _mapper.Map<RequestLocalHotel>(request);
            await _RequestLocalHotelRepository.CheckDuplicateData(RequestLocalHotel, c => c.Description);
            _RequestLocalHotelRepository.Update(RequestLocalHotel);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
