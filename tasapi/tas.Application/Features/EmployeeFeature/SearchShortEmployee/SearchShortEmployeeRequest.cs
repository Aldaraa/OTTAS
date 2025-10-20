using MediatR;
using System.Reflection;
using System.Runtime.CompilerServices;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.SearchShortEmployee
{
    public sealed record SearchShortEmployeeRequest(RequestSearchShortModel model) : BasePagenationRequest, IRequest<SearchShortEmployeeResponse>;
}

public record RequestSearchShortModel(
        string keyWord
    ); 
