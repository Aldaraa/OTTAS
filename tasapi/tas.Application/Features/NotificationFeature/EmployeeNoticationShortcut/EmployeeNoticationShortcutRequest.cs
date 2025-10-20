using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NotificationFeature.EmployeeNoticationShortcut;

namespace tas.Application.Features.NotificationFeature.EmployeeNoticationShortcut
{
    public sealed record EmployeeNoticationShortcutRequest(int EmployeeId) : IRequest<EmployeeNoticationShortcutResponse>;
}
