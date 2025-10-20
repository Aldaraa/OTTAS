using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SysResponseTimeFeature.DeleteSysResponseTime
{
    public sealed class DeleteSysResponseTimeHandler : IRequestHandler<DeleteSysResponseTimeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysResponseTimeRepository _SysResponseTimeRepository;
        private readonly IMapper _mapper;

        public DeleteSysResponseTimeHandler(IUnitOfWork unitOfWork, ISysResponseTimeRepository SysResponseTimeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysResponseTimeRepository = SysResponseTimeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(DeleteSysResponseTimeRequest request, CancellationToken cancellationToken)
        {
            await  _SysResponseTimeRepository.DeleteOldData(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
