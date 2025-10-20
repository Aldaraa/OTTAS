using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SysResponseTimeFeature.CreateSysResponseTime
{
    public sealed class CreateSysResponseTimeHandler : IRequestHandler<CreateSysResponseTimeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysResponseTimeRepository _SysResponseTimeRepository;
        private readonly IMapper _mapper;

        public CreateSysResponseTimeHandler(IUnitOfWork unitOfWork, ISysResponseTimeRepository SysResponseTimeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysResponseTimeRepository = SysResponseTimeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateSysResponseTimeRequest request, CancellationToken cancellationToken)
        {
            var SysResponseTime = _mapper.Map<SysResponseTime>(request);
            _SysResponseTimeRepository.Create(SysResponseTime);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
