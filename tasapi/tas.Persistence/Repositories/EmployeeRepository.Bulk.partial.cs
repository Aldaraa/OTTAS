using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.BulkDownloadEmployee;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Policy;
using System.Drawing;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCode;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Application.Features.EmployeeFeature.BulkUploadEmployee;
using tas.Domain.Enums;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployee;
using tas.Application.Common.Exceptions;
using Microsoft.Data.SqlClient;
using System.Data;
using tas.Application.Features.TransportFeature.CheckDataRequest;
using MediatR;
using System.Dynamic;
using tas.Application.Features.TransportFeature.GetDataRequest;
using System.Reflection.PortableExecutable;
using tas.Persistence.Context;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.StatusChangesEmployeeRequestFeature.DeleteStatusChangesEmployeeRequest;
using tas.Application.Features.DashboardSystemAdminFeature.GetPeopleTypeAndDepartment;
using Newtonsoft.Json;

namespace tas.Persistence.Repositories
{
    public partial class EmployeeRepository
    {
        #region BatchDownload

        private async Task<List<dynamic>> GetRawEmployeeData( List<GroupMaster> groupData, List<int> empIds)
        {

            string groupName = string.Join(",", groupData.Select(x => "[" + x.Description + "_group" + "]"));
            string whereCondition = "";
            if (empIds.Count > 0)
            {
                string ids = string.Join(",", empIds);
                whereCondition = $" AND e.Id in ({ids})";
            }

            string queryData = "";
            if (groupData.Count > 0)
            {
                queryData = @$"SELECT Id, Firstname, Lastname, MFirstname, MLastname, 
                                 Mobile Phone, 
                                 PersonalMobile,
                                 FrequentFlyer,  EmergencyContactMobile, EmergencyContactName,
                                 Dob, Gender, SAPID, Email,
                                 PassportNumber, PassportName NameOnPassport, CompletionDate, PassportExpiry, CommenceDate,
                                 NRN, HotelCheck, Nationality, Employer,
                                 State, ContractNumber, CostCode, Department, Position, Roster,
                                 Location,
                                 PeopleType ResourceType,
                                 PickUpAddress,
                                 TransportGroup,
                                 {groupName} 
                                    FROM 
                                    (
                                       SELECT e.Id, e.Lastname, e.Firstname, e.MFirstname, e.MLastname,
                                              e.Mobile, e.PersonalMobile, e.Dob, e.gender Gender, e.SAPID, e.Email,
                                              e.PassportNumber, e.PassportName, e.PassportExpiry, e.CommenceDate, 
                                              e.NRN, e.HotelCheck, n.Description AS Nationality, emp.Description AS Employer,
                                              s.Description AS State, e.ContractNumber,
                                              e.FrequentFlyer,
                                              e.CompletionDate,

                                              Concat(cc.Number, ' ', cc.Description) AS CostCode,
                                              Concat(d.Id,'.', d.Name) AS Department, p.Description AS Position,
                                              r.Description AS Roster,
                                              l.Description AS Location,
                                              pt.Description AS PeopleType,
                                              e.PickUpAddress,
                                              fgm.Description AS TransportGroup,
                                              e.EmergencyContactName,
                                              e.EmergencyContactMobile,
                                              gd.Description AS GroupDetailName,
                    
                                              Concat(gm.Description, '_group') AS GroupMasterName
                                       FROM Employee e
                                       LEFT JOIN Nationality n ON e.NationalityId = n.Id
                                       LEFT JOIN Employer emp ON e.EmployerId = emp.Id
                                       LEFT JOIN State s ON e.StateId = s.Id
                                       LEFT JOIN CostCodes cc ON e.CostCodeId = cc.Id
                                       LEFT JOIN Department d ON d.Id = e.DepartmentId
                                       LEFT JOIN Position p ON e.PositionId = p.Id
                                       LEFT JOIN Roster r ON e.RosterId = r.Id
                                       LEFT JOIN Location l ON e.LocationId = l.Id
                                       LEFT JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                                       LEFT JOIN FlightGroupMaster fgm ON e.FlightGroupMasterId = fgm.Id
                                       left JOIN GroupMembers gm1 ON e.Id = gm1.EmployeeId
                                       left JOIN GroupDetail gd ON gm1.GroupDetailId = gd.id
                                       left JOIN GroupMaster gm ON gd.GroupMasterId = gm.id
                                       WHERE e.Active = 1 {whereCondition}
                                    ) x
                                    PIVOT
                                    (
                                        MAX(GroupDetailName)
                                        FOR GroupMasterName IN ({groupName})
                                    ) p";

            }
            else {
                queryData = @$"SELECT e.Id, e.Lastname, e.Firstname, e.MFirstname, e.MLastname,
                                              e.Mobile Phone, 
                                              e.PersonalMobile,
                                              e.Dob, e.gender Gender, e.SAPID, e.Email,
                                              e.PassportNumber, e.PassportName NameOnPassport, e.PassportExpiry, e.CommenceDate, 
                                              e.NRN, e.HotelCheck, n.Description AS Nationality, emp.Description AS Employer,
                                              s.Description AS State, e.ContractNumber,
                                              e.FrequentFlyer,
                                              e.CompletionDate,

                                              Concat(cc.Number,' ', cc.Description) AS CostCode,
                                              Concat(d.Id,'.', d.Name)  AS Department, p.Description AS Position,
                                              r.Description AS Roster,
                                              l.Description AS Location,
                                              pt.Description AS ResourceType,
                                              e.PickUpAddress,
                                              fgm.Description AS TransportGroup,
                                              e.EmergencyContactName,
                                              e.EmergencyContactMobile
                                       FROM Employee e
                                       LEFT JOIN Nationality n ON e.NationalityId = n.Id
                                       LEFT JOIN Employer emp ON e.EmployerId = emp.Id
                                       LEFT JOIN State s ON e.StateId = s.Id
                                       LEFT JOIN CostCodes cc ON e.CostCodeId = cc.Id
                                       LEFT JOIN Department d ON d.Id = e.DepartmentId
                                       LEFT JOIN Position p ON e.PositionId = p.Id
                                       LEFT JOIN Roster r ON e.RosterId = r.Id
                                       LEFT JOIN Location l ON e.LocationId = l.Id
                                       LEFT JOIN PeopleType pt ON e.PeopleTypeId = pt.Id
                                       LEFT JOIN FlightGroupMaster fgm ON e.FlightGroupMasterId = fgm.Id
                                       WHERE e.Active = 1 {whereCondition}
                                    ";

            }


            var returnData = new List<dynamic>();
            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                try
                {

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        await sqlConnection.OpenAsync();
                    }
                    if (sqlConnection.State == ConnectionState.Open)
                    {

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.CommandText = queryData;
                            command.CommandTimeout = 300;


                            try
                            {
                                using (var result = await command.ExecuteReaderAsync())
                                {
                                    var dynamicList = new List<dynamic>();
                                    int rowNumber = 0;
                                    while (await result.ReadAsync())
                                    {
                                        dynamic d = new ExpandoObject();
                                        d.No = ++rowNumber;
                                        for (int i = 0; i < result.FieldCount; i++)
                                        {
                                            if (result.GetName(i) == "PersonalMobile")
                                            {
                                                if (result[i] != DBNull.Value)
                                                {
                                                    ((IDictionary<string, object>)d).Add(result.GetName(i), result[i]);
                                                }
                                                else
                                                {
                                                    ((IDictionary<string, object>)d).Add(result.GetName(i), string.Empty);
                                                }
                                            }
                                            else {
                                                ((IDictionary<string, object>)d).Add(result.GetName(i), result[i]);
                                            }


                                        }
                                        dynamicList.Add(d);
                                    }

                                    returnData = dynamicList;
                                    return returnData;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new BadRequestException(ex.Message);
                            }

                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Database connection is not open.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while executing the query.", ex);
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Database connection is not of type SqlConnection.");
            }
        }

        #endregion


        #region BulkDownload
        public async Task<BulkDownloadEmployeeResponse> BulkRequestDownload(BulkDownloadEmployeeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheetEmployee = package.Workbook.Worksheets.Add("Employee");
                    worksheetEmployee.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);
                    var worksheetNationality = package.Workbook.Worksheets.Add("Nationality");
                    var worksheetEmployer = package.Workbook.Worksheets.Add("Employer");
                    var worksheetState = package.Workbook.Worksheets.Add("State");
                    var worksheetCostCode = package.Workbook.Worksheets.Add("CostCode");
                    var worksheetDepartment = package.Workbook.Worksheets.Add("Department");
                    var worksheetPosition = package.Workbook.Worksheets.Add("Position");
                    var worksheetRoster = package.Workbook.Worksheets.Add("Roster");
                    var worksheetLocation = package.Workbook.Worksheets.Add("Location");
                    var worksheetPeopleType = package.Workbook.Worksheets.Add("PeopleType");
                    var worksheetGender = package.Workbook.Worksheets.Add("Gender");

                    // var worksheetRoom = package.Workbook.Worksheets.Add("Room");
                    var worksheetTransportGroup = package.Workbook.Worksheets.Add("TransportGroup");



                    foreach (var item in package.Workbook.Worksheets)
                    {
                        if (item.Name != "Employee")
                        {
                            item.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);
                        }

                    }

                    var groupData = await Context.GroupMaster
                                        .AsNoTracking()
                                        .Where(x => x.ShowOnProfile == 1 && x.Description != null)
                                        .ToListAsync();

                    worksheetEmployee.Workbook.CalcMode = ExcelCalcMode.Automatic;

                    var worksheetNationalityChangeData = await ModfyNationalitySheet(worksheetNationality);
                    worksheetNationality = worksheetNationalityChangeData.sheet;

                    var worksheetEmployerChangeData = await ModifyEmployerSheet(worksheetEmployer);
                    worksheetEmployer = worksheetEmployerChangeData.sheet;

                    var worksheetStateChangeData = await ModifyStateSheet(worksheetState);
                    worksheetState = worksheetStateChangeData.sheet;


                    var worksheetCostCodeChangeData = await ModifyCostCodeSheet(worksheetCostCode);
                    worksheetCostCode = worksheetCostCodeChangeData.sheet;

                    var worksheetDepartmentChangeData = await ModifyDepartmentSheet(worksheetDepartment, cancellationToken);
                    worksheetDepartment = worksheetDepartmentChangeData.sheet;

                    var worksheetPositionChangeData = await ModifyPositionSheet(worksheetPosition);
                    worksheetPosition = worksheetPositionChangeData.sheet;

                    var worksheetRosterChangeData = await ModifyRosterSheet(worksheetRoster);
                    worksheetRoster = worksheetRosterChangeData.sheet;

                    var worksheetLocationChangeData = await ModifyLocationSheet(worksheetLocation);
                    worksheetLocation = worksheetLocationChangeData.sheet;

                    //PeopleType
                    var worksheetPeopleTypeChangeData = await ModifyPeopleTypeSheet(worksheetPeopleType);
                    worksheetPeopleType = worksheetPeopleTypeChangeData.sheet;


                    var worksheetGenderChangeData = ModfyGenderSheet(worksheetGender);
                    worksheetGender = worksheetGenderChangeData.sheet;


                    //var worksheetRoomChangeData = await ModifyRoomSheet(worksheetRoom);
                    //worksheetRoom = worksheetRoomChangeData.sheet;

                    var worksheetTransportGroupChangeData = await ModifyTransportGroupSheet(worksheetTransportGroup);
                    worksheetTransportGroup = worksheetTransportGroupChangeData.sheet;





                    List<string> ActionModes = new List<string> { "NONE", "ADD", "UPDATE" };

                    string ActionModesList = string.Join(",", ActionModes);

                    worksheetEmployee.Row(1).Style.Font.Bold = true;
                    var headerCells = worksheetEmployee.Cells[1, 1, 1, 32 + groupData.Count];
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Font.Size = 13;
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                    headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));


  

                    var entities = await GetRawEmployeeData(groupData, request.EmpIds);


                    foreach (var item in groupData)
                    {
                        var worksheetItem = package.Workbook.Worksheets.Add(item.Description);
                        var worksheetItemChangeData = await ModfyworksheetItem(worksheetItem, item.Id);
                    }



                    worksheetEmployee.Cells[1, 1].Value = "Mode";
                    worksheetEmployee.Cells[1, 2].Value = "Id";
                    worksheetEmployee.Cells[1, 3].Value = "Firstname";
                    worksheetEmployee.Cells[1, 4].Value = "Lastname";
                    worksheetEmployee.Cells[1, 5].Value = "SAPID";
                    worksheetEmployee.Cells[1, 6].Value = "Department";
                    worksheetEmployee.Cells[1, 7].Value = "Email";

                    worksheetEmployee.Cells[1, 8].Value = "CostCode";
                    worksheetEmployee.Cells[1, 9].Value = "Employer";
                    worksheetEmployee.Cells[1, 10].Value = "Position";
                    worksheetEmployee.Cells[1, 11].Value = "NRN";
                    worksheetEmployee.Cells[1, 12].Value = "ResourceType";
                    worksheetEmployee.Cells[1, 13].Value = "Roster";
                    worksheetEmployee.Cells[1, 14].Value = "Nationality";
                    worksheetEmployee.Cells[1, 15].Value = "Aimag";
                    worksheetEmployee.Cells[1, 16].Value = "CommenceDate";
                    worksheetEmployee.Cells[1, 17].Value = "CompletionDate";
                    worksheetEmployee.Cells[1, 18].Value = "Phone";
                    worksheetEmployee.Cells[1, 19].Value = "PersonalMobile";
                    worksheetEmployee.Cells[1, 20].Value = "EmergencyContactMobile";
                    worksheetEmployee.Cells[1, 21].Value = "EmergencyContactName";
                    worksheetEmployee.Cells[1, 22].Value = "Dob";
                    worksheetEmployee.Cells[1, 23].Value = "Gender";
                    worksheetEmployee.Cells[1, 24].Value = "PassportNumber";
                    worksheetEmployee.Cells[1, 25].Value = "NameOnPassport";
                    worksheetEmployee.Cells[1, 26].Value = "PassportExpiry";
                    worksheetEmployee.Cells[1, 27].Value = "FrequentFlyer";
                    worksheetEmployee.Cells[1, 28].Value = "HotelCheck";
                    worksheetEmployee.Cells[1, 29].Value = "ContractNumber";
                    worksheetEmployee.Cells[1, 30].Value = "Location";
                    worksheetEmployee.Cells[1, 31].Value = "PickUpAddress";
                    worksheetEmployee.Cells[1, 32].Value = "TransportGroup";




                    int columnindex = 32;
                    foreach (var item in groupData)
                    {
                        columnindex++;
                        worksheetEmployee.Cells[1, columnindex].Value = item.Description;
                    }



                    var IdColumn = worksheetEmployee.Cells[1, 2];

                    if (IdColumn.Comment == null)
                    {
                        IdColumn.AddComment("TAS SYSTEM : If new employee register must be empty.");
                    }
                    else
                    {
                        IdColumn.Comment.Text = "TAS SYSTEM : If new employee register must be empty.";
                    }

                    var modeColumn = worksheetEmployee.Cells[1, 1];

                    if (modeColumn.Comment == null)
                    {
                        modeColumn.AddComment("TAS SYSTEM : If no changes are made to the data, set to 'NONE' mode. This affects processing speed.");
                    }
                    else
                    {
                        modeColumn.Comment.Text = "TAS SYSTEM : If no changes are made to the data, set to 'NONE' mode. This affects processing speed.";
                    }

                    //If no changes are made to the data, set to 'NONE' mode. This affects processing speed.


                    var validation = worksheetEmployee.DataValidations.AddListValidation($"A2:A{entities.Count() + 1}");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                    validation.ErrorTitle = "An invalid value was entered";
                    validation.Error = "Select a value from the list";

                    foreach (var item in ActionModes)
                    {
                        validation.Formula.Values.Add(item);
                    }


                    //CostCode

                    int row = 2;

       


                    int batchSize = 3000;
                    int totalRows = entities.Count();
                   // int row = 2;



                    for (int batch = 0; batch < Math.Ceiling(totalRows / (double)batchSize); batch++)
                    {
                        int startRow = batch * batchSize;
                        int endRow = Math.Min(startRow + batchSize, totalRows);

                        for (int i = startRow; i < endRow; i++)
                        {
                            var entity = entities[i];

                            string? gender = null;
                            string? personalMobile = string.Empty;
                            if (entity.Gender == 1)
                            {
                                gender = "Male";
                            }
                            if (entity.Gender == 0)
                            {
                                gender = "Female";
                            }

                            if (row == 10)
                            {
                                Console.WriteLine("aaa" + entity);
                                var aa = entity.PersonalMobile;
                            }

                            try
                            {
                                if (!string.IsNullOrWhiteSpace(entity.PersonalMobile))
                                {
                                    string mobile = Convert.ToString(entity.PersonalMobile); 
                                    if (!string.IsNullOrWhiteSpace(mobile))
                                    {
                                        bool isNumeric = mobile.All(char.IsDigit);
                                        if (isNumeric)
                                        {
                                            personalMobile = $"+976{mobile}";
                                        }
                                        else
                                        {
                                            personalMobile = mobile;
                                        }
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                personalMobile = entity.PersonalMobile;
                            }




                            worksheetEmployee.Cells[row, 1].Value = ActionModes[2];
                            worksheetEmployee.Cells[row, 2].Value = entity.Id;
                            worksheetEmployee.Cells[row, 3].Value = entity.Firstname;
                            worksheetEmployee.Cells[row, 4].Value = entity.Lastname;
                            worksheetEmployee.Cells[row, 5].Value = entity.SAPID;
                            worksheetEmployee.Cells[row, 6].Value = entity.Department;
                            worksheetEmployee.Cells[row, 7].Value = entity.Email;

                            worksheetEmployee.Cells[row, 8].Value = entity.CostCode;
                            worksheetEmployee.Cells[row, 9].Value = entity.Employer;
                            worksheetEmployee.Cells[row, 10].Value = entity.Position;
                            worksheetEmployee.Cells[row, 11].Value = entity.NRN;
                            worksheetEmployee.Cells[row, 12].Value = entity.ResourceType;
                            worksheetEmployee.Cells[row, 13].Value = entity.Roster;
                            worksheetEmployee.Cells[row, 14].Value = entity.Nationality;
                            worksheetEmployee.Cells[row, 15].Value = entity.State;
                            worksheetEmployee.Cells[row, 16].Value = entity.CommenceDate is DBNull ? null : ((DateTime)entity.CommenceDate).ToString("yyyy-MM-dd");
                            worksheetEmployee.Cells[row, 17].Value = entity.CompletionDate is DBNull ? null : ((DateTime)entity.CompletionDate).ToString("yyyy-MM-dd");
                            worksheetEmployee.Cells[row, 18].Value = entity.Phone;
                            worksheetEmployee.Cells[row, 19].Value = personalMobile;
                            worksheetEmployee.Cells[row, 20].Value = entity.EmergencyContactMobile;
                            worksheetEmployee.Cells[row, 21].Value = entity.EmergencyContactName;
                            worksheetEmployee.Cells[row, 22].Value = entity.Dob is DBNull ? null : ((DateTime)entity.Dob).ToString("yyyy-MM-dd");
                            worksheetEmployee.Cells[row, 23].Value = gender;
                            worksheetEmployee.Cells[row, 24].Value = entity.PassportNumber;
                            worksheetEmployee.Cells[row, 25].Value = entity.NameOnPassport;
                            worksheetEmployee.Cells[row, 26].Value = entity.PassportExpiry is DBNull ? null : ((DateTime)entity.PassportExpiry).ToString("yyyy-MM-dd");
                            worksheetEmployee.Cells[row, 27].Value = entity.FrequentFlyer;
                            worksheetEmployee.Cells[row, 28].Value = entity.HotelCheck;
                            worksheetEmployee.Cells[row, 29].Value = entity.ContractNumber;
                            worksheetEmployee.Cells[row, 30].Value = entity.Location;
                            worksheetEmployee.Cells[row, 31].Value = entity.PickUpAddress;
                            worksheetEmployee.Cells[row, 32].Value = entity.TransportGroup;


                            foreach (var item in groupData)
                            {
                                ExcelRangeBase cell = worksheetEmployee.Cells[1, 1, 1, worksheetEmployee.Dimension.End.Column]
                                .FirstOrDefault(c => c.Value != null && c.Value.ToString() == item.Description);
                                if (cell != null)
                                {
                                    object value = GetDynamicValue(entity, item.Description +"_group");
                                    worksheetEmployee.Cells[row, cell.EntireColumn.StartColumn].Value = value;
                                }
                                
                            }

                            Console.WriteLine("Row =========>" + row);
                            row++;
                        }
                    }

                    string cellRange = $"A2:A{row}";  

                    // Define a common style for easier management
                    var fillStyle = ExcelFillStyle.Solid;

                    // Add "ADD" condition
                    var formatAdd = worksheetEmployee.ConditionalFormatting.AddExpression(worksheetEmployee.Cells[cellRange]);
                    formatAdd.Style.Fill.PatternType = fillStyle;
                    formatAdd.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.ActionModeAddColor));
                    formatAdd.Formula = "A2=\"ADD\"";  // This assumes the rule applies for the range starting at A2

                    // Add "UPDATE" condition
                    var formatUpdate = worksheetEmployee.ConditionalFormatting.AddExpression(worksheetEmployee.Cells[cellRange]);
                    formatUpdate.Style.Fill.PatternType = fillStyle;
                    formatUpdate.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor));
                    formatUpdate.Formula = "A2=\"UPDATE\"";



                    var NationalityValidation = worksheetEmployee.DataValidations.AddListValidation($"N2:N{entities.Count() + 1}");
                    NationalityValidation.Formula.ExcelFormula = $"Nationality!$A$2:$A${worksheetNationalityChangeData.sheetDatas.Count() + 1}";

                    var EmployerValidation = worksheetEmployee.DataValidations.AddListValidation($"I2:I{entities.Count() + 1}");
                    EmployerValidation.Formula.ExcelFormula = $"Employer!$A$2:$A${worksheetEmployerChangeData.sheetDatas.Count() + 1}";

                    var StateValidation = worksheetEmployee.DataValidations.AddListValidation($"O2:O{entities.Count() + 1}");
                    StateValidation.Formula.ExcelFormula = $"State!$A$2:$A${worksheetStateChangeData.sheetDatas.Count() + 1}";


                    var CostCodeValidation = worksheetEmployee.DataValidations.AddListValidation($"H2:H{entities.Count() + 1}");
                    CostCodeValidation.Formula.ExcelFormula = $"CostCode!$A$2:$A${worksheetCostCodeChangeData.sheetDatas.Count() + 1}";


                    var DepartmentValidation = worksheetEmployee.DataValidations.AddListValidation($"F2:F{entities.Count() + 1}");
                    DepartmentValidation.Formula.ExcelFormula = $"Department!$A$2:$A${worksheetDepartmentChangeData.sheetDatas.Count() + 1}";


                    var PositionValidation = worksheetEmployee.DataValidations.AddListValidation($"J2:J{entities.Count() + 1}");
                    PositionValidation.Formula.ExcelFormula = $"Position!$A$2:$A${worksheetPositionChangeData.sheetDatas.Count() + 1}";


                    var RosterValidation = worksheetEmployee.DataValidations.AddListValidation($"M2:M{entities.Count() + 1}");
                    RosterValidation.Formula.ExcelFormula = $"Roster!$A$2:$A${worksheetRosterChangeData.sheetDatas.Count() + 1}";


                    var LocationValidation = worksheetEmployee.DataValidations.AddListValidation($"AD2:AD{entities.Count() + 1}");
                    LocationValidation.Formula.ExcelFormula = $"Location!$A$2:$A${worksheetLocationChangeData.sheetDatas.Count() + 1}";


                    var PeopleTypeValidation = worksheetEmployee.DataValidations.AddListValidation($"L2:L{entities.Count() + 1}");
                    PeopleTypeValidation.Formula.ExcelFormula = $"PeopleType!$A$2:$A${worksheetPeopleTypeChangeData.sheetDatas.Count() + 1}";

                    var GenderValidation = worksheetEmployee.DataValidations.AddListValidation($"V2:V{entities.Count() + 1}");
                    GenderValidation.Formula.ExcelFormula = $"Gender!$A$2:$A${worksheetGenderChangeData?.sheetDatas.Count() + 1}";


                    var TransportGroupValidation = worksheetEmployee.DataValidations.AddListValidation($"AF2:AF{entities.Count() + 1}");
                    TransportGroupValidation.Formula.ExcelFormula = $"TransportGroup!$A$2:$A${worksheetTransportGroupChangeData.sheetDatas.Count() + 1}";


                    //DOB Date

                    foreach (var item in groupData)
                    {
                        ExcelRangeBase cell = worksheetEmployee.Cells[1, 1, 1, worksheetEmployee.Dimension.End.Column]
                            .FirstOrDefault(c => c.Value != null && c.Value.ToString() == item.Description);
                        if (cell != null)
                        {

                            string address = cell.Address.Replace("1", "");
                            var detailCnt = await Context.GroupDetail.Where(x => x.GroupMasterId == item.Id).CountAsync();
                            var ItemGroupValidation = worksheetEmployee.DataValidations.AddListValidation($"{address}2:{address}{entities.Count() + 1}");
                            ItemGroupValidation.Formula.ExcelFormula = $"'{item.Description}'!$A$2:$A${detailCnt + 1}";

                        }

                    }


                    
                    headerCells.AutoFilter = true;
                    worksheetEmployee.Cells.AutoFitColumns();
                    worksheetEmployee.View.FreezePanes(2, 7);
                    package.Save();

                    return new BulkDownloadEmployeeResponse
                    {
                        ExcelFile = package.GetAsByteArray()
                    };

                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }


        }


      

        private object GetDynamicValue(dynamic entity, string description)
        {
            IDictionary<string, object> dictEntity = entity as IDictionary<string, object>;
            if (dictEntity != null && dictEntity.ContainsKey(description))
            {
                return dictEntity[description];
            }
            else
            {
                return null; // Handle unknown descriptions or null entity
            }
        }


        private  ModfySheetReturnData ModfyGenderSheet(ExcelWorksheet sheet)
        {
            var genders = new List<string>();
            genders.Add("Female");
            genders.Add("Male");

            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in genders)
            {
                sheet.Cells[row, 1].Value = $"{entity}";

                returnData.Add(new SheetData { Description = entity });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }



        private async Task<ModfySheetReturnData> ModfyNationalitySheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Nationality.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData { Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }

        private async Task<ModfySheetReturnData> ModfyworksheetItem(ExcelWorksheet sheet, int masterId)
        {
            var Camps = await Context.GroupDetail.Where(x=> x.GroupMasterId == masterId).Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData { Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }

        private async Task<ModfySheetReturnData> ModifyEmployerSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Employer.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData { Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }

        private async Task<ModfySheetReturnData> ModifyStateSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.State.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData { Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }


        private async Task<ModfySheetReturnData> ModifyCostCodeSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.CostCodes.Select(x => new { x.Id, x.Number, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Number} {entity.Description}";

                returnData.Add(new SheetData {  Description = $"{entity.Number} {entity.Description}" });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }
        private async Task<ModfySheetReturnData> ModifyDepartmentSheet(ExcelWorksheet sheet, CancellationToken cancellationToken)
        {
            //var Camps = await Context.Department.Select(x => new { x.Id, x.Name }).ToListAsync();

            var departments = await GetDeparmentData(cancellationToken);

            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1:B1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            sheet.Cells[1, 2].Value = "ParentDepartment";

            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in departments)
            {
                sheet.Cells[row, 1].Value = $"{entity.Id}.{entity.OriginalName}";
                sheet.Cells[row, 2].Value = $"{entity.Name}";

                returnData.Add(new SheetData {Description = entity.Name });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        } 
        



        
        private async Task<ModfySheetReturnData> ModifyPositionSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Position.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData {Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }  
        
        
        private async Task<ModfySheetReturnData> ModifyRosterSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Roster.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData {Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }


        private async Task<ModfySheetReturnData> ModifyLocationSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.Location.Select(x => new { x.Id, x.Description, x.onSite }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1:B1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            sheet.Cells[1, 2].Value = "On Site";

            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";
                sheet.Cells[row, 2].Value = $"{entity.onSite}";

                returnData.Add(new SheetData {Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }

        private async Task<ModfySheetReturnData> ModifyPeopleTypeSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.PeopleType.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData {Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        } 
        



        private async Task<ModfySheetReturnData> ModifyTransportGroupSheet(ExcelWorksheet sheet)
        {
            var Camps = await Context.FlightGroupMaster.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData = new List<SheetData>();
            foreach (var entity in Camps)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData {Description = entity.Description });
                row++;
            }
            sheet.Workbook.CalcMode = ExcelCalcMode.Automatic;
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }

        #endregion



        private async Task SaveGroupData(List<BulkUploadEmployeeGroup> data)
        { 
            var changeData = data.Where(x=> x.Mode != "NONE").ToList();
            var empIds = changeData.Where(x=> x.Id != null).Select(x=> x.Id).Distinct().ToList();
            var existingData =await Context.GroupMembers.Where(x => empIds.Contains(x.EmployeeId)).ToListAsync();

            foreach (var item in changeData)
            {

                if (item.Id == 427329) {
                    var aa = 0;
                }

                var currentData =  existingData.Where(x => x.GroupMasterId == item.MasterId && x.EmployeeId == item.Id).FirstOrDefault();
                if (currentData != null)
                {
                    if (item.DetailId != null)
                    {
                        currentData.GroupDetailId = item.DetailId;
                        currentData.DateUpdated = DateTime.Now;
                        currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                        Context.GroupMembers.Update(currentData);
                    }
                    else {
                        Context.GroupMembers.Remove(currentData);
                    }
                }
                else {

                    if (item.DetailId != null)
                    {
                        var newRecord = new GroupMembers
                        {
                            Active = 1,
                            DateCreated = DateTime.Now,
                            GroupDetailId = item.DetailId,
                            GroupMasterId = item.MasterId,
                            EmployeeId = item.Id,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                        };

                        Context.GroupMembers.Add(newRecord);
                    }   
                }
            }
        }


        private ValidateBulkEmployeeRow ValidateAddEmployee(BulkUploadEmployee item, List<Employee> activeEmployees, ExcelWorksheet worksheet)
        {
            var errormessages = new List<string>();
            var columns = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                .Select(c => c.Text.Trim())
                .ToList();

            int excelRowIndex = item.ExcelRowIndex;
            string? lastname = item.Lastname;
            string? firstname = item.Firstname;
            int? Id = item.Id;
            string? department = item.Department;
            string? costcode = item.CostCode;
            string? mobile = item.Phone;
            string? personalMobile = item.PersonalMobile;
            int? sAPID = item.SAPID;
            string? nRN = item.NRN;
            string? email = item.Email;

            var returnData = new ValidateBulkEmployeeRow();

            if (columns.Contains("Firstname") && !string.IsNullOrWhiteSpace(nRN))
            {
                if (string.IsNullOrWhiteSpace(firstname))
                {
                    errormessages.Add("Please provide a non-empty value for Firstname.");
                }

            }
            else {
                errormessages.Add("Please provide a non-empty value for Firstname.");
            }

            if (columns.Contains("Lastname") && !string.IsNullOrWhiteSpace(nRN))
            {
                if (string.IsNullOrWhiteSpace(lastname))
                {
                    errormessages.Add("Please provide a non-empty value for Lastname.");
                }
            }
            else {
                errormessages.Add("Please provide a non-empty value for Lastname.");
            }

            if (columns.Contains("Department") && !string.IsNullOrWhiteSpace(department))
            {
                if (string.IsNullOrWhiteSpace(department))
                {
                    errormessages.Add("Please provide a non-empty value for Department.");
                }
            }
            else {
                errormessages.Add("Please provide a non-empty value for Department.");
            }


            if (columns.Contains("SAPID") && sAPID.HasValue)
            {
                if (!sAPID.HasValue)
                {
                    errormessages.Add("Please provide a non-empty value for SAPID.");

                }
                else if (activeEmployees.Any(x => x.SAPID == sAPID))
                {
                    errormessages.Add("SAPID is duplicated.");
                }
            }
            else {
                errormessages.Add("Please provide a non-empty value for SAPID.");
            }


            // Validate optional fields if present in columns
            if (columns.Contains("NRN") && !string.IsNullOrWhiteSpace(nRN))
            {
                string pattern = @"^[АБВГДЕЁЖЗИӨҮЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыөүьэюя]{2}";

                if (activeEmployees.Any(x => x.NRN == nRN))
                {
                    errormessages.Add("NRN is duplicated.");
                }
                if (nRN.Length != 10)
                {
                    errormessages.Add("NRN is not the correct length.");
                }
                if (!Regex.IsMatch(nRN, pattern))
                {
                    errormessages.Add("NRN first two characters are not Cyrillic.");
                }
            }

            if (columns.Contains("PersonalMobile") && !string.IsNullOrWhiteSpace(personalMobile))
            {
                if (personalMobile.Length > 12 || personalMobile.Length == 10 || personalMobile.Length == 9 || personalMobile.Length < 8)
                {
                    errormessages.Add("Personal Mobile is not the correct length.");
                }
                else
                {
                    if (personalMobile.Length == 12 && personalMobile.StartsWith("+976"))
                    {
                        personalMobile = personalMobile.Substring(4);
                    }
                    if (personalMobile.Length == 11 && personalMobile.StartsWith("976"))
                    {
                        personalMobile = personalMobile.Substring(3);
                    }
                    if (personalMobile.Length != 8)
                    {
                        errormessages.Add("Personal Mobile is not the correct length.");
                    }
                    else if (activeEmployees.Any(x => x.PersonalMobile == personalMobile))
                    {
                        errormessages.Add("Personal Mobile is duplicated.");
                    }
                }
            }

            if (columns.Contains("Email") && !string.IsNullOrWhiteSpace(email))
            {
                string emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

                if (!Regex.IsMatch(email, emailPattern))
                {
                    errormessages.Add("Invalid email address.");
                }
            }


            if (columns.Contains("Phone") && !string.IsNullOrWhiteSpace(mobile) && activeEmployees.Any(x => x.Mobile == mobile))
            {
                errormessages.Add("Phone is duplicated.");
            }

            // Set the validation result
            if (errormessages.Count == 0)
            {
                returnData.validationStatus = true;
            }
            else
            {
                returnData.validationStatus = false;
                returnData.ErrorMessage = errormessages;
                returnData.Fullname = $"{firstname} {lastname}";
                returnData.SAPID = sAPID;
                returnData.PersonId = Id;
            }

            return returnData;
        }




        private ValidateBulkEmployeeRow ValidateUpdateEmployee(BulkUploadEmployee item, List<Employee> activeEmployees, ExcelWorksheet worksheet)
        {
            var errormessages = new List<string>();
            var columns = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                .Select(c => c.Text.Trim())
                .ToList();

            int? Id = item.Id;
            if (!columns.Contains("Id") && !Id.HasValue)
            {
                errormessages.Add("Please provide a non-empty value for ID.");
            }

            string? lastname = item.Lastname;
            string? firstname = item.Firstname;
            string? mobile = item.Phone;
            int? sAPID = item.SAPID;
            string? nRN = item.NRN;
            string? personalMobile = item.PersonalMobile;
            string? department = item.Department;
            string? costcode = item.CostCode;
            string? email = item.Email;


            if (columns.Contains("Firstname"))
            {
                if (string.IsNullOrWhiteSpace(firstname))
                {
                    errormessages.Add("Please provide a non-empty value for Firstname.");
                }
                else if (firstname.Length > 50)
                {
                    errormessages.Add("Firstname should not exceed 50 characters.");
                }
            }

            if (columns.Contains("CostCode"))
            {
                if (string.IsNullOrWhiteSpace(costcode))
                {
                    errormessages.Add("Please provide a non-empty value for CostCode.");
                }
            }

            if (columns.Contains("Department"))
            {
                if (string.IsNullOrWhiteSpace(department))
                {
                    errormessages.Add("Please provide a non-empty value for Department.");
                }
            }

            if (columns.Contains("Lastname") && string.IsNullOrWhiteSpace(lastname))
            {
                errormessages.Add("Please provide a non-empty value for Lastname.");
            }

            if (columns.Contains("SAPID"))
            {
                if (sAPID.HasValue && Id.HasValue)
                {
                    if (activeEmployees.Any(x => x.SAPID == sAPID && x.Id != Id))
                    {
                        errormessages.Add("SAPID is duplicated.");
                    }
                }
                else if (sAPID.HasValue && !Id.HasValue)
                {
                    if (activeEmployees.Any(x => x.SAPID == sAPID))
                    {
                        errormessages.Add("SAPID is duplicated.");
                    }
                }
            }

            if (columns.Contains("Email") && !string.IsNullOrWhiteSpace(email))
            {
                string emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

                if (!Regex.IsMatch(email, emailPattern))
                {
                    errormessages.Add("Invalid email address.");
                }
            }

            if (columns.Contains("NRN"))
            {
                if (!string.IsNullOrWhiteSpace(nRN))
                {
                    if (activeEmployees.Any(x => x.NRN == nRN && (!Id.HasValue || x.Id != Id)))
                    {
                        errormessages.Add("NRN is duplicated.");
                    }

                    string pattern = @"^[АБВГДЕЁЖЗИӨҮЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыөүьэюя]{2}";
                    if (nRN.Length != 10)
                    {
                        errormessages.Add("NRN is not the correct length.");
                    }
                    if (!Regex.IsMatch(nRN, pattern))
                    {
                        errormessages.Add("NRN first two characters must be Cyrillic.");
                    }
                }
            }

            if (columns.Contains("Phone"))
            {
                if (!string.IsNullOrWhiteSpace(mobile))
                {
                    if (activeEmployees.Any(x => x.Mobile == mobile && (!Id.HasValue || x.Id != Id)))
                    {
                        errormessages.Add("Phone is duplicated.");
                    }
                    if (mobile.Length > 13)
                    {
                        errormessages.Add("Phone is not the correct length.");
                    }
                }
            }

            if (columns.Contains("PersonalMobile"))
            {
                if (!string.IsNullOrWhiteSpace(personalMobile))
                {
                    if (personalMobile.Length > 12 || personalMobile.Length == 10 || personalMobile.Length == 9 || personalMobile.Length < 8)
                    {
                        errormessages.Add("Personal Mobile is not the correct length.");
                    }
                    else
                    {
                        if (personalMobile.Length == 12 && personalMobile.StartsWith("+976"))
                        {
                            personalMobile = personalMobile.Substring(4);
                        }
                        if (personalMobile.Length == 11 && personalMobile.StartsWith("976"))
                        {
                            personalMobile = personalMobile.Substring(3);
                        }
                    }

                    if (activeEmployees.Any(x => x.PersonalMobile == personalMobile && (!Id.HasValue || x.Id != Id)))
                    {
                        errormessages.Add("Personal Mobile is duplicated.");
                    }
                }
            }

            var returnData = new ValidateBulkEmployeeRow
            {
                validationStatus = errormessages.Count == 0,
                ErrorMessage = errormessages,
                Fullname = $"{firstname} {lastname}",
                PersonId = Id,
                SAPID = sAPID
            };

            return returnData;
        }




        #region BulkUploadPreview
        public async Task<BulkUploadPreviewEmployeeResponse> BulkRequestUploadPreview(BulkUploadPreviewEmployeeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var returnData = new BulkUploadPreviewEmployeeResponse();
                var returnDataFailedRows = new List<EmployeeBulkFailedRow>();
                int AddedRows = 0;
                int UpdatedRows = 0;
                int NoneRows = 0;
                Stopwatch stopwatch = new Stopwatch();

                // Begin timing
                stopwatch.Start();

                Console.WriteLine("VALIDATION STARTED : {0}", stopwatch.Elapsed);

                using (var stream = new MemoryStream())
                {
                    request.BulkEmployeeFile.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet

                        var columns = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                        .Select(c => c.Text.Trim())
                        .ToList();

                        // Check for mandatory columns
                        if (!columns.Contains("Id") || !columns.Contains("Mode"))
                        {
                            throw new BadRequestException("The Excel file is missing mandatory columns 'Id' and 'Mode'.");
                        }

                        var extracIds = new List<string>();

                        var activeEmployees = await Context.Employee.Where(x => x.Active == 1).ToListAsync();

                        var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadEmployee>(package.Workbook.Worksheets[0], extracIds);

                        NoneRows = BulkUploadCodes.Where(x => x.Mode == "NONE").Count();

                        foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                        {
                            if (item.Mode == "ADD")
                            {
                                var validation = ValidateAddEmployee(item, activeEmployees, worksheet);
                                if (validation.validationStatus)
                                {
                                    AddedRows++;
                                }
                                else
                                {
                                    returnDataFailedRows.Add(new EmployeeBulkFailedRow
                                    {
                                        Fullname = validation.Fullname,
                                        Error = validation.ErrorMessage,
                                        ExcelRowIndex = item.ExcelRowIndex,
                                        SAPID = validation.SAPID,
                                        PersonId = validation.PersonId
                                    });
                                }

                            }
                            if (item.Mode == "UPDATE")
                            {
                                var validation = ValidateUpdateEmployee(item, activeEmployees, worksheet);
                                if (validation.validationStatus)
                                {
                                    UpdatedRows++;
                                }
                                else
                                {
                                    returnDataFailedRows.Add(new EmployeeBulkFailedRow
                                    {
                                        PersonId = validation.PersonId,
                                        SAPID = validation.SAPID,
                                        Fullname = validation.Fullname,
                                        Error = validation.ErrorMessage,
                                        ExcelRowIndex = item.ExcelRowIndex,
                                    });
                                }
                            }
                        }
                    }
                }

                returnData.AddRow = AddedRows;
                returnData.UpdateRow = UpdatedRows;
                returnData.NoneRow = NoneRows;
                returnData.FailedRows = returnDataFailedRows;

                stopwatch.Stop();
                Console.WriteLine("VALIDATION EXECUTED : {0}", stopwatch.Elapsed);

                return returnData;
            }
            catch (Exception ex) {
                throw new BadRequestException("The structure of the Excel file appears to be incorrect. Could you please provide the updated/corrected file?");
            }
        }

        #endregion



        #region MyRegion
        public async Task<List<int>> BulkRequestUpload(BulkUploadEmployeeRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<int>();
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var stream = new MemoryStream())
                {
                    request.BulkEmployeeFile.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    { 
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is on the first worksheet

                        var columns = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                        .Select(c => c.Text.Trim())
                        .ToList();

                        // Check for mandatory columns
                        if (!columns.Contains("Id") || !columns.Contains("Mode"))
                        {
                            throw new BadRequestException("The Excel file is missing mandatory columns 'Id' and 'Mode'.");
                        }



                        // Preload necessary data
                        var nationalityData = await LoadDataAsync<Nationality>(Context.Nationality);
                        var employerData = await LoadDataAsync<Employer>(Context.Employer);
                        var stateData = await LoadDataAsync<State>(Context.State);
                        var costcodeData = await Context.CostCodes.AsNoTracking()
                            .Select(x => new CostCodeData { Id = x.Id, Descr = $"{x.Number} {x.Description}" })
                            .ToListAsync();
                        var departmentData = await LoadDataAsync<Department>(Context.Department);
                        var positionData = await LoadDataAsync<Position>(Context.Position);
                        var rosterData = await LoadDataAsync<Roster>(Context.Roster);
                        var locationData = await LoadDataAsync<Location>(Context.Location);
                        var peopletypeData = await LoadDataAsync<PeopleType>(Context.PeopleType);
                        var transportData = await LoadDataAsync<FlightGroupMaster>(Context.FlightGroupMaster);
                        var groupData = await Context.GroupMaster.AsNoTracking()
                            .Where(x => x.ShowOnProfile == 1)
                            .ToListAsync();
                        var groupDetailData = await LoadDataAsync<GroupDetail>(Context.GroupDetail);

                     //   var BulkEmployeeGroupData = _bulkImportExcelService.GetGroupList<BulkUploadEmployeeGroup>(worksheet, groupData, groupDetailData);
                        var activeEmployees = await Context.Employee.Where(x => x.Active == 1).ToListAsync();
                        var BulkUploadCodes = _bulkImportExcelService.GetList<BulkUploadEmployee>(worksheet, new List<string>());

                        var BulkEmployeeGroupData = _bulkImportExcelService.GetGroupList<BulkUploadEmployeeGroup>(package.Workbook.Worksheets[0], groupData, groupDetailData);


                        foreach (var item in BulkUploadCodes.Where(x => x.Mode != "NONE"))
                        {
                            if (item.Mode == "ADD")
                            {
                                var validation = ValidateAddEmployee(item, activeEmployees, worksheet);
                                if (validation.validationStatus)
                                {
                                    var newData = CreateNewEmployee(item, nationalityData, employerData, stateData, costcodeData, departmentData, positionData, rosterData, locationData, peopletypeData, transportData);
                                    Context.Employee.Add(newData);
                                    await Context.SaveChangesAsync();
                                    returnData.Add(newData.Id);
                                }
                            }
                            else if (item.Mode == "UPDATE")
                            {
                                var validation = ValidateUpdateEmployee(item, activeEmployees, worksheet);
                                if (validation.validationStatus)
                                {
                                    var currentEmployee = activeEmployees.FirstOrDefault(x => x.Id == item.Id.Value);
                                    if (currentEmployee != null)
                                    {
                                      await  UpdateEmployee(currentEmployee, item, nationalityData, employerData, stateData, costcodeData, departmentData, positionData, rosterData, locationData, peopletypeData, transportData, worksheet, cancellationToken);
                                        Context.Employee.Update(currentEmployee);
  
                                        returnData.Add(currentEmployee.Id);
                                    }
                                }
                            }
                        }

                        await SaveGroupData(BulkEmployeeGroupData);
                //        await Context.SaveChangesAsync();

                    }
                }

                return returnData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new BadRequestException("The structure of the Excel file appears to be incorrect. Could you please provide the updated/corrected file?");
            }
        }

        // Helper methods
        private async Task<List<T>> LoadDataAsync<T>(DbSet<T> dbSet) where T : class
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }

        private Employee CreateNewEmployee(BulkUploadEmployee item, List<Nationality> nationalityData, List<Employer> employerData, List<State> stateData, List<CostCodeData> costcodeData, List<Department> departmentData, List<Position> positionData, List<Roster> rosterData, List<Location> locationData, List<PeopleType> peopletypeData, List<FlightGroupMaster> transportData)
        {
            // Process gender
            int genderData = ProcessGender(item.Gender);

            // Process personal mobile
            string? personalMobile = ProcessPersonalMobile(item.PersonalMobile);

            int? newdepartmentId = null;
            if (!string.IsNullOrWhiteSpace(item.Department))
            {
                if (item.Department.IndexOf(".") > -1)
                {

                    int? departmentId = Convert.ToInt32(item.Department?.Split(".")[0]);

                   newdepartmentId = departmentData.FirstOrDefault(x => x.Id == departmentId)?.Id;
                }
            }

            // Create new employee object
            var newData = new Employee
            {
                Firstname = item.Firstname,
                Lastname = item.Lastname,
                MLastname = Transliterator.LatinToCyrillic(item.Lastname),
                MFirstname = Transliterator.LatinToCyrillic(item.Firstname),
                Mobile = item.Phone,
                PersonalMobile = personalMobile,
                Dob = item.Dob,
                Gender = genderData,
                SAPID = item.SAPID,
                PassportNumber = item.PassportNumber,
                PassportName = item.NameOnPassport,
                PassportExpiry = ParseDate(item.PassportExpiry),
                NRN = item.NRN,
                Email = item.Email,
                HotelCheck = item.HotelCheck,
                NationalityId = nationalityData.FirstOrDefault(x => x.Description == item.Nationality)?.Id,
                EmployerId = employerData.FirstOrDefault(x => x.Description == item.Employer)?.Id,
                StateId = stateData.FirstOrDefault(x => x.Description == item.Aimag)?.Id,
                ContractNumber = item.ContractNumber,
                CostCodeId = costcodeData.FirstOrDefault(x => x.Descr == item.CostCode)?.Id,
                DepartmentId = newdepartmentId,
                PositionId = positionData.FirstOrDefault(x => x.Description == item.Position)?.Id,
                RosterId = rosterData.FirstOrDefault(x => x.Description == item.Roster)?.Id,
                LocationId = locationData.FirstOrDefault(x => x.Description == item.Location)?.Id,
                PeopleTypeId = peopletypeData.FirstOrDefault(x => x.Description == item.ResourceType)?.Id,
                PickUpAddress = item.PickUpAddress,
                FlightGroupMasterId = transportData.FirstOrDefault(x => x.Description == item.TransportGroup)?.Id,
                EmergencyContactName = item.EmergencyContactName,
                EmergencyContactMobile = item.EmergencyContactMobile,
                DateCreated = DateTime.Now,
                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                Active = 1,
                FrequentFlyer = item.FrequentFlyer
            };

            return newData;
        }

        private async Task UpdateEmployee(Employee currentEmployee, BulkUploadEmployee item, List<Nationality> nationalityData, List<Employer> employerData, List<State> stateData, List<CostCodeData> costcodeData, List<Department> departmentData, List<Position> positionData, List<Roster> rosterData, List<Location> locationData, List<PeopleType> peopletypeData, List<FlightGroupMaster> transportData, ExcelWorksheet worksheet, CancellationToken cancellationToken)
        {
            var columns = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
                .Select(c => c.Text.Trim())
                .ToList();

            if (columns.Contains("Firstname"))
            {
                currentEmployee.Firstname = item.Firstname;
                currentEmployee.MFirstname = Transliterator.LatinToCyrillic(item.Firstname);
            }
            if (columns.Contains("Lastname"))
            {
                currentEmployee.Lastname = item.Lastname;
                currentEmployee.MLastname = Transliterator.LatinToCyrillic(item.Lastname);
            }
            if (columns.Contains("Email"))
            {
                currentEmployee.Email = item.Email;
            }

            if (columns.Contains("Phone"))
            {
                currentEmployee.Mobile = item.Phone;
            }

            if (columns.Contains("PersonalMobile"))
            {
                currentEmployee.PersonalMobile = ProcessPersonalMobile(item.PersonalMobile, currentEmployee.PersonalMobile);
            }

            if (columns.Contains("Dob"))
            {
                currentEmployee.Dob = item.Dob ?? currentEmployee.Dob;
            }

            if (columns.Contains("Gender"))
            {
                currentEmployee.Gender = ProcessGender(item.Gender, currentEmployee.Gender);
            }

            if (columns.Contains("SAPID"))
            {
                currentEmployee.SAPID = item.SAPID;
            }

            if (columns.Contains("PassportNumber"))
            {
                currentEmployee.PassportNumber = item.PassportNumber;
            }

            if (columns.Contains("NameOnPassport"))
            {
                currentEmployee.PassportName = item.NameOnPassport;
            }

            if (columns.Contains("PassportExpiry"))
            {
                currentEmployee.PassportExpiry = ParseDate(item.PassportExpiry, currentEmployee.PassportExpiry);
            }

            if (columns.Contains("NRN"))
            {
                currentEmployee.NRN = item.NRN;
            }

            if (columns.Contains("HotelCheck"))
            {
                currentEmployee.HotelCheck = item.HotelCheck;
            }

            if (columns.Contains("Nationality"))
            {
                currentEmployee.NationalityId = nationalityData.FirstOrDefault(x => x.Description == item.Nationality)?.Id ?? currentEmployee.NationalityId;
            }

            if (columns.Contains("Employer"))
            {
                //   currentEmployee.EmployerId = employerData.FirstOrDefault(x => x.Description == item.Employer)?.Id ?? currentEmployee.EmployerId;

                var employerDataItem = employerData.FirstOrDefault(x => x.Description == item.Employer);
                if (employerDataItem != null)
                {
                    await ChangeEmployeeTransportStatusData(currentEmployee.Id,  employerDataItem.Id, "EmployerId", cancellationToken);

                }

            }

            if (columns.Contains("Aimag"))
            {
                currentEmployee.StateId = stateData.FirstOrDefault(x => x.Description == item.Aimag)?.Id ?? currentEmployee.StateId;
            }

            if (columns.Contains("ContractNumber"))
            {
                currentEmployee.ContractNumber = item.ContractNumber;
            }

            if (columns.Contains("CostCode"))
            {
                //currentEmployee.CostCodeId = costcodeData.FirstOrDefault(x => x.Descr == item.CostCode)?.Id ?? currentEmployee.CostCodeId;

            //    currentEmployee.EmployerId = employerData.FirstOrDefault(x => x.Description == item.Employer)?.Id ?? currentEmployee.EmployerId;

                var costcodeDataItem = costcodeData.FirstOrDefault(x => x.Descr == item.CostCode);
                if (costcodeDataItem != null)
                {
                    currentEmployee.CostCodeId = costcodeDataItem.Id;
                    await ChangeEmployeeTransportStatusData(currentEmployee.Id, costcodeDataItem.Id, "CostCodeId", cancellationToken);
                    //    await ChangeEmployeeCostCodeData(currentEmployee.Id, costcodeDataItem.Id, cancellationToken);

                }
                //else
                //{
                //    currentEmployee.CostCodeId = currentEmployee.CostCodeId;
                //}

            }

            if (columns.Contains("Department"))
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(item.Department))
                    {
                        if (item.Department.IndexOf(".") > -1)
                        {

                            int? departmentId = Convert.ToInt32(item.Department?.Split(".")[0]);


                            var departmentDataItem = departmentData.FirstOrDefault(x => x.Id == departmentId);
                            if (departmentDataItem != null)
                            {
                                currentEmployee.DepartmentId = departmentDataItem.Id;

                                await ChangeEmployeeTransportStatusData(currentEmployee.Id, departmentDataItem.Id, "DepartmentId", cancellationToken);


                                //         await ChangeEmployeeDepartmentData(currentEmployee.Id, departmentDataItem.Id, cancellationToken);
                            }
                            //else
                            //{
                            //    currentEmployee.DepartmentId = currentEmployee.DepartmentId;
                            //}

 
                        }
                    }
                }
                catch (Exception ex) { 
                
                }
                
            }

            if (columns.Contains("Position"))
            {

                var positionDataItem = positionData.FirstOrDefault(x => x.Description == item.Position);
                if (positionDataItem != null)
                {
                    currentEmployee.PositionId = positionDataItem.Id;
                //   await ChangeEmployeePositionData(currentEmployee.Id, positionDataItem.Id, cancellationToken);

                    await ChangeEmployeeTransportStatusData(currentEmployee.Id, positionDataItem.Id, "PositionId", cancellationToken);


                }
                //else {
                //    currentEmployee.PositionId = currentEmployee.PositionId;
                //}




            }

            if (columns.Contains("Roster"))
            {
                currentEmployee.RosterId = rosterData.FirstOrDefault(x => x.Description == item.Roster)?.Id ?? currentEmployee.RosterId;
            }

            if (columns.Contains("Location"))
            {
                currentEmployee.LocationId = locationData.FirstOrDefault(x => x.Description == item.Location)?.Id ?? currentEmployee.LocationId;
            }

            if (columns.Contains("ResourceType"))
            {
                currentEmployee.PeopleTypeId = peopletypeData.FirstOrDefault(x => x.Description == item.ResourceType)?.Id ?? currentEmployee.PeopleTypeId;
            }

            if (columns.Contains("PickUpAddress"))
            {
                currentEmployee.PickUpAddress = item.PickUpAddress;
            }

            if (columns.Contains("TransportGroup"))
            {
                currentEmployee.FlightGroupMasterId = transportData.FirstOrDefault(x => x.Description == item.TransportGroup)?.Id ?? currentEmployee.FlightGroupMasterId;
            }

            if (columns.Contains("EmergencyContactName"))
            {
                currentEmployee.EmergencyContactName = item.EmergencyContactName;
            }

            if (columns.Contains("EmergencyContactMobile"))
            {
                currentEmployee.EmergencyContactMobile = item.EmergencyContactMobile;
            }

            currentEmployee.DateUpdated = DateTime.Now;
            currentEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

            if (columns.Contains("CommenceDate"))
            {
                currentEmployee.CommenceDate = item.CommenceDate;
            }

            if (columns.Contains("CompletionDate"))
            {
                currentEmployee.CompletionDate = item.CompletionDate;
            }

            if (columns.Contains("FrequentFlyer"))
            {
                currentEmployee.FrequentFlyer = item.FrequentFlyer;
            }
        }

        private int ProcessGender(string gender, int? currentGender = null)
        {
            return gender switch
            {
                "Female" => 0,
                "Male" => 1,
                _ => currentGender ?? 0,
            };
        }

        private string? ProcessPersonalMobile(string? personalMobile, string? currentMobile = null)
        {
            if (string.IsNullOrWhiteSpace(personalMobile))
                return currentMobile;

            if (personalMobile.Length > 12 || personalMobile.Length == 10 || personalMobile.Length == 9 || personalMobile.Length < 8)
                return currentMobile;

            if (personalMobile.Length == 12 && personalMobile.StartsWith("+976"))
                return personalMobile.Substring(4);

            if (personalMobile.Length == 11 && personalMobile.StartsWith("976"))
                return personalMobile.Substring(3);

            return personalMobile.Length == 8 ? personalMobile : currentMobile;
        }

        private DateTime? ParseDate(string? date, DateTime? fallbackDate = null)
        {
            return DateTime.TryParse(date, out var result) ? result : fallbackDate;
        }

        #endregion



        #region GetDepartmentData

        private async Task<List<DeparmentData>> GetDeparmentData(CancellationToken cancellationToken)
        {

            string query = @$"SELECT Id, OriginalName, Name FROM ReportDeparmentData";

            return await GetRawQueryData<DeparmentData>(query, cancellationToken);
        }


        private async Task<List<T>> GetRawQueryData<T>(string query, CancellationToken cancellationToken)
        {
            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        await sqlConnection.OpenAsync(cancellationToken);
                    }

                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandTimeout = 300;

                        using (var result = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            var resultList = new List<T>();
                            while (await result.ReadAsync(cancellationToken))
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

        #endregion

    }



    public sealed class ValidateBulkEmployeeRow
    {
        public bool validationStatus { get; set; }

        public string? Fullname { get; set; }

        public int? PersonId { get; set; }
        public int? SAPID { get; set; }
        public List<string> ErrorMessage { get; set; }
    }

    public  class BulkUploadEmployee
    {
        public int ExcelRowIndex { get; set; }

        public int? Id { get; set; }

        public string? Mode { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public string? Phone { get; set; }
        public string? PersonalMobile { get; set; }

        public DateTime? Dob { get; set; }
        public string Gender { get; set; }
        public int? SAPID { get; set; }
        public string? PassportNumber { get; set; }
        public string? NameOnPassport { get; set; }
        public string? PassportExpiry { get; set; }
        public string? NRN { get; set; }

        public string? Email { get; set; }



        public int? HotelCheck { get; set; }
        public string? Nationality { get; set; }
        public string? Employer { get; set; }
        public string? Aimag { get; set; }
        public string? ContractNumber { get; set; }
       
        public string? CostCode { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Roster { get; set; }
        public string? Location { get; set; }
        public string? ResourceType { get; set; }

        public string? PickUpAddress { get; set; }
        public string? TransportGroup { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactMobile { get; set; }

        public DateTime? CompletionDate { get; set; }
        public DateTime? CommenceDate { get; set; }

        public int? FrequentFlyer { get; set; }
        

    }

    public  class BulkUploadEmployeeGroup
    {
        public int ExcelRowIndex { get; set; }

        public string? Mode { get; set; }
        public int? Id { get; set; }
        public int? MasterId { get; set; }
        public int? DetailId { get; set; }

     

    }

    public sealed record CostCodeData
    {
        public int Id { get; set; }

        public string? Descr { get; set; }



    }

    public sealed record DeparmentData
    {
        public int Id { get; set; }

        public string? OriginalName { get; set; }


        public string? Name { get; set; }



    }






}
