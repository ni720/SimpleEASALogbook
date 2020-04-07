using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace SimpleEASALogbook
{
    internal class Import_EASA_CSV
    {
        private bool _ErrorOccured = false;
        private string Aircraft = "";
        private TimeSpan beginTime = TimeSpan.Zero;
        private TimeSpan CopilotTime = TimeSpan.Zero;
        private DateTime date_of_sim = DateTime.MinValue;
        private int DayLanding = 0;
        private TimeSpan DualTime = TimeSpan.Zero;
        private TimeSpan endTime = TimeSpan.Zero;
        private List<Flight> Flights = new List<Flight>();
        private string FROM = "";
        private TimeSpan IFRTime = TimeSpan.Zero;
        private TimeSpan InstructorTime = TimeSpan.Zero;
        private bool METime = false;
        private TimeSpan MultiPilotTime = TimeSpan.Zero;
        private bool nextpageafter = false;
        private int NightLanding = 0;
        private TimeSpan NightTime = TimeSpan.Zero;
        private string PIC = "";
        private TimeSpan PICTime = TimeSpan.Zero;
        private string remarks = "";
        private bool SETime = false;
        private TimeSpan sim_time = TimeSpan.Zero;
        private DateTime StartDate = DateTime.MinValue;
        private string TO = "";
        private TimeSpan TotalTimeOfFlight = TimeSpan.Zero;
        private string Type = "";
        private string Type_of_sim = "";

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
                        File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + ": error parsing EASA CSV, skipping line: " + i.ToString() + "\n" + exc.ToString() + "\n");
                        _ErrorOccured = true;
                    }
                    i++;
                }
            }
            if (Flights.Count < 1)
            {
                _ErrorOccured = true;
                File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + " Import_EASA_CSV: found no Flights to parse.\n");
            }
        }

        public bool GetError()
        {
            return _ErrorOccured;
        }

        public List<Flight> GetFlightList()
        {
            return Flights;
        }

        private Flight CreateFlight(string[] csvline)
        {
            DateTime.TryParse(csvline[0], out StartDate);
            FROM = csvline[1];
            TimeSpan.TryParse(csvline[2], out beginTime);
            TO = csvline[3];
            TimeSpan.TryParse(csvline[4], out endTime);
            Type = csvline[5];
            Aircraft = csvline[6];
            if (csvline[7].Equals("X"))
            {
                SETime = true;
            }
            else
            {
                SETime = false;
            }
            if (csvline[8].Equals("X"))
            {
                METime = true;
            }
            else
            {
                METime = false;
            }
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

            return new Flight(StartDate, FROM, beginTime, TO, endTime, Type, Aircraft, SETime, METime, MultiPilotTime, TotalTimeOfFlight, PIC, DayLanding, NightLanding, NightTime, IFRTime, PICTime, CopilotTime, DualTime, InstructorTime, date_of_sim, Type_of_sim, sim_time, remarks, nextpageafter);
        }
    }
}