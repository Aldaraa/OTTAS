using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.Common
{
    public static class VersionInfo
    {

        public static readonly string Version = "1.2.12";
        public static string VersionString { get; set; } = "" +
         "Roster Module\r\n" +
         "Single and Multiple roster responses are now displayed in the same format (Overbooked seats are highlighted in red).\r\n\r\n" +

         "Active Transport Module\r\n" +
         "Real ETD (Actual Departure Time) can now be edited directly by date.\r\n" +
         "In the Schedule list, the default current year filter is automatically applied.\r\n" +
         "Column filters have been added to the Schedule list.\r\n\r\n" +

         "Request Module\r\n" +
         "NoShow/GoShow data for domestic travel is now stored in a standardized format.\r\n" +
         "In international travel requests, the task description now includes the Flight Travel Date.\r\n" +
         "Approve validation – Users can no longer approve their own requests.\r\n\r\n" +

         "Manage Schedule\r\n" +
         "PAX information now displays the system-generated completion date.\r\n\r\n" +

         "Accommodation Module\r\n" +
         "After moving people between rooms, the response section now includes a 'View Booking' button that links directly to the room information.";



    }

 }
