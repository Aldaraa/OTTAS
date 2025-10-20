using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.NotificationFeature.NotificationGetDetail
{

    public sealed class NotifcationGetDetailHandler : IRequestHandler<NotificationGetDetailRequest, NotificationGetDetailResponse>
    {
        private readonly INotificationRepository _NotifcationRepository;
        private readonly IMapper _mapper;

        public NotifcationGetDetailHandler(INotificationRepository NotifcationRepository, IMapper mapper)
        {
            _NotifcationRepository = NotifcationRepository;
            _mapper = mapper;
        }

        public async Task<NotificationGetDetailResponse> Handle(NotificationGetDetailRequest request, CancellationToken cancellationToken)
        {
            var data = await _NotifcationRepository.NotificationGetDetail(request, cancellationToken);
            return data;

        }
    }
}
