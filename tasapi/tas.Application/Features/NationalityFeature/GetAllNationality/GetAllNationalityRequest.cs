using MediatR;

namespace tas.Application.Features.NationalityFeature.GetAllNationality
{
    public sealed record GetAllNationalityRequest(int? status) : IRequest<List<GetAllNationalityResponse>>;
}
