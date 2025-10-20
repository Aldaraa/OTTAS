using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.BusstopFeature.UpdateBusstop
{
    public class UpdateBusstopHandler : IRequestHandler<UpdateBusstopRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBusstopRepository _BusstopRepository;
        private readonly IMapper _mapper;

        public UpdateBusstopHandler(IUnitOfWork unitOfWork, IBusstopRepository BusstopRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _BusstopRepository = BusstopRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateBusstopRequest request, CancellationToken cancellationToken)
        {
            var Busstop = _mapper.Map<Busstop>(request);
            _BusstopRepository.Update(Busstop);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
