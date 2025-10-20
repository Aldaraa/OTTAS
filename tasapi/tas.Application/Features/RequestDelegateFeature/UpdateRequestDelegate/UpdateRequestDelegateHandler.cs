using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestDelegateFeature.UpdateRequestDelegate
{
    public class UpdateRequestDelegateHandler : IRequestHandler<UpdateRequestDelegateRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDelegateRepository _RequestDelegateRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDelegateHandler(IUnitOfWork unitOfWork, IRequestDelegateRepository RequestDelegateRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDelegateRepository = RequestDelegateRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestDelegateRequest request, CancellationToken cancellationToken)
        {
            await  _RequestDelegateRepository.UpdateData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
