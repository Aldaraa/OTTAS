using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.UpdateBed
{
    public class UpdateBedHandler : IRequestHandler<UpdateBedRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBedRepository _BedRepository;
        private readonly IMapper _mapper;

        public UpdateBedHandler(IUnitOfWork unitOfWork, IBedRepository BedRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _BedRepository = BedRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateBedRequest request, CancellationToken cancellationToken)
        {
            var Bed = _mapper.Map<Bed>(request);
            _BedRepository.Update(Bed);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
