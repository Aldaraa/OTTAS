using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.NationalityFeature.UpdateNationality
{
    public class UpdateNationalityHandler : IRequestHandler<UpdateNationalityRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INationalityRepository _NationalityRepository;
        private readonly IMapper _mapper;

        public UpdateNationalityHandler(IUnitOfWork unitOfWork, INationalityRepository NationalityRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _NationalityRepository = NationalityRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateNationalityRequest request, CancellationToken cancellationToken)
        {
            var Nationality = _mapper.Map<Nationality>(request);
            _NationalityRepository.Update(Nationality);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
