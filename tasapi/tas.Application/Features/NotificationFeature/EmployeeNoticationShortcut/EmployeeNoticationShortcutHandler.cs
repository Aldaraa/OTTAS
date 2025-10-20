using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.NotificationFeature.EmployeeNoticationShortcut
{

    public sealed class NotifcationGetDetailHandler : IRequestHandler<EmployeeNoticationShortcutRequest, EmployeeNoticationShortcutResponse>
    {
        private readonly INotificationRepository _NotifcationRepository;
        private readonly IMapper _mapper;

        public NotifcationGetDetailHandler(INotificationRepository NotifcationRepository, IMapper mapper)
        {
            _NotifcationRepository = NotifcationRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeNoticationShortcutResponse> Handle(EmployeeNoticationShortcutRequest request, CancellationToken cancellationToken)
        {
            var data = await _NotifcationRepository.EmployeeNoticationShortcut(request, cancellationToken);
            return data;

        }
    }
}
