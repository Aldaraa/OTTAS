using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SysVersionFeature.CreateSysVersion
{
    public sealed class CreateSysVersionHandler : IRequestHandler<CreateSysVersionRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysVersionRepository _SysVersionRepository;
        private readonly IMapper _mapper;

        public CreateSysVersionHandler(IUnitOfWork unitOfWork, ISysVersionRepository SysVersionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysVersionRepository = SysVersionRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateSysVersionRequest request, CancellationToken cancellationToken)
        {
            _SysVersionRepository.CreateVersion(request,cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
