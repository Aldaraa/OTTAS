using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PeopleTypeFeature.UpdatePeopleType
{
    public class UpdatePeopleTypeHandler : IRequestHandler<UpdatePeopleTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPeopleTypeRepository _PeopleTypeRepository;
        private readonly IMapper _mapper;

        public UpdatePeopleTypeHandler(IUnitOfWork unitOfWork, IPeopleTypeRepository PeopleTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PeopleTypeRepository = PeopleTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdatePeopleTypeRequest request, CancellationToken cancellationToken)
        {
            var PeopleType = _mapper.Map<PeopleType>(request);
            await _PeopleTypeRepository.CheckDuplicateData(PeopleType, c => c.Code, c => c.Description);
            _PeopleTypeRepository.Update(PeopleType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
