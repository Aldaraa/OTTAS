using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.SearchEmployee;

namespace tas.Application.Service
{
    public class CacheKeyGenerator
    {
        public static string GenerateCacheKeyEmployeeSearch(SearchEmployeeRequest request, string? RoleName, int? Id)
        {
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append($"EMPLOYEE_SEARCH_Role_{RoleName}_Id_{Id}");

            keyBuilder.Append($"_{request.pageIndex}_");
            keyBuilder.Append($"_{request.pageSize}_");

            keyBuilder.Append(request.model.Id ?? ""); // Using ?? to handle nullable values
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.Lastname ?? "");
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.Firstname ?? "");
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.RoomNumber ?? "");
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.Departmentid ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.LocationId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.CostCodeId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.NRN ?? "");
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.RosterId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.EmployerId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.PeopleTypeId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.FlightGroupMasterId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.SAPID ?? "");
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.CampId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.RoomTypeId ?? 0);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.Mobile ?? "");
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.HasRoom ?? -1);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.FutureBooking ?? -1);
            keyBuilder.Append("_");
            keyBuilder.Append(request.model.Active ?? -1);

            return keyBuilder.ToString();
        }

    }
}
