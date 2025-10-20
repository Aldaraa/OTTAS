using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.CampFeature.UpdateCamp
{
    public class UpdateCampHandler : IRequestHandler<UpdateCampRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICampRepository _CampRepository;
        private readonly IMapper _mapper;

        public UpdateCampHandler(IUnitOfWork unitOfWork, ICampRepository CampRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _CampRepository = CampRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCampRequest request, CancellationToken cancellationToken)
        {
            var Camp = _mapper.Map<Camp>(request);
            _CampRepository.Update(Camp);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
