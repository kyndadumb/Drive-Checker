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
            // open xml-document: config.xml
            XmlDocument config = new XmlDocument();
            config.Load(@"C:\Inst\Config.xml");

            // create weekday with german cultureinfo
            CultureInfo culture = new CultureInfo("de-DE");
            String weekday = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);

            // read weekday node and drivename
            XmlNode XML_Drive = config.SelectSingleNode("/days/day[@name='" + weekday + "']/@drive");
            String driveName = XML_Drive.Value + ":\\";

            // validate parameter - driveName
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

                // if - driveName is there and drive is ready
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

            // create eventlog-source if it doesn't exist
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, target);
            }
            
            // write eventlog message and write to console
            EventLog.WriteEntry(source, message, type, eventId);
            Console.WriteLine(message);
        }
    }
}
