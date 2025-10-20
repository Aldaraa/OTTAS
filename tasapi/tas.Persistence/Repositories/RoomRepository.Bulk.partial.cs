using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.BulkDownloadRoom;
using tas.Application.Features.RoomFeature.BulkUploadPreviewRoom;
using tas.Application.Features.RoomFeature.BulkUploadRoom;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Domain.Enums;

namespace tas.Persistence.Repositories
{
    public partial class RoomRepository : BaseRepository<Room>, IRoomRepository
    {

        #region BulkRequest
        public async Task<BulkDownloadRoomResponse> BulkRequestDownload(BulkDownloadRoomRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetRoom = package.Workbook.Worksheets.Add("Rooms");
                var worksheetCamp = package.Workbook.Worksheets.Add("Camp");
                var worksheetRoomType = package.Workbook.Worksheets.Add("RoomType");
                worksheetRoom.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MainTabColor);
                worksheetCamp.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);
                worksheetRoomType.TabColor = ColorTranslator.FromHtml(BulkExcelColors.MasterTabColor);

                worksheetRoom.Workbook.CalcMode = ExcelCalcMode.Automatic;
                var worksheetCampChangeData = await ModfyCampSheet(worksheetCamp);
                worksheetCamp = worksheetCampChangeData.sheet;
                var worksheetRoomTypeChangeData = await ModfyRoomTypeSheet(worksheetRoomType);
                worksheetRoomType = worksheetRoomTypeChangeData.sheet;
                List<string> ActionModes = new List<string> { "NONE","UPDATE"};

                string ActionModesList = string.Join(",", ActionModes);

                worksheetRoom.Row(1).Style.Font.Bold = true;
                var headerCells = worksheetRoom.Cells["A1:F1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Font.Size = 13;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                
                headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
                headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
                var entities = Context.Room.Where(x => request.RoomIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.BedCount, x.Number, x.CampId, x.RoomTypeId, x.Private });



                worksheetRoom.Cells[1, 1].Value = "Mode";
                worksheetRoom.Cells[1, 2].Value = "Id";
                worksheetRoom.Cells[1, 3].Value = "Number";
                worksheetRoom.Cells[1, 4].Value = "Camp";
                worksheetRoom.Cells[1, 5].Value = "Private";
                worksheetRoom.Cells[1, 6].Value = "RoomType";

                var validation = worksheetRoom.DataValidations.AddListValidation($"A2:A{entities.Count() + 1}");
                validation.ShowErrorMessage = true;
                validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                validation.ErrorTitle = "An invalid value was entered";
                validation.Error = "Select a value from the list";

                var CampValidation = worksheetRoom.DataValidations.AddListValidation($"D2:D{entities.Count() + 1}");
                var RoomTypeValidation = worksheetRoom.DataValidations.AddListValidation($"F2:F{entities.Count() + 1}");
                CampValidation.Formula.ExcelFormula = $"Camp!$A$2:$A${worksheetCampChangeData.sheetDatas.Count() + 1}";
                RoomTypeValidation.Formula.ExcelFormula = $"RoomType!$A$2:$A${worksheetRoomTypeChangeData.sheetDatas.Count() + 1}";
                int row = 2;

                foreach (var item in ActionModes)
                {
                    validation.Formula.Values.Add(item);
                }
                foreach (var entity in entities)
                {

                    worksheetRoom.Cells[row, 1].Value = ActionModes[1];
                    worksheetRoom.Cells[row, 2].Value = entity.Id;
                    worksheetRoom.Cells[row, 3].Value = entity.Number;
                    worksheetRoom.Cells[row, 4].Value = worksheetCampChangeData.sheetDatas.FirstOrDefault(x=> x.Id == entity.CampId)?.Description;
                    worksheetRoom.Cells[row, 5].Value = $"{entity.Private}";
                    worksheetRoom.Cells[row, 6].Value = worksheetRoomTypeChangeData.sheetDatas.FirstOrDefault(x => x.Id == entity.RoomTypeId)?.Description;

                  
                    string cellRange = $"A{row}:F{row}";

                    var f = worksheetRoom.ConditionalFormatting.AddExpression(worksheetRoom.Cells[cellRange]);

                    f.Address = worksheetRoom.Cells[cellRange];
                    f.Style.Fill.BackgroundColor.Color = ColorTranslator.FromHtml(BulkExcelColors.ActionModeEditColor);
                    f.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    f.Formula = $"A{row}=\"UPDATE\"";


                    f = worksheetRoom.ConditionalFormatting.AddExpression(worksheetRoom.Cells[cellRange]);
                    f.Address = worksheetRoom.Cells[cellRange];


                    row++;
                }

                worksheetRoom.Cells["A1:F1"].AutoFilter = true;
                worksheetRoom.Cells.AutoFitColumns();
                package.Save();

                return new BulkDownloadRoomResponse
                {
                    ExcelFile = package.GetAsByteArray()
                };

            }


        }

        private async Task<ModfySheetReturnData> ModfyCampSheet(ExcelWorksheet sheet)
        {
            var Camps =await Context.Camp.Select(x=> new { x.Id, x.Description}).ToListAsync();
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

                returnData.Add(new SheetData { Id = entity.Id, Description = entity.Description });
                row++;
            }
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData
            {
                sheet = sheet,
                sheetDatas = returnData
            };
        }

        private async Task<ModfySheetReturnData> ModfyRoomTypeSheet(ExcelWorksheet sheet)
        {
            var RoomTypes = await Context.RoomType.Select(x => new { x.Id, x.Description }).ToListAsync();
            sheet.Row(1).Style.Font.Bold = true;
            var headerCells = sheet.Cells["A1"];
            headerCells.Style.Font.Bold = true;
            headerCells.Style.Font.Size = 13;
            headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundColor));
            headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml(BulkExcelColors.HeaderBackgroundTextColor));
            sheet.Cells[1, 1].Value = "Description";
            int row = 2;
            var returnData  = new List<SheetData>();

            foreach (var entity in RoomTypes)
            {
                sheet.Cells[row, 1].Value = $"{entity.Description}";

                returnData.Add(new SheetData { Id = entity.Id, Description = entity.Description });
                row++;
            }
            sheet.Cells.AutoFitColumns();
            return new ModfySheetReturnData {
                sheet = sheet,
                sheetDatas = returnData
            };
        }
        #endregion


        #region BulkUpload

        public async  Task BulkRequestUpload(BulkUploadRoomRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                request.BulkRoomFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var extracIds = new List<string>();
                    var BulkUploadRooms = _bulkImportExcelService.GetList<BulkUploadRoom>(package.Workbook.Worksheets[0], extracIds);

                    var allRoomTypes =await Context.RoomType.AsNoTracking().ToListAsync();
                    var allCamps = await Context.Camp.AsNoTracking().ToListAsync();
                    var allRooms =await Context.Room.AsNoTracking().ToListAsync();

                    foreach (var item in BulkUploadRooms.Where(x=> x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateRoom(item);
                            if (validation.validationStatus)
                            {
 
                                var currentRoom = allRooms.Where(x => x.Id == item.Id).FirstOrDefault();
                                var Id = item.Id;
                                var camp = item.Camp;
                                var roomtype = item.RoomType;
                                var Private = item.Private;
                                var Number = item.Number;
                                if (currentRoom != null)
                                {

                                    currentRoom.Active = 1;
                                    currentRoom.CampId = allCamps.Where(x => x.Description == camp).FirstOrDefault() != null ? allCamps.Where(x => x.Description == camp).FirstOrDefault().Id : currentRoom.CampId;
                                    currentRoom.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    currentRoom.DateUpdated = DateTime.Now;
                                    currentRoom.Number = Number;
                                    currentRoom.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    currentRoom.Private = (int)Private.Value;
                                    currentRoom.RoomTypeId = allRoomTypes.Where(x => x.Description == roomtype).FirstOrDefault() != null ? allRoomTypes.Where(x => x.Description == roomtype).FirstOrDefault()?.Id : currentRoom?.RoomTypeId;

                                    Context.Room.Update(currentRoom);

                                }
                            }
                        }
                    }
                }
            }

            await Task.CompletedTask;
        }




        private async Task<ValidateBulkRoomRow> ValidateUpdateRoom(BulkUploadRoom item)
        {
            var Id = item.Id;
            var camp = item.Camp;
            var roomtype = item.RoomType;
            var Private = item.Private;
            var Number = item.Number;
            var returnData = new ValidateBulkRoomRow();
            var errormessages = new List<string>();
            if (!Id.HasValue)
            {
                errormessages.Add("Id is an invalid value");
            }
            if (!Private.HasValue)
            {
                errormessages.Add("Private is an invalid value.");
            }
            if (Private.HasValue)
            {
                if (Private > 1)
                {
                    errormessages.Add("Private is only in 1 or 0");
                }
            }
            var CurrentRoom = Context.Room.Any(x => x.Id == Id);
            if (!CurrentRoom)
            {
                errormessages.Add("Room is not found");
            }
            var CurrentCamp =await Context.Camp.AnyAsync(x => x.Description == camp);
            if (!CurrentCamp)
            {
                errormessages.Add("Camp is an invalid value");
            }
            var CurrentRoomTypeId =await Context.RoomType.AnyAsync(x => x.Description == roomtype);
            if (!CurrentRoomTypeId)
            {
                errormessages.Add("RoomType is an invalid value");
            }

            if (string.IsNullOrWhiteSpace(Number))
            {
                errormessages.Add("RoomNumber  is required");
            }

            if (!string.IsNullOrWhiteSpace(Number))
            {
                var NumberCheck = await Context.Room.AnyAsync(x => x.Number == Number && x.Id != Id);
                if (NumberCheck)
                {
                    errormessages.Add("RoomNumber  is duplicated");
                }

            }


            if (errormessages.Count == 0)
            {
                returnData.validationStatus = true;
            }
            else
            {
                returnData.validationStatus = false;
                returnData.ErrorMessage = errormessages;
            }
            return returnData;
        }

        #endregion


        public async Task<BulkUploadPreviewRoomResponse> BulkRequestUploadPreview(BulkUploadPreviewRoomRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var returnData = new BulkUploadPreviewRoomResponse();
            var returnDataFailedRows = new List<RoomBulkFailedRow>();
            int AddedRows = 0;
            int UpdatedRows = 0;
            int DeletedRows = 0;
            int NoneRows = 0;
            using (var stream = new MemoryStream())
            {
                request.BulkRoomFile.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is on the first worksheet
                    var extracIds = new List<string>();
                    var BulkUploadRooms = _bulkImportExcelService.GetList<BulkUploadRoom>(package.Workbook.Worksheets[0], extracIds);
                    NoneRows = BulkUploadRooms.Where(x => x.Mode == "NONE").Count();

                    foreach (var item in BulkUploadRooms.Where(x => x.Mode != "NONE"))
                    {
                        if (item.Mode == "UPDATE")
                        {
                            var validation = await ValidateUpdateRoom(item);
                            if (validation.validationStatus)
                            {
                                UpdatedRows++;
                            }
                            else
                            {
                                returnDataFailedRows.Add(new RoomBulkFailedRow
                                {
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
            returnData.DeleteRow = DeletedRows;
            returnData.FailedRows = returnDataFailedRows;
            return returnData;
        }



    }



    public class ModfySheetReturnData
    { 
        public ExcelWorksheet sheet { get; set; }

        public List<SheetData> sheetDatas { get; set; }


    }

    public sealed class ValidateBulkRoomRow
    {
        public bool validationStatus {get; set;}

        public List<string> ErrorMessage { get; set; }
    }

    public class SheetData 
    {
        public int Id { get; set; }

        public string? Description { get; set; }
    }

    public sealed class BulkUploadRoom
    { 
        public int ExcelRowIndex { get; set; }
        public string? Mode { get; set; }
        public int? Id { get; set; }

        public string? Number { get; set; }


        public string? Camp { get; set; }

        public int? Private { get; set; }

        public string? RoomType { get; set; }

    }

}
