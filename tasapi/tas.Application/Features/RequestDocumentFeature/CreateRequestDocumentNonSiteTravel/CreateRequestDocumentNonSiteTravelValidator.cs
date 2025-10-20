using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel
{


    public class CreateRequestDocumentNonSiteTravelRequestValidator : AbstractValidator<CreateRequestDocumentNonSiteTravelRequest>
    {
        public CreateRequestDocumentNonSiteTravelRequestValidator()
        {
            RuleFor(x => x.travelData).NotNull().SetValidator(new CreateRequestDocumentNonSiteTravelValidator());
            RuleForEach(x => x.AccommodationData).SetValidator(new CreateRequestDocumentNonSiteTravelAccommodationValidator());
            //RuleFor(x => x.RequestInfo).NotNull().SetValidator(new CreateRequestDocumentNonSiteTravelRequestInfoValidator());
        }

    }

    public class CreateRequestDocumentNonSiteTravelValidator : AbstractValidator<CreateRequestDocumentNonSiteTravel>
    {
        public CreateRequestDocumentNonSiteTravelValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.Action).NotEmpty();


        }
    }

    public class CreateRequestDocumentNonSiteTravelAccommodationValidator : AbstractValidator<CreateRequestDocumentNonSiteTravelAccommodation>
    {
        public CreateRequestDocumentNonSiteTravelAccommodationValidator()
        {
            RuleFor(x => x.FirstNight)
                .LessThan(x => x.LastNight)
                .WithMessage("Accommodation First Night must be before Last Night.");
        }
    }





}
