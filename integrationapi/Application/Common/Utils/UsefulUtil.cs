using Application.Common.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utils
{
    public static class UsefulUtil
    {
       public static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    {
                        newText.Append(' ');
                    }
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }


        //TAS_DYNAMIC_DATE date calculation

        public static DateTime CalculateReportParamDynamicDate(string fieldValue, string fieldName, int? days)
        {

            var returnDate = DateTime.Today;

            var modifiedDateType = fieldValue.Replace("{", "").Replace("}", "");

            if (Enum.TryParse<ReportDateTypes>(modifiedDateType, true, out ReportDateTypes dateType))
            {
                returnDate = DateFunctions.GetDate(dateType);
                return returnDate.AddDays(days.HasValue ? days.Value : 0);
            }
            else
            {
                DateTime currentDate;

                if (DateTime.TryParseExact(fieldValue, "yyyy-MM-dd",
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                out currentDate))
                {

                    return currentDate.AddDays(days.HasValue ? days.Value : 0);
                }
                else
                {
                    return returnDate;

                }


            }


            //    var returnDate = DateTime.Today;
            //    if (fieldName == "EndDate")
            //    {
            //        returnDate = DateTime.Today.AddMonths(1);
            //    }
            //    else if (fieldName == "CurrentDate")
            //    {
            //        returnDate = DateTime.Today;
            //    }

            //    if (fieldValue.StartsWith("[") && fieldValue.EndsWith("]"))
            //    {
            //        string expression = fieldValue.Substring(1, fieldValue.Length - 2);

            //        // Split the expression into parts (e.g., "TODAY + 1" => ["TODAY", "+", "1"])
            //        string[] parts = expression.Split(' ');

            //        if (parts.Length == 3)
            //        {
            //            DateTime baseDate;
            //            if (parts[0] == "TODAY")
            //            {
            //                baseDate = DateTime.Today;
            //                int offset;
            //                if (int.TryParse(parts[2], out offset))
            //                {
            //                    return baseDate.AddDays(parts[1] == "+" ? offset : -offset);
            //                }
            //                else
            //                {
            //                    return baseDate;
            //                }
            //            }
            //            if (parts[0] == "TOMORROW")
            //            {
            //                baseDate = DateTime.Today.AddDays(1);
            //                int offset;
            //                if (int.TryParse(parts[2], out offset))
            //                {
            //                    return baseDate.AddDays(parts[1] == "+" ? offset : -offset);
            //                }
            //                else
            //                {
            //                    return baseDate;
            //                }
            //            }
            //            if (parts[0] == "YESTERDAY")
            //            {
            //                baseDate = DateTime.Today.AddDays(-1);
            //                int offset;
            //                if (int.TryParse(parts[2], out offset))
            //                {
            //                    return baseDate.AddDays(parts[1] == "+" ? offset : -offset);
            //                }
            //                else
            //                {
            //                    return baseDate;
            //                }
            //            }
            //            if (parts[0] == "MONTH")
            //            {
            //                baseDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //                int offset;
            //                if (int.TryParse(parts[2], out offset))
            //                {
            //                    return baseDate.AddMonths(parts[1] == "+" ? offset : -offset);
            //                }
            //                else{ 
            //                    return baseDate;
            //                }
            //            }
            //            if (parts[0] == "YEAR")
            //            {
            //                baseDate = new DateTime(DateTime.Today.Year,1, 1);
            //                int offset;
            //                if (int.TryParse(parts[2], out offset))
            //                {
            //                    return baseDate.AddYears(parts[1] == "+" ? offset : -offset);
            //                }
            //                else
            //                {
            //                    return baseDate;
            //                }
            //            }
            //            else
            //            {
            //               return baseDate = returnDate;
            //            }
            //        }
            //        else
            //        {
            //            return returnDate;//  throw new ArgumentException($"Invalid expression format: {expression}");
            //        }
            //    }
            //    else
            //    {
            //    //   throw new ArgumentException($"Invalid fieldValue format: {fieldValue}");
            //        return returnDate;
            //    }
            }
        }
}
