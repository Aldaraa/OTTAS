using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.StateFeature.UpdateState
{
    public class UpdateStateHandler : IRequestHandler<UpdateStateRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStateRepository _StateRepository;
        private readonly IMapper _mapper;

        public UpdateStateHandler(IUnitOfWork unitOfWork, IStateRepository StateRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _StateRepository = StateRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateStateRequest request, CancellationToken cancellationToken)
        {
            var State = _mapper.Map<State>(request);
            _StateRepository.Update(State);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
