using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeData;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class EmployeeRepository
    {

        #region ChangeCostcode

    
        private async Task ChangeEmployeeTransportStatusData(int EmployeeId, int DataId, string column, CancellationToken cancellationToken)
        {
             var allowedColumns = new List<string>() {"EmployerId","PositionId", "DepartmentId", "CostCodeId"};
            if (allowedColumns.Contains(column))
            {
                if (column == "DepartmentId")
                {
                    var transportUpdateQuery = @$" UPDATE Transport
                        SET DepId = @DataId, 
                            UserIdUpdated = @UserIdUpdated, 
                            DateUpdated = @DateUpdated
                        WHERE EmployeeId = @EmployeeId
                        AND EventDate >= @Today";

                    await Context.Database.ExecuteSqlRawAsync(transportUpdateQuery,
                        new SqlParameter("@DataId", DataId),
                        new SqlParameter("@UserIdUpdated", _hTTPUserRepository.LogCurrentUser()?.Id ?? (object)DBNull.Value),
                        new SqlParameter("@DateUpdated", DateTime.Now),
                        new SqlParameter("@EmployeeId", EmployeeId),
                        new SqlParameter("@Today", DateTime.Today));

                    // Update EmployeeStatus table
                    var statusUpdateQuery = @" UPDATE EmployeeStatus
                    SET DepId = @DataId, 
                        UserIdUpdated = @UserIdUpdated, 
                        DateUpdated = @DateUpdated
                    WHERE EmployeeId = @EmployeeId
                    AND EventDate >= @Today";

                    await Context.Database.ExecuteSqlRawAsync(statusUpdateQuery,
                                    new SqlParameter("@DataId", DataId),
                                    new SqlParameter("@UserIdUpdated", _hTTPUserRepository.LogCurrentUser()?.Id ?? (object)DBNull.Value),
                                    new SqlParameter("@DateUpdated", DateTime.Now),
                                    new SqlParameter("@EmployeeId", EmployeeId),
                                    new SqlParameter("@Today", DateTime.Today));
                }

                else {
                    var transportUpdateQuery = @$" UPDATE Transport
                        SET {column} = @DataId, 
                            UserIdUpdated = @UserIdUpdated, 
                            DateUpdated = @DateUpdated
                        WHERE EmployeeId = @EmployeeId
                        AND EventDate >= @Today";

                    await Context.Database.ExecuteSqlRawAsync(transportUpdateQuery,
                        new SqlParameter("@DataId", DataId),
                        new SqlParameter("@UserIdUpdated", _hTTPUserRepository.LogCurrentUser()?.Id ?? (object)DBNull.Value),
                        new SqlParameter("@DateUpdated", DateTime.Now),
                        new SqlParameter("@EmployeeId", EmployeeId),
                        new SqlParameter("@Today", DateTime.Today));

                    // Update EmployeeStatus table
                    var statusUpdateQuery = @$" UPDATE EmployeeStatus
                    SET {column} = @DataId, 
                        UserIdUpdated = @UserIdUpdated, 
                        DateUpdated = @DateUpdated
                    WHERE EmployeeId = @EmployeeId
                    AND EventDate >= @Today";

                    await Context.Database.ExecuteSqlRawAsync(statusUpdateQuery,
                                    new SqlParameter("@DataId", DataId),
                                    new SqlParameter("@UserIdUpdated", _hTTPUserRepository.LogCurrentUser()?.Id ?? (object)DBNull.Value),
                                    new SqlParameter("@DateUpdated", DateTime.Now),
                                    new SqlParameter("@EmployeeId", EmployeeId),
                                    new SqlParameter("@Today", DateTime.Today));
                }


            }

        
        }

        #endregion


    }
}
