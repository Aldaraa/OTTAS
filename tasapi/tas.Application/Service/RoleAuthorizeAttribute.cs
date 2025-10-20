using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using tas.Domain.Entities;

namespace tas.Application.Service
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(string roles)
        {   
            if (roles != null) {
                _roles = roles.Split(',');
            }

        }
        public RoleAuthorizeAttribute()
        {
            _roles=new string[0];
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var path = context.HttpContext.Request.Path.Value.ToLower();
            var method = context.HttpContext.Request.Method;
            if (context.HttpContext.Request.Path.Value.ToLower().Contains("api") ||  context.HttpContext.Request.Path.Value.ToLower().Contains("screenhub"))
            {
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                var userPrincipal = context.HttpContext.User;
                var hasRole = _roles != null && userPrincipal.Claims.Any(x => x.Type == "TASSystemRole" && _roles.Contains(x.Value));
                var ReadOnlyAccess = userPrincipal.Claims.FirstOrDefault(x => x.Type == "TASSystemReadOnlyAccess");
                var isGuest = userPrincipal.Claims.Any(x => x.Type == "TASSystemRole" && x.Value == "Guest");

                if (isGuest)
                {
                    if (!IsGuestRouteAllowed(method, path, context.HttpContext))
                    {
                        context.Result = new ForbidResult();
                        return;
                    }


                }
                else
                {
                    // Check if user has one of the required roles
                    if (_roles.Length > 0 && !hasRole)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }

        }

        private bool IsGuestRouteAllowed(string method, string path, HttpContext context)
        {
            var guestAllowedRoutes = GuestAllowedRoutes();
            var normalizedMethod = method.ToLower();
            var normalizedPath = path.ToLower();

            return guestAllowedRoutes.Any(route =>
            {
                var routeMethod = route.method.ToLower();
                var routePath = route.path.ToLower();

                // Split the route path and normalized path by '/'
                var routeSegments = routePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var pathSegments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (routeSegments.Length > pathSegments.Length)
                {
                    return false;
                }

                for (int i = 0; i < routeSegments.Length; i++)
                {
                    if (routeSegments[i] == "*")
                    {
                        continue;
                    }

                    if (routeSegments[i] != pathSegments[i])
                    {
                        return false;
                    }
                }

                return normalizedMethod == routeMethod;
            });
        }

      





        private List<TasRoute> GuestAllowedRoutes()
        {
            return new List<TasRoute>
                {

                new TasRoute { path = "/api/tas/activetransport/schedule/*", method = "GET" },
                new TasRoute { path = "/api/tas/activetransport/getdatetransport", method = "GET" },
                new TasRoute { path = "/api/tas/activetransport/getcalendardata", method = "GET" },
                new TasRoute { path = "/api/tas/agreement", method = "GET" },
                new TasRoute { path = "/api/tas/agreement/agreementcheck", method = "POST" },
                new TasRoute { path = "/api/tas/camp", method = "GET" },
                new TasRoute { path = "/api/tas/carrier", method = "GET" },
                new TasRoute { path = "/api/tas/cluster/activetransports/*", method = "GET" },
                new TasRoute { path = "/api/tas/color", method = "GET" },
                new TasRoute { path = "/api/tas/costcode", method = "GET" },
                new TasRoute { path = "/api/tas/department", method = "GET" },
                new TasRoute { path = "/api/tas/department/parent/*", method = "GET" },
                new TasRoute { path = "/api/tas/department/*", method = "GET" },
                new TasRoute { path = "/api/tas/department/manager", method = "POST" },
                new TasRoute { path = "/api/tas/department/supervisor", method = "POST" },
                new TasRoute { path = "/api/tas/department/admin", method = "POST" },
                new TasRoute { path = "/api/tas/department/minimum", method = "GET" },
                new TasRoute { path = "/api/tas/department/admins", method = "GET" },
                new TasRoute { path = "/api/tas/department/managers", method = "GET" },
                new TasRoute { path = "/api/tas/department/managerdepartments/*", method = "GET" },
                new TasRoute { path = "/api/tas/department/admindepartments/*", method = "GET" },
                new TasRoute { path = "/api/tas/employee/search", method = "GET" },
                new TasRoute { path = "/api/tas/employee/search", method = "POST" },
                new TasRoute { path = "/api/tas/employee/searchshort", method = "POST" },
                new TasRoute { path = "/api/tas/employee/activedirectly/*", method = "PUT" },
                new TasRoute { path = "/api/tas/employee/profiletransport/*", method = "GET" },
                new TasRoute { path = "/api/tas/employee/*", method = "GET" },
                new TasRoute { path = "/api/tas/employee/accounthistory/*", method = "GET" },
                new TasRoute { path = "/api/tas/employee", method = "GET" },
                new TasRoute { path = "/api/tas/employee/request", method = "POST" },
                new TasRoute { path = "/api/tas/employee/statusdates", method = "POST" },
                new TasRoute { path = "/api/tas/employee/checkadaccount", method = "POST" },
                new TasRoute { path = "/api/tas/employee/myinfo", method = "GET" },
                new TasRoute { path = "/api/tas/employee/deactive/check/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/employee/deactive", method = "POST" },
                new TasRoute { path = "/api/tas/employee/reactive", method = "POST" },
                new TasRoute { path = "/api/tas/employeestatus/employeebooking", method = "GET" },
                new TasRoute { path = "/api/tas/employeestatus/visualstatusdates/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/employeestatus/roombookingviewcalendar/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/employeestatus/profilebydate/*/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/employeestatus/annualyear/*/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/employeestatus/lasterstatus/onsite/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/employeestatus/lasterstatus/offsite/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/employer", method = "GET" },
                new TasRoute { path = "/api/tas/flightgroupmaster", method = "GET" },
                new TasRoute { path = "/api/tas/flightgroupmaster/*", method = "GET" },
                new TasRoute { path = "/api/tas/groupdetail/*", method = "GET" },
                new TasRoute { path = "/api/tas/groupmaster", method = "GET" },
                new TasRoute { path = "/api/tas/groupmaster/profiledata", method = "GET" },
                new TasRoute { path = "/api/tas/groupmembers/*", method = "POST" },
                new TasRoute { path = "/api/tas/location", method = "GET" },
                new TasRoute { path = "/api/tas/mailsmtpconfig", method = "GET" },
                new TasRoute { path = "/api/tas/nationality", method = "GET" },
                new TasRoute { path = "/api/tas/peopletype", method = "GET" },
                new TasRoute { path = "/api/tas/position", method = "GET" },
                new TasRoute { path = "/api/tas/profilefield", method = "GET" },
                new TasRoute { path = "/api/tas/profilefield", method = "PUT" },
                new TasRoute { path = "/api/tas/requestairport", method = "GET" },
                new TasRoute { path = "/api/tas/requestairport/search/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdelegate", method = "GET" },
                new TasRoute { path = "/api/tas/requestdemobilisation/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdemobilisation", method = "POST" },
                new TasRoute { path = "/api/tas/requestdemobilisation", method = "PUT" },
                new TasRoute { path = "/api/tas/requestdemobilisation/complete/*", method = "PUT" },
                new TasRoute { path = "/api/tas/requestdocumentattachment", method = "POST" },
                new TasRoute { path = "/api/tas/requestdemobilisationtype", method = "GET" },
                new TasRoute { path = "/api/tas/requestdemobilisationtype", method = "POST" },
                new TasRoute { path = "/api/tas/requestdocumentattachment/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocumentattachment/download/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/master", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/dashboard", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/documentlist", method = "POST" },
                new TasRoute { path = "/api/tas/requestdocument/documentlist/cancelled", method = "POST" },
                new TasRoute { path = "/api/tas/requestdocument/documentlistinpersonate/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/cancel", method = "PUT" },
                new TasRoute { path = "/api/tas/requestdocument/approve", method = "PUT" },



                new TasRoute { path = "/api/tas/requestdocument/checkduplicate/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/existingbooking/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/nonsitetravel/groups", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/myinfo", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocument/testnotification/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocumenthistory/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange/temp/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange", method = "PUT" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange/temp", method = "PUT" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange", method = "POST" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange/temp", method = "POST" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange/complete/*", method = "PUT" },
                new TasRoute { path = "/api/tas/requestdocumentprofilechange/complete/temp/*", method = "PUT" },
                new TasRoute { path = "/api/tas/requestgroupconfig/documenttypes", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupconfig/documentapproval/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupconfig/documentapprovalorder", method = "PUT" },
                new TasRoute { path = "/api/tas/requestgroupconfig/groupandmembers/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupconfig/groupandmembers/bytype/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupconfig/documentroute/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroup", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupemployee", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupemployee/activeemployees", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupemployee/inpersonateusers", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupemployee/groupemployees/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupemployee/linemanageremployees", method = "GET" },
                new TasRoute { path = "/api/tas/requestgroupemployee/linemanageradminemployees", method = "GET" },
                new TasRoute { path = "/api/tas/requestlinemanageremployee", method = "GET" },
                new TasRoute { path = "/api/tas/requestlocalhotel", method = "GET" },
                new TasRoute { path = "/api/tas/requestnonsiteticketconfig", method = "GET" },
                new TasRoute { path = "/api/tas/requestnonsiteticketconfig/extractoption", method = "POST" },
                new TasRoute { path = "/api/tas/requestnonsitetravel", method = "POST" },
                new TasRoute { path = "/api/tas/requestnonsitetravel/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestnonsitetravel/traveldata", method = "PUT" },
                new TasRoute { path = "/api/tas/requestnonsitetravel/employee", method = "PUT" },
                new TasRoute { path = "/api/tas/requestnonsitetravelflight", method = "POST" },
                new TasRoute { path = "/api/tas/requestnonsitetravelflight", method = "PUT" },
                new TasRoute { path = "/api/tas/requestnonsitetraveloption", method = "POST" },
                new TasRoute { path = "/api/tas/requestnonsitetraveloption", method = "PUT" },
                new TasRoute { path = "/api/tas/requestnonsitetraveloption/fulldata", method = "PUT" },
                new TasRoute { path = "/api/tas/requestnonsitetraveloption/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestnonsitetraveloption/final/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestsitetravel/addtravel", method = "POST" },
                new TasRoute { path = "/api/tas/requestsitetravel/addtravel", method = "PUT" },
                new TasRoute { path = "/api/tas/requestsitetravel/addtravel/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestsitetravel/addtravel/complete/*", method = "PUT" },
                new TasRoute { path = "/api/tas/requestsitetravel/addtravel/checkduplicate", method = "POST" },
                new TasRoute { path = "/api/tas/requestsitetravel/reschedule", method = "POST" },
                new TasRoute { path = "/api/tas/requestsitetravel/reschedule", method = "PUT" },
                new TasRoute { path = "/api/tas/requestsitetravel/reschedule/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestsitetravel/reschedule/complete/*", method = "PUT" },
                new TasRoute { path = "/api/tas/requestsitetravel/reschedule/checkduplicate", method = "POST" },
                new TasRoute { path = "/api/tas/requestsitetravel/remove/*", method = "GET" },
                new TasRoute { path = "/api/tas/requestsitetravel/removetravel", method = "POST" },
                new TasRoute { path = "/api/tas/requestsitetravel/removetravel", method = "PUT" },
                new TasRoute { path = "/api/tas/requestsitetravel/removetravel/checkduplicate", method = "POST" },
                new TasRoute { path = "/api/tas/requestsitetravel/removetravel/complete/*", method = "PUT" },

                new TasRoute { path = "/api/tas/requestsitetravel/remove/checkduplicate", method = "POST" },
                new TasRoute { path = "/api/tas/requesttravelagent", method = "GET" },
                new TasRoute { path = "/api/tas/requesttravelpurpose", method = "GET" },
                new TasRoute { path = "/api/tas/room", method = "GET" },
                new TasRoute { path = "/api/tas/room/*", method = "GET" },
                new TasRoute { path = "/api/tas/room/search", method = "POST" },
                new TasRoute { path = "/api/tas/room/employeeprofile/*", method = "GET" },
                new TasRoute { path = "/api/tas/room/statusbetweendates", method = "POST" },
                new TasRoute { path = "/api/tas/room/getvirtualroomid", method = "GET" },
                new TasRoute { path = "/api/tas/roomtype", method = "GET" },
                new TasRoute { path = "/api/tas/roster", method = "GET" },
                new TasRoute { path = "/api/tas/roster/*", method = "GET" },
                new TasRoute { path = "/api/tas/rosterdetail/*", method = "GET" },
                new TasRoute { path = "/api/tas/rostergroup", method = "GET" },
                new TasRoute { path = "/api/tas/shift", method = "GET" },
                new TasRoute { path = "/api/tas/state", method = "GET" },
                new TasRoute { path = "/api/tas/statuschangesemployeerequest/deactive", method = "GET" },
                new TasRoute { path = "/api/tas/statuschangesemployeerequest/reactive", method = "GET" },
                new TasRoute { path = "/api/tas/sysfile", method = "POST" },
                new TasRoute { path = "/api/tas/sysfile/multi", method = "POST" },
                new TasRoute { path = "/api/tas/sysrole/accesspermission", method = "PUT" },
                new TasRoute { path = "/api/tas/sysrole/roleinfo/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysrole/testsignal", method = "GET" },
                new TasRoute { path = "/api/tas/sysroleemployeemenu/getmenu/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysroleemployeereportdepartment/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysroleemployeereportemployer/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysroleemployeereporttemplate/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysrolemenu/getmenu/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysrolereporttemplate/*", method = "GET" },
                new TasRoute { path = "/api/tas/systeam", method = "GET" },
                new TasRoute { path = "/api/tas/systeam/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysversion", method = "GET" },
                new TasRoute { path = "/api/tas/sysversion/releasenote/*", method = "GET" },
                new TasRoute { path = "/api/tas/sysversion/versionhistory", method = "GET" },
                new TasRoute { path = "/api/tas/terminationtype", method = "GET" },
                new TasRoute { path = "/api/tas/transport", method = "GET" },
                new TasRoute { path = "/api/tas/transport/all/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/transport/scheduledetail", method = "GET" },
                new TasRoute { path = "/api/tas/transport/searchreschedulepeople", method = "POST" },
                new TasRoute { path = "/api/tas/transport/dateschedule", method = "POST" },
                new TasRoute { path = "/api/tas/transport/rescheduleexternaltransport", method = "PUT" },
                new TasRoute { path = "/api/tas/transport/noshow/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/transport/employee/existingtransport/*/*/*", method = "GET" },
                new TasRoute { path = "/api/tas/transportmode", method = "GET" },
                new TasRoute { path = "/api/tas/transportschedule/*", method = "GET" },
                new TasRoute { path = "/api/tas/transportschedule/monthtransportschedule", method = "POST" },
                new TasRoute { path = "/api/tas/transportschedule/manageschedule", method = "POST" },
                new TasRoute { path = "/api/auth/auth/userinfo", method = "GET" },
                new TasRoute { path = "/api/auth/auth/loginadservice", method = "GET" },
                new TasRoute { path = "/api/auth/auth/impersonite/*", method = "GET" },
                new TasRoute { path = "/api/auth/auth/loginadservicecheck", method = "POST" },
                new TasRoute { path = "/api/home/index", method = "GET" },
                new TasRoute { path = "/index2", method = "GET" },
                new TasRoute { path = "/api/auth/menu", method = "GET" },
                new TasRoute { path = "/api/tas/test", method = "POST" },
                new TasRoute { path = "/api/auth/user", method = "GET" },
            };

        }



        public class TasRoute
        {
            public string path { get; set; }
            public string method { get; set; }
        }








    }
}
