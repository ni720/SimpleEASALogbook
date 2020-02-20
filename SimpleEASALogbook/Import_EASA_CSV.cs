using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace SimpleEASALogbook
{
    class Import_EASA_CSV
    {
        DateTime StartDate = DateTime.MinValue;
        string FROM = "";
        TimeSpan beginTime = TimeSpan.Zero;
        string TO = "";
        TimeSpan endTime = TimeSpan.Zero;
        string Aircraft = "";
        string Type = "";
        TimeSpan SETime = TimeSpan.Zero;
        TimeSpan METime = TimeSpan.Zero;
        TimeSpan MultiPilotTime = TimeSpan.Zero;
        TimeSpan TotalTimeOfFlight = TimeSpan.Zero;
        string PIC = "";
        int DayLanding = 0;
        int NightLanding = 0;
        TimeSpan NightTime = TimeSpan.Zero;
        TimeSpan IFRTime = TimeSpan.Zero;
        TimeSpan PICTime = TimeSpan.Zero;
        TimeSpan CopilotTime = TimeSpan.Zero;
        TimeSpan DualTime = TimeSpan.Zero;
        TimeSpan InstructorTime = TimeSpan.Zero;
        DateTime date_of_sim = DateTime.MinValue;
        string Type_of_sim = "";
        TimeSpan sim_time = TimeSpan.Zero;
        string remarks = "";
        bool nextpageafter = false;

        List<Flight> Flights = new List<Flight>();
        public Import_EASA_CSV(string stringToParse)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            using (var reader = new StringReader(stringToParse))
            {
                int i = 0;
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    try
                    {
                        string[] csvline = new string[25];
                        csvline = line.Split(';');
                        Flights.Add(CreateFlight(csvline));

                    }
                    catch (Exception exc)
                    {
                        File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + ": error parsing, skipping line: " + i.ToString() + "\n" + exc.ToString() + "\n");
                    }

                    i++;
                }
            }
        }

        Flight CreateFlight(string[] csvline)
        {
            DateTime.TryParse(csvline[0], out StartDate);
            FROM = csvline[1];
            TimeSpan.TryParse(csvline[2], out beginTime);
            TO = csvline[3];
            TimeSpan.TryParse(csvline[4], out endTime);
            Type = csvline[5];
            Aircraft = csvline[6];
            TimeSpan.TryParse(csvline[7], out SETime);
            TimeSpan.TryParse(csvline[8], out METime);
            TimeSpan.TryParse(csvline[9], out MultiPilotTime);
            TimeSpan.TryParse(csvline[10], out TotalTimeOfFlight);
            PIC = csvline[11];
            int.TryParse(csvline[12], out DayLanding);
            int.TryParse(csvline[13], out NightLanding);
            TimeSpan.TryParse(csvline[14], out NightTime);
            TimeSpan.TryParse(csvline[15], out IFRTime);
            TimeSpan.TryParse(csvline[16], out PICTime);
            TimeSpan.TryParse(csvline[17], out CopilotTime);
            TimeSpan.TryParse(csvline[18], out DualTime);
            TimeSpan.TryParse(csvline[19], out InstructorTime);
            DateTime.TryParse(csvline[20], out date_of_sim);
            Type_of_sim = csvline[21];
            TimeSpan.TryParse(csvline[22], out sim_time);
            remarks = csvline[23];

            if (csvline[24].Equals("pagebreak"))
            {
                nextpageafter = true;
            }
            else
            {
                nextpageafter = false;
            }

            return new Flight(StartDate, beginTime, FROM, endTime, TO, Type, Aircraft, SETime, METime, MultiPilotTime, TotalTimeOfFlight, PIC, DayLanding, NightLanding, NightTime, IFRTime, PICTime, CopilotTime, DualTime, InstructorTime, date_of_sim, Type_of_sim, sim_time, remarks, nextpageafter);
        }
        public List<Flight> getFlightList()
        {
            return Flights;
        }
    }
}
