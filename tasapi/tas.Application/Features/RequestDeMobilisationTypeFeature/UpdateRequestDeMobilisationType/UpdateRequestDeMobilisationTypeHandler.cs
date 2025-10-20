using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.UpdateRequestDeMobilisationType
{
    public class UpdateRequestDeMobilisationTypeHandler : IRequestHandler<UpdateRequestDeMobilisationTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDeMobilisationTypeRepository _RequestDeMobilisationTypeRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDeMobilisationTypeHandler(IUnitOfWork unitOfWork, IRequestDeMobilisationTypeRepository RequestDeMobilisationTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDeMobilisationTypeRepository = RequestDeMobilisationTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestDeMobilisationTypeRequest request, CancellationToken cancellationToken)
        {
            var RequestDeMobilisationType = _mapper.Map<RequestDeMobilisationType>(request);
            _RequestDeMobilisationTypeRepository.Update(RequestDeMobilisationType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
