using Application.Common.Exceptions;
using Domain.CustomModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {



        private async Task<ProfileMasterData> GetMasterData(string? columnName, string? columnValue)
        {


            var allowedFieldName = InitReportMasterData();
            var returnData = new ProfileMasterData();

            if (columnName == null) {
                return new ProfileMasterData { ColumnValue = columnValue, ColumnCaption = columnName };
            }

            if (allowedFieldName.Where(x=> x.ColumnName == columnName)?.FirstOrDefault() == null)
            {
                return new ProfileMasterData { ColumnValue = columnValue, ColumnCaption = columnName };   
            }


            if (allowedFieldName.Where(x => x.ColumnName == columnName)?.FirstOrDefault()  != null && columnValue == null)
            {
                return new ProfileMasterData { ColumnValue = columnValue, ColumnCaption = allowedFieldName.Where(x => x.ColumnName == columnName)?.FirstOrDefault()?.ColumnCaption};
            }



            var currentData = allowedFieldName.Where(x => x.ColumnName == columnName)?.FirstOrDefault();

            if (currentData != null)
            {
                returnData.ColumnCaption = currentData.ColumnCaption;
                returnData.ColumnValue = await ExecuteQueryGetMasterData(currentData,columnValue);
                return returnData;
            }
            else {
                return new ProfileMasterData { ColumnValue = columnValue, ColumnCaption = columnName };
            }





        }


        private List<ProfileMasterAllowedColumns> InitReportMasterData() 
        {
            var returnData = new List<ProfileMasterAllowedColumns>();
            //"RosterId", "FlightGroupMasterId", "EmployerId", "PeopleTypeId", "NationalityId", "StateId", "RoomId", "DepartmentId", "LocationId", "CostCodeId"
            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "RosterId",
                ColumnCaption = "Roster",
                Query = "Select Id, name Description from Roster"
            }) ;

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "FlightGroupMasterId",
                ColumnCaption = "TransportGroup",
                Query = "SELECT Id, Description from GroupMaster"
            });

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "EmployerId",
                ColumnCaption = "Employer",
                Query = "SELECT Id, Description from Employer"
            });

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "PeopleTypeId",
                ColumnCaption = "PeopleType",
                Query = "SELECT Id, Description from PeopleType"
            });
            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "NationalityId",
                ColumnCaption = "Nationality",
                Query = "SELECT Id, Description from Nationality"
            });

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "StateId",
                ColumnCaption = "State",
                Query = "SELECT Id, Description from State"
            });

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "StateId",
                ColumnCaption = "State",
                Query = "SELECT Id, Description from State"
            });

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "RoomId",
                ColumnCaption = "Room",
                Query = "SELECT Id, Number Description from Room"
            });


            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "DepartmentId",
                ColumnCaption = "Department",
                Query = "SELECT Id, Name Description from Department"
            });


            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "LocationId",
                ColumnCaption = "Location",
                Query = "SELECT Id,  Description from Location"
            });


            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "CostCodeId",
                ColumnCaption = "CostCode",
                Query = "SELECT Id, Concat(Number, ' ', Description) Description from CostCodes"
            });

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "RoomTypeId",
                ColumnCaption = "RoomType",
                Query = "SELECT Id, Description from RoomType"
            });

            returnData.Add(new ProfileMasterAllowedColumns
            {
                ColumnName = "CampId",
                ColumnCaption = "Camp",
                Query = "SELECT Id, Description from Camp"
            });



            return returnData;

        }


        public async Task<string?> ExecuteQueryGetMasterData(ProfileMasterAllowedColumns data, string? columnValue)
        {
            try
            {
                var mdata =await GetProfileMasterCacheData(data);
                if (mdata.Any())
                {
                    return mdata.Where(x => x.Id == columnValue).FirstOrDefault()?.Description;
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

        private async Task<List<ProfileMasterCacheData>> GetProfileMasterCacheData(ProfileMasterAllowedColumns data)
        {

            var outData = new List<ProfileMasterCacheData>();

            if (_memoryCache.TryGetValue($"REPORT_PROFILE_AUDIT_{data.ColumnName}", out outData))
            {
                return outData;
            }
            else
            {
                Console.WriteLine("-------------------------DVV-------------------------");
                var masterData = await GetRawQueryData<ProfileMasterCacheData>(data.Query);
                _memoryCache.Set($"REPORT_PROFILE_AUDIT_{data.ColumnName}", masterData);

                return masterData;

            }

        }





        private async Task<List<T>> GetRawQueryData<T>(string query)
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
                            var resultList = new List<T>();
                            while (await result.ReadAsync())
                            {
                                var d = (IDictionary<string, object>)new ExpandoObject();
                                for (int i = 0; i < result.FieldCount; i++)
                                {
                                    d.Add(result.GetName(i), result.IsDBNull(i) ? null : result.GetValue(i));
                                }

                                resultList.Add(JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(d)));
                            }
                            return resultList;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("An error occurred while executing the query.");
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




    }




    public class ProfileMasterData
    { 
        public string? ColumnCaption { get; set; }

        public string?  ColumnValue { get; set; }

    }


    public class ProfileMasterCacheData
    {
        public string Id { get; set; }
        public string? Description { get; set; }


    }



    public class ProfileMasterAllowedColumns
    {
        public string ColumnName { get; set; }
        public string? ColumnCaption { get; set; }
        public string? Query { get; set; }



    }
}
