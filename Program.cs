using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Globalization;

namespace DriveChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            XmlDocument config = new XmlDocument();
            config.Load(@"C:\Inst\Config.xml");

            CultureInfo culture = new System.Globalization.CultureInfo("de-DE");
            String weekday = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);

            XmlNode XML_Drive = config.SelectSingleNode("/days/day[@name='" + weekday + "']/@drive");
            String driveName = XML_Drive.Value + ":\\";

            // Validate command line parameters
            if (driveName == String.Empty)
            {
                Console.WriteLine("Kein Laufwerksbuchstabe in der Config.xml gegeben");
                return;
            }

            // Check if the drive is present
            bool isDrivePresent = false;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                String driveInfo = driveName;

                if (driveInfo == drive.Name && drive.IsReady)
                {
                    isDrivePresent = true;
                    break;
                }
            }

            // Write message to event log
            string source = "DriveChecker";
            int eventId = 1009;
            string target = "Application";
            string message = isDrivePresent ? $"Laufwerk {driveName} ist verfügbar. " : $"Laufwerk {driveName} ist nicht verfügbar.";
            EventLogEntryType type = isDrivePresent ? EventLogEntryType.Information : EventLogEntryType.Error;

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, target);
            }
            
            EventLog.WriteEntry(source, message, type, eventId);

            Console.WriteLine(message);
        }
    }
}
