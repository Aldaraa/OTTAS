using Application.Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {

        private async Task<List<int>> DepartmentHierarchyIds(List<int> departmentIds)
        {


            return await GetRawQueryData(GetDepartmentHierarchyQuery(departmentIds));

           

        }


        #region QueryData

        private string GetDepartmentHierarchyQuery(List<int> Ids)
        {



            string departmentParam = string.Join(", ", Ids);

            return @$"WITH DepartmentHierarchy AS (
                        SELECT 
                            Id,
                            ParentDepartmentId
                        FROM 
                            dbo.Department
                        WHERE 
                            Id  IN ({departmentParam})
                        UNION ALL
                        SELECT 
                            d.Id,
                            d.ParentDepartmentId
                        FROM 
                            dbo.Department d
                        INNER JOIN 
                            DepartmentHierarchy dh ON dh.Id = d.ParentDepartmentId
                    )
                    SELECT 
                        Id
                    FROM 
                        DepartmentHierarchy";
        }
        #endregion



        #region RawQueryExecute  

        private async Task<List<int>> GetRawQueryData(string query)
        {
            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        await sqlConnection.OpenAsync();
                    }

                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandTimeout = 300;

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            var resultList = new List<int>();
                            while (await result.ReadAsync())
                            {
                                // Assuming the integer value is in the first column
                                if (!result.IsDBNull(0))
                                {
                                    resultList.Add(result.GetInt32(0));
                                }
                            }
                            return resultList;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new BadRequestException("Report failed");
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("Report failed");
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        await sqlConnection.CloseAsync();
                    }
                }
            }
            else
            {
                throw new BadRequestException("Database connection is not of type SqlConnection.");
            }
        }

        #endregion




    }
}
