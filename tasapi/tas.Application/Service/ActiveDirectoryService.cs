using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.CustomModel;

namespace tas.Application.Service
{
 public class ActiveDirectoryService
    {


        public bool ValidateCredentials(string domain, string userName, string password)
        {
            try
            {
                userName = userName.Trim();
                password = password.Trim();

                using (var context = new PrincipalContext(ContextType.Domain, domain))
                {
                    return context.ValidateCredentials(userName, password);
                }
            }
            catch (Exception ex)
            {


                return false;
            }

        }

        public ADUser GetUserFromAd(string domain, string userName)
        {
            try
            {
                ADUser result = null;
                userName = userName.Trim();
                using (var context = new PrincipalContext(ContextType.Domain, domain))
                {
                    var user = UserPrincipal.FindByIdentity(context, userName);
                    if (user != null)
                    {
                        result = new ADUser
                        {
                            Id = 0,
                            UserName = userName,
                            Active = user.Enabled,
                            UserPrincipalName = user.UserPrincipalName,
                            DisplayName = user.DisplayName,
                            SamAccountName = user.SamAccountName,
                            DistinguishedName = user.DistinguishedName,
                            Guid = user.Guid

                        };
                    }
                    else
                    {

                        return null;
                    }
                }
                return result;
            }

            catch (Exception ex)
            {
                var es = ex;
                return null;

            }

        }
    }
}
