using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ProfileFieldFeature.UpdateProfileField
{
    public class UpdateClusterHandler : IRequestHandler<UpdateProfileFieldRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProfileFieldRepository _ProfileFieldRepository;
        private readonly IMapper _mapper;

        public UpdateClusterHandler(IUnitOfWork unitOfWork, IProfileFieldRepository ProfileFieldRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ProfileFieldRepository = ProfileFieldRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateProfileFieldRequest request, CancellationToken cancellationToken)
        {
            await _ProfileFieldRepository.UpdateProfileField(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
