using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Persistence.Repositories
{
    public partial class RequestDocumentRepository
    {
        public async Task<List<int>> GetRoleEmployeeIds()
        {
            var role = _HTTPUserRepository.LogCurrentUser()?.Role;
            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;
            var returnData = new List<int>();
            if (role == "DepartmentAdmin" || role == "DepartmentManager")
            {
                if (role == "DepartmentAdmin")
                {

                    var departmentIds = await Context.DepartmentAdmin.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();

                    List<int> RoleDepartmenIds = new List<int>();
                    foreach (var item in departmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds =await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }

                    if (RoleDepartmenIds.Count > 0)
                    {
                        returnData = await Context.Employee.AsNoTracking().Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value)).Select(x => x.Id).ToListAsync();
                    }
                    else
                    {
                        returnData.Add(userId.Value);
                    }


                }

                if (role == "DepartmentManager")
                {
                    List<int> RoleDepartmenIds = new List<int>();
                    var departmentIds = await Context.DepartmentManager.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();
                    foreach (var item in departmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds =await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }

                    if (RoleDepartmenIds.Count > 0)
                    {
                        returnData = await Context.Employee.AsNoTracking().Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value)).Select(x => x.Id).ToListAsync();
                    }
                    else
                    {
                        returnData.Add(userId.Value);
                    }



                }

            }
            if (role == "Guest")
            {
                returnData.Add(userId.Value);
            }



            return returnData;
        }

        public async Task<List<int>> GetAllChildDepartmentIds(int parentDepartmentId)
        {
            var ids = new HashSet<int>(); // Using HashSet to avoid duplicates
            await foreach (var id in GetChildDepartmentIdsRecursive(parentDepartmentId))
            {
                ids.Add(id);
            }
            return ids.ToList();
        }


        private async IAsyncEnumerable<int> GetChildDepartmentIdsRecursive(int departmentId)
        {
            var childDepartments = await Context.Department.AsNoTracking()
                .Where(d => d.ParentDepartmentId == departmentId)
                .Select(d => d.Id)
                .ToListAsync(); // Executing the query asynchronously

            foreach (var deptId in childDepartments)
            {
                yield return deptId;
                await foreach (var childDeptId in GetChildDepartmentIdsRecursive(deptId))
                {
                    yield return childDeptId;
                }
            }
        }


    }
}
