using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SimpleEASALogbook
{
    class Import_Brussels_PDF
    {
        List<Flight> Flights = new List<Flight>();
        public bool Error = false;

        public Import_Brussels_PDF(string textToParse)
        {

                CultureInfo provider = CultureInfo.InvariantCulture;
                int ldg_day = 0;
                int ldg_night = 0;
                string from = "";
                string to = "";
                string aircraft = "";
                string type = "";
                string PIC = "";
                string remarks = "";
                DateTime begin, end;
                TimeSpan begin_time = TimeSpan.Zero;
                TimeSpan end_time = TimeSpan.Zero;
                TimeSpan multiPilotTime = TimeSpan.Zero;
                TimeSpan CoPilotTime = TimeSpan.Zero;
                TimeSpan nightTime = TimeSpan.Zero;
                TimeSpan IFRTime = TimeSpan.Zero;
                TimeSpan PICTime = TimeSpan.Zero;
                TimeSpan InstructorTime = TimeSpan.Zero;
                TimeSpan SimTime = TimeSpan.Zero;
            try
            {

                Regex regexAllFlights = new Regex(@"\d\d\w\w\w\d\d\d\d\D*\w\w\w\ *\d\d\:\d\d\ *\w\w\w\ *\d\d\:\d\d\ *\d\d\d\ *\w\w\w\w\w\ .*");
                Regex regexSimFLigts = new Regex(@"\d\d\/\d\d/\d\d\ *\d\d\d\ *\d\:\d\d\ .*");
                foreach (Match flightmatch in regexAllFlights.Matches(textToParse))
                {
                    begin = DateTime.ParseExact(flightmatch.Value.Substring(0, 9), "ddMMMyyyy", provider);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(12, 5), "hh\\:mm", provider, out begin_time);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(20, 5), "hh\\:mm", provider, out end_time);
                    from = flightmatch.Value.Replace(" ", "").Substring(9, 3);
                    to = flightmatch.Value.Replace(" ", "").Substring(17, 3);
                    type = flightmatch.Value.Replace(" ", "").Substring(25, 3);
                    aircraft = flightmatch.Value.Replace(" ", "").Substring(28, 5);
                    PIC = flightmatch.Value.Replace(" ", "").Substring(37);
                    string[] numbersSplit = Regex.Split(PIC, @"\d");
                    PIC = numbersSplit[0];
                    remarks = numbersSplit[numbersSplit.Length - 1];
                    int.TryParse(flightmatch.Value.Replace(" ", "").Substring(37 + PIC.Length, 1), out ldg_day);
                    int.TryParse(flightmatch.Value.Replace(" ", "").Substring(37 + PIC.Length + 1, 1), out ldg_night);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(33, 4), "h\\:mm", provider, out multiPilotTime);
                    TimeSpan.TryParseExact(flightmatch.Value.Substring(117, 4), "h\\:mm", provider, out nightTime);
                    TimeSpan.TryParseExact(flightmatch.Value.Substring(128, 4), "h\\:mm", provider, out IFRTime);
                    TimeSpan.TryParseExact(flightmatch.Value.Substring(138, 4), "h\\:mm", provider, out PICTime);
                    begin = begin.Add(begin_time);
                    if (end_time.Hours < begin_time.Hours)
                    {
                        end = begin.Add(end_time.Subtract(begin_time)).AddHours(24);
                    }
                    else
                    {
                        end = begin.Add(end_time.Subtract(begin_time));
                    }
                    Flights.Add(new Flight(begin, from, begin_time, to, end_time, type, aircraft, TimeSpan.Zero, TimeSpan.Zero, multiPilotTime, end.Subtract(begin), PIC, ldg_day, ldg_night, nightTime, IFRTime, PICTime, end.Subtract(begin), TimeSpan.Zero, InstructorTime, DateTime.MinValue, "", TimeSpan.Zero, remarks, false));
                }
                foreach (Match flightmatch in regexSimFLigts.Matches(textToParse))
                {
                    begin = DateTime.ParseExact(flightmatch.Value.Substring(0, 8), "dd/MM/yy", provider);
                    type = flightmatch.Value.Replace(" ", "").Substring(8, 3);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(11, 4), "h\\:mm", provider, out SimTime);
                    remarks = flightmatch.Value.Substring(28).Trim();

                    Flights.Add(new Flight(begin, "", TimeSpan.MinValue, "", TimeSpan.MinValue, "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, begin, type, SimTime, remarks, false));
                }
            }

            catch (Exception ey)
            {
                Error = true;
                Console.WriteLine("error parsing Brussels PDF at");
                Console.WriteLine(ey.ToString());
            }

        }

        public List<Flight> GetFlightList()
        {
            return Flights;
        }
    }
}
