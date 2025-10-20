using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ColorFeature.UpdateColor
{
    public class UpdateClusterHandler : IRequestHandler<UpdateColorRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IColorRepository _ColorRepository;
        private readonly IMapper _mapper;

        public UpdateClusterHandler(IUnitOfWork unitOfWork, IColorRepository ColorRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ColorRepository = ColorRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateColorRequest request, CancellationToken cancellationToken)
        {
            var Color = _mapper.Map<Color>(request);
            await _ColorRepository.CheckDuplicateData(Color, c => c.Code, c => c.Description);
            _ColorRepository.Update(Color);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
