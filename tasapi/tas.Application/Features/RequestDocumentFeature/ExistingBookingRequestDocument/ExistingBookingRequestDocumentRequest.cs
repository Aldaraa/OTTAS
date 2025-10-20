using MediatR;
using tas.Domain.Common;

namespace tas.Application.Features.RequestDocumentFeature.ExistingBookingRequestDocument
{
    public sealed record ExistingBookingRequestDocumentRequest(int EmployeeId) :  IRequest<ExistingBookingRequestDocumentResponse>;

}
