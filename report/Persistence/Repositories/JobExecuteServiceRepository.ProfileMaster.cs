using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository :  BaseRepository<ReportJob>, IJobExecuteServiceRepository
    {
        #region ProfileMasterData

        private bool hasGroupMaster = false;

        private async Task<string> Profilemaster(List<ReportCol> FieldNames, string reportName, List<ReportParam> reportParams)
        {
            string fields = "";
            string fieldAliases = "";
            foreach (var field in FieldNames)
            {

                fields += $", {field.FieldName} AS {field.FieldCaption}";
                fieldAliases += $", {field.FieldCaption}";


            }

            var employeeId = await GetUserId();

            var query = @$"select profiledata.id 'Person #', CASE WHEN  profiledata.Active = 1 THEN 'Yes' else 'No' End as Active,  profiledata.SAPID,  profiledata.firstname Firstname,
                        profiledata.lastname Lastname,
                        department.name Department, employer.description Employer,
                        position.description Position, 
                        peopletype.code Peopletype,
                        roster.description Roster,
                        case    when profiledata.gender = 1 then 'male'    else 'female'  end as Gender,
                        Concat('+976', profiledata.PersonalMobile) Mobile
                        {fields} from employee as profiledata
                         left join reportdeparmentdata department on profiledata.departmentid = department.id 
                         left join costcodes costcodes on profiledata.costcodeid = costcodes.id
                         left join employer employer on profiledata.employerid = employer.id
                         left join position position on profiledata.positionid = position.id
                         left join peopletype peopletype on profiledata.peopletypeid = peopletype.id
                         left join roster roster on profiledata.rosterid = roster.id
                     /*    left join reportprofiledata profile on profiledata.id = profile.personno */
                             inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo";



            var query2 = @$"SELECT PersonId 'Person #',  CASE WHEN  Active = 1 THEN 'Yes' else 'No' End as Active, Firstname, Lastname, Department, Employer, 
                           Position, PeopleType, Roster, Gender, Mobile ,[COLS] {fieldAliases}
                    FROM (
                        SELECT 
                            ProfileData.Id AS PersonId, 
                            ProfileData.Active,
                            ProfileData.SAPID,  
                            ProfileData.Firstname,
                            ProfileData.Lastname,
                            Department.Name AS Department, 
                            Employer.Description AS Employer,
                            Position.Description AS Position, 
                            PeopleType.Code AS PeopleType,
                            Roster.Description AS Roster,
                            CASE WHEN ProfileData.gender = 1 THEN 'Male' ELSE 'Female' END AS Gender,
                            ProfileData.Mobile,
                            gm.Description AS GroupMasterDescription, 
                            gd.Description AS GroupDetailDescription
                            {fields}
                        FROM 
                            Employee AS ProfileData
                            LEFT JOIN ReportDeparmentData Department ON ProfileData.DepartmentId = Department.Id 
                            LEFT JOIN CostCodes CostCodes ON ProfileData.CostCodeId = CostCodes.Id
                            LEFT JOIN Employer Employer ON ProfileData.EmployerId = Employer.Id
                            LEFT JOIN Position Position ON ProfileData.PositionId = Position.Id
                            LEFT JOIN PeopleType PeopleType ON ProfileData.PeopleTypeId = PeopleType.Id
                            LEFT JOIN Roster Roster ON ProfileData.RosterId = Roster.Id
                        /*    LEFT JOIN ReportProfileData Profile ON ProfileData.Id = Profile.PersonNo*/
                             inner join GetReportProfileData({employeeId}) Profile on ProfileData.Id = Profile.PersonNo

                            LEFT JOIN GroupMembers gmbr ON ProfileData.Id = gmbr.EmployeeId
                            LEFT JOIN GroupMaster gm ON gmbr.GroupMasterId = gm.Id
                            LEFT JOIN dbo.GroupDetail gd ON gmbr.GroupDetailId = gd.Id 
                            [WHERECONDITION]  [GROUPDETAILDESCRIPTIONCONDITION]
                    
                    ) x
                    PIVOT 
                    (
                        MAX(GroupDetailDescription)
                        FOR GroupMasterDescription IN ([COLS2])
                    ) p 
                    ORDER BY Firstname;";

            string whereCondition = " Where ProfileData.Active  [ACTIVESTATUS] ";
            foreach (var item in reportParams)
            {

                var ItemData = await GetParametervalue(item);
                if (ItemData.FieldName == "CampId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                    //    query += $"   LEFT JOIN Camp  ON ProfileData.RoomId in (select Id from room where CampId in {ItemData.FieldValue} )";



                        query += $" LEFT JOIN (select Id from room where CampId in {ItemData.FieldValue}) AS RoomData" +
                            " ON ProfileData.RoomId = RoomData.Id ";


                            whereCondition += $" AND ProfileData.RoomId is not NULL";


                    }
                }
                if (ItemData.FieldName == "EmployerId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.EmployerId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.EmployerId IN {item.FieldValue}";
                        }
                    }


                }

                if (ItemData.FieldName == "PositionId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.PositionId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.PositionId IN {item.FieldValue}";
                        }
                    }


                }
                if (ItemData.FieldName == "Active")
                {

                    string fieldValue = ItemData.FieldValue?.Trim('\"').Trim();
                    if (!string.IsNullOrEmpty(fieldValue) && fieldValue.Equals("No", StringComparison.OrdinalIgnoreCase))
                    {
                        whereCondition = whereCondition.Replace("[ACTIVESTATUS]", " != 2");
                    }
                    else {
                        whereCondition = whereCondition.Replace("[ACTIVESTATUS]", " = 1");
                    }
                }



                if (ItemData.FieldName == "StateId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.StateId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.StateId IN {item.FieldValue}";
                        }
                    }


                }
                if (ItemData.FieldName == "PeopleTypeId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.PeopleTypeId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.PeopleTypeId IN {item.FieldValue}";
                        }
                    }


                }
                if (ItemData.FieldName == "DepartmentId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.DepartmentId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.DepartmentId IN {item.FieldValue}";
                        }
                    }
                }
                if (ItemData.FieldName == "CostcodeId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.CostCodeId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.CostCodeId IN {item.FieldValue}";
                        }
                    }
                }
                if (ItemData.FieldName == "NationalityId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        if (whereCondition == "")
                        {
                            if (ItemData.FieldValue != "ALL")
                            {
                                whereCondition = $" WHERE ProfileData.NationalityId IN {item.FieldValue}";
                            }

                        }
                        else
                        {
                            whereCondition += $" AND ProfileData.NationalityId IN {item.FieldValue}";
                        }
                    }
                }
                if (ItemData.FieldName == "GroupMasterId")
                {
                    if (ItemData.FieldValue != "ALL")
                    {
                        var groupMasterQuery = @$"SELECT Id, gm.Description FROM GroupMaster gm WHERE gm.id IN (
                                SELECT gd.GroupMasterId FROM GroupDetail gd WHERE gd.id IN {ItemData.FieldValue})";
                        hasGroupMaster = true ; 

                        var groupColumns = await ExecuteGroupColumnData(groupMasterQuery);
                        if (!string.IsNullOrWhiteSpace(groupColumns))
                        {
                            query2 = query2.Replace("[GROUPDETAILDESCRIPTIONCONDITION]", $" AND gd.id IN {ItemData.FieldValue}");
                            query2 = query2.Replace("[COLS]", groupColumns);
                            query2 = query2.Replace("[COLS2]", groupColumns);



                        }


                    }

                }



            }

            if (hasGroupMaster)
            {
                whereCondition = whereCondition.Replace("[ACTIVESTATUS]", " = 1");
                query2 = query2.Replace("[WHERECONDITION]",  whereCondition);


                return await SaveDynamicListToExcelFile(query2, @"\Assets\GeneratedFiles\", reportName, reportParams);
            }
            else {

                whereCondition = whereCondition.Replace("[ACTIVESTATUS]", " = 1");

                query = query + " " + whereCondition + " Order by ProfileData.Firstname";
                return await SaveDynamicListToExcelFile(query, @"\Assets\GeneratedFiles\", reportName, reportParams);
            }

        }










        #endregion

    }
}
