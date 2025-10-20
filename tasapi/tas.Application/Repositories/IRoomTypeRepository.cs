using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Features.RoomTypeFeature.GetAllRoomType;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRoomTypeRepository : IBaseRepository<RoomType>
    {
        Task<List<GetAllRoomTypeResponse>> GetAllData(GetAllRoomTypeRequest request, CancellationToken cancellationToken);


    }
}
