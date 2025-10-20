using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.NotificationFeature.NotificationGetAll
{

    public sealed class NotifcationGetAllHandler : IRequestHandler<NotificationGetAllRequest, NotificationGetAllResponse>
    {
        private readonly INotificationRepository _NotifcationRepository;
        private readonly IMapper _mapper;

        public NotifcationGetAllHandler(INotificationRepository NotifcationRepository, IMapper mapper)
        {
            _NotifcationRepository = NotifcationRepository;
            _mapper = mapper;
        }

        public async Task<NotificationGetAllResponse> Handle(NotificationGetAllRequest request, CancellationToken cancellationToken)
        {
            var data = await _NotifcationRepository.NotificationGetAll(request, cancellationToken);
            return data;

        }
    }
}
