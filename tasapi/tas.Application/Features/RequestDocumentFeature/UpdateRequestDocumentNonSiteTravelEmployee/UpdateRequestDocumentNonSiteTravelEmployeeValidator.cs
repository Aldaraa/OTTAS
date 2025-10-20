using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelEmployee;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel
{


    public class UpdateRequestDocumentNonSiteTravelEmployeeValidator : AbstractValidator<UpdateRequestDocumentNonSiteTravelEmployeeRequest>
    {
        public UpdateRequestDocumentNonSiteTravelEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.EmergencyContactMobile).NotEmpty();
            RuleFor(x => x.EmergencyContactName).NotEmpty();

            RuleFor(x => x.PassportExpiry).NotEmpty();



            //RuleFor(x => x.RequestInfo).NotNull().SetValidator(new UpdateRequestDocumentNonSiteTravelEmployeeInfoValidator());
        }
    }




}
