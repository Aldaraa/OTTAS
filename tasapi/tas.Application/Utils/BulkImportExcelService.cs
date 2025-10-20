 using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Domain.Entities;

namespace tas.Application.Utils
{
    public  class BulkImportExcelService
    {
        public BulkImportExcelService() 
        {
        
        }

        public List<T> GetList<T>(ExcelWorksheet sheet, List<string> ExtractIdValues )
        {

            try
            {
                List<T> list = new List<T>();
                var columnInfo = Enumerable.Range(1, sheet.Dimension.Columns).Select(n =>
                    new { Index = n, ColumnName = sheet.Cells[1, n].Value.ToString() }
                ).ToList();

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        var propType = prop.PropertyType;
                        var column = columnInfo.SingleOrDefault(c => c.ColumnName == prop.Name);
                        if (prop.Name == "ExcelRowIndex")
                        {
                            prop.SetValue(obj, row);
                        }
                        if (column != null)
                        {
                            if (ExtractIdValues.AsEnumerable().FirstOrDefault(c => c == prop.Name) != null)
                            {
                                var val = sheet.Cells[row, column.Index].Value;
                                if (val != null)
                                {

                                    if (val.ToString().Contains("."))
                                    {
                                        var newValue = val.ToString().Split(".")[0];
                                        prop.SetValue(obj, ChangeType(newValue, propType));
                                    }
                                    else
                                    {
                                        prop.SetValue(obj, ChangeType(val, propType));
                                    }

                                }
                                else
                                {
                                    prop.SetValue(obj, null);
                                }
                            }
                            else
                            {
                                var val = sheet.Cells[row, column.Index].Value;
                                prop.SetValue(obj, ChangeType(val, propType));
                            }
                        }

                    }


                    list.Add(obj);

                }

                return list;
            }
            catch (Exception ex)
            {
                throw new BadRequestException("The structure of the Excel file appears to be incorrect. Could you please provide the updated/corrected file?");
            }
        
        }


        public List<T> GetGroupList<T>(ExcelWorksheet sheet, List<GroupMaster> groups, List<GroupDetail> details)
        {
            try
            {
                List<T> list = new List<T>();
                var columnInfo = Enumerable.Range(1, sheet.Dimension.Columns).Select(n =>
                    new { Index = n, ColumnName = sheet.Cells[1, n].Value.ToString() }
                ).ToList();

                for (int row = 2; row <= sheet.Dimension.Rows; row++)
                {
                    foreach (var item in columnInfo)
                    {
                        T obj = Activator.CreateInstance<T>();
                        PropertyInfo empIdProp = typeof(T).GetProperty("Id");
                        PropertyInfo excelRowIndexProp = typeof(T).GetProperty("ExcelRowIndex");
                        PropertyInfo masterIdProp = typeof(T).GetProperty("MasterId");
                        PropertyInfo detailIdProp = typeof(T).GetProperty("DetailId");
                        PropertyInfo detailModeProp = typeof(T).GetProperty("Mode");

                        if (empIdProp != null)
                        {
                            empIdProp.SetValue(obj, Convert.ToInt32(sheet.Cells[row, 2].Value));

                        }

                        if (excelRowIndexProp != null)
                        {
                            excelRowIndexProp.SetValue(obj, row);

                        }


                        if (detailModeProp != null)
                        {
                            detailModeProp.SetValue(obj, Convert.ToString(sheet.Cells[row, 1].Value));

                        }

                        var groupData = groups.FirstOrDefault(x => x.Description == item.ColumnName);
                        if (groupData != null && masterIdProp != null)
                        {
                            var val = sheet.Cells[row, item.Index].Value;
                            var detailData = details.FirstOrDefault(x => x.Description == Convert.ToString(val));

                            masterIdProp.SetValue(obj, groupData.Id);
                            if (detailIdProp != null)
                            {
                                detailIdProp.SetValue(obj, detailData?.Id);
                            }
                            list.Add(obj);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new BadRequestException("The structure of the Excel file appears to be incorrect. Could you please provide the updated/corrected file?");
            }
        }


        //public List<dynamic> GetGroupList<T>(ExcelWorksheet sheet, List<GroupMaster> groups, List<GroupDetail> details)
        //{

        //    try
        //    {
        //        List<dynamic> list = new List<dynamic>();
        //        var columnInfo = Enumerable.Range(1, sheet.Dimension.Columns).Select(n =>
        //            new { Index = n, ColumnName = sheet.Cells[1, n].Value.ToString() }
        //        ).ToList();

        //        for (int row = 2; row <= sheet.Dimension.Rows; row++)
        //        {
        //            dynamic obj = Activator.CreateInstance<dynamic>();
        //            foreach (var prop in typeof(T).GetProperties())
        //            {
        //                var propType = prop.PropertyType;
        //                var column = columnInfo.SingleOrDefault(c => c.ColumnName == prop.Name);
        //                if (prop.Name == "Id")
        //                {
        //                    if (column != null)
        //                    {
        //                        var val = sheet.Cells[row, column.Index].Value;
        //                        if (val != null)
        //                        {
        //                            obj.EmpId = Convert.ToInt32(val);
        //                        }
        //                        else {
        //                            obj.EmpId = Convert.ToInt32(val);
        //                        }

        //                    }

        //                }
        //                if (column != null)
        //                {
        //                    var groupData = groups.SingleOrDefault(x => x.Description == prop.Name);
        //                    if(groupData != null)
        //                    {
        //                        var val = sheet.Cells[row, column.Index].Value;
        //                        if (val != null)
        //                        {
        //                            var detailData = details.SingleOrDefault(x => x.Description == Convert.ToString(val));
        //                            if (detailData != null)
        //                            {
        //                                obj.detailId = detailData.Id;
        //                                obj.masterId = groupData.Id;
        //                            }
        //                            else
        //                            {
        //                                obj.detailId = null;
        //                                obj.masterId = groupData.Id;
        //                            }

        //                        }
        //                        else { 
        //                                                                obj.detailId = null;
        //                                obj.masterId = groupData.Id;
        //                        }
        //                    }


        //                }

        //            }
        //            list.Add(obj);

        //        }

        //        return list;
        //    }
        //    catch (Exception)
        //    {
        //        throw new BadRequestException("The structure of the Excel file appears to be incorrect. Could you please provide the updated/corrected file?");
        //    }

        //}

        private  object ChangeType(object value, Type conversion)
        {

            try
            {
                var t = conversion;

                if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (value == null)
                    {
                        return null;
                    }

                    t = Nullable.GetUnderlyingType(t);
                }

                return Convert.ChangeType(value, t);
            }
            catch (Exception ex)
            {
                var aa = ex.Message;
                return null;
            }

        }
      
    }
}
