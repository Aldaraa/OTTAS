using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Utils
{
    public static class Transliterator
    {
        private static readonly Dictionary<string, string> _translitDictionary;

        static Transliterator()
        {
            _translitDictionary = new Dictionary<string, string>
                {
                    {"a", "а"}, {"b", "б"}, {"t", "т"},
                    {"g", "г"}, {"d", "д"}, {"e", "э"},
                    {"v", "в"}, {"z", "з"}, {"i", "и"},
                    {"j", "ж"}, {"k", "к"}, {"l", "л"},
                    {"m", "м"}, {"n", "н"}, {"o", "о"},
                    {"p", "п"}, {"r", "р"}, {"s", "с"},
                    {"u", "у"}, {"f", "ф"}, {"h", "х"},
                    {"c", "ц"}, {"ch", "ч"}, {"sh", "ш"},
                    {"sch", "щ"}, {"'", "ь"}, {"y", "ы"},
                    {"\"", "ъ"}, {"ye", "э"}, {"yu", "ю"},
                    {"ya", "я"}, {"kh", "х" }
                };

            var keys = new HashSet<string>();
            foreach (var key in _translitDictionary.Keys)
            {
                if (!keys.Add(key))
                {
                    throw new ArgumentException($"Duplicate key: {key}");
                }
            }
        }


        public static string CapitalizeFirst(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Check if the first character is already uppercase
            if (char.IsUpper(input[0]))
            {
                return input;
            }

            // Convert only the first character to uppercase
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        public static string LatinToCyrillic(string input, bool firsUppercase = true)
        {
            StringBuilder output = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                string latinChar = input[i].ToString().ToLower();
                if (i + 1 < input.Length)
                {
                    string possibleTwoChar = input.Substring(i, 2).ToLower();
                    if (_translitDictionary.ContainsKey(possibleTwoChar))
                    {
                        output.Append(_translitDictionary[possibleTwoChar]);
                        i++; // Skip the next character since it's part of a two-character group
                        continue;
                    }
                }

                if (_translitDictionary.ContainsKey(latinChar))
                {
                    output.Append(_translitDictionary[latinChar]);
                }
                else
                {
                    output.Append(input[i]); // Append original character if no mapping found
                }
            }

            if (firsUppercase)
            {
              return  CapitalizeFirst(output.ToString());
            }
            return output.ToString();
        }


        public static DateTime ExtractDOB(string registrationNumber)
        {
            try
            {
                // Skip the first two characters and take the rest
                string numberPart = registrationNumber.Substring(2);

                // Check if the length is 8, if not assume it's missing leading zeros
                numberPart = numberPart.PadLeft(8, '0');

                int yearPart = int.Parse(numberPart.Substring(0, 2));
                int monthPart = int.Parse(numberPart.Substring(2, 2));
                int dayPart = int.Parse(numberPart.Substring(4, 2));

                int year;

                // Adjust the year based on month part
                if (monthPart > 20)
                {
                    year = 2000 + yearPart;
                    monthPart -= 20;
                }
                else
                {
                    year = 2000 + yearPart;
                }

                // Create a DateTime object for the extracted date
                DateTime dob = new DateTime(year, monthPart, dayPart);

                return dob;
            }
            catch (Exception)
            {
                return DateTime.Today;
            }

        }

       public static int GetWeekNumber(DateTime date)
        {
            //CultureInfo culture = CultureInfo.CurrentUICulture;
            //Calendar calendar = culture.Calendar;
            //return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    date,
                    CalendarWeekRule.FirstFourDayWeek,  // ISO standard
                    DayOfWeek.Monday                   // ISO standard first day of the week
                );
        }




        public static string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove newline characters and extra spaces, convert to lowercase
            return input.Replace("\r\n", " ").Replace("\n", " ").ToLowerInvariant().Trim();
        }


        public static bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

    }
}
