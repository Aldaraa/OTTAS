using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Enums;

namespace tas.Persistence.Repositories
{
    public partial class MailSmtpConfigRepository
    {

        private async Task<List<string?>> GetGroupEmails(int documentId)
        {
            try
            {
                var currentDocument = await Context.RequestDocument.Where(x => x.Id == documentId).FirstOrDefaultAsync();
                var returnData = new List<string?>();

                if (currentDocument != null)
                {
                    var requesterEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdCreated).FirstOrDefaultAsync();

                    if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {
                        if (currentDocument.DocumentType == RequestDocumentType.ProfileChanges)
                        {
                            if (requesterEmployee != null && !string.IsNullOrWhiteSpace(requesterEmployee.Email))
                            {
                                returnData.Add(requesterEmployee.Email);
                            }
                            return returnData;
                        }
                        else if (currentDocument.DocumentType == RequestDocumentType.DeMobilisation)
                        {

                            if (requesterEmployee != null && !string.IsNullOrWhiteSpace(requesterEmployee.Email))
                            {
                                returnData.Add(requesterEmployee.Email);
                            }
                            return returnData;
                            //var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "datateam").FirstOrDefaultAsync();
                            //if (currentGroup != null)
                            //{
                            //    var empIds = await Context.RequestGroupEmployee.Where(x => x.RequestGroupId == currentGroup.Id).Select(x => x.EmployeeId).ToListAsync();
                            //    empIds.Add(currentDocument.UserIdCreated);
                            //    if (empIds.Count > 0)
                            //    {
                            //        var dataTeamEmployees = await Context.Employee.AsNoTracking().Where(x => empIds.Contains(x.Id)).ToListAsync();
                            //        foreach (var item in dataTeamEmployees)
                            //        {
                            //            if (!string.IsNullOrWhiteSpace(item.Email))
                            //            {
                            //                returnData.Add(item.Email);
                            //            }
                            //        }
                            //    }
                            //}
                        }
                        else if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                        {
                            if (requesterEmployee != null && !string.IsNullOrWhiteSpace(requesterEmployee.Email))
                            {
                                returnData.Add(requesterEmployee.Email);
                            }



                            return returnData;
                        }
                        else if (currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                        {
                            if (requesterEmployee != null && !string.IsNullOrWhiteSpace(requesterEmployee.Email))
                            {
                                returnData.Add(requesterEmployee.Email);
                            }
                            return returnData;
                        }
                    }
                    else if (currentDocument.CurrentAction == RequestDocumentAction.Declined || currentDocument.CurrentAction == RequestDocumentAction.Cancelled)
                    {
                        if (requesterEmployee != null && !string.IsNullOrWhiteSpace(requesterEmployee.Email))
                        {
                            returnData.Add(requesterEmployee.Email);
                        }
                        return returnData;
                    }
                    else if (currentDocument.CurrentAction == RequestDocumentAction.Approved)
                    {
                        if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                        {
                            var assignedEmployee = await Context.Employee.AsNoTracking().Where(c => c.Id == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync();
                            if (assignedEmployee != null && !string.IsNullOrWhiteSpace(assignedEmployee.Email))
                            {
                                returnData.Add(assignedEmployee.Email);
                            }
                            return returnData;
                        }
                    }
                    else if (currentDocument.CurrentAction == RequestDocumentAction.Submitted)
                    {
                        
                        if (currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                        {
                            var assignedEmployee = await Context.Employee.AsNoTracking().Where(c => c.Id == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync();
                            if (assignedEmployee != null && !string.IsNullOrWhiteSpace(assignedEmployee.Email))
                            {
                                returnData.Add(assignedEmployee.Email);
                            }
                            return returnData;
                        }
                    }
                }
                return returnData;
            }

            catch (Exception ex)
            {
                return new List<string?>();
            }

        }



        private async Task<string?> GetResourceMail(int documentId)
        {
            try
            {
                var currentDocument = await Context.RequestDocument.Where(x => x.Id == documentId).FirstOrDefaultAsync();
                var returnData = new List<string?>();

                if (currentDocument != null)
                {
                    var resourceEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync();

                    if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {

                        if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel || currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                        {
                            if (resourceEmployee != null && !string.IsNullOrWhiteSpace(resourceEmployee.Email))
                            {
                                return resourceEmployee.Email;
                            }
                            else {
                                return null;
                            }


                        }
                        else {
                            return null;
                        }
                    }
                    else {
                        return null;
                    }

                }
                else {
                    return null;
                }
            }

            catch (Exception ex)
            {
                return null;
            }

        }


        private async Task<string?> GetRequesterMail(int documentId)
        {
            try
            {
                var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == documentId).FirstOrDefaultAsync();
                var returnData = new List<string?>();

                if (currentDocument != null)
                {
                    var requestEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdCreated).FirstOrDefaultAsync();
                        if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                        {
                            if (requestEmployee != null && !string.IsNullOrWhiteSpace(requestEmployee.Email))
                            {
                                return requestEmployee.Email;
                            }
                            else
                            {
                                return null;
                            }


                        }
                        else
                        {
                            return null;
                        }

                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                return null;
            }

        }

    }


}
        
   
