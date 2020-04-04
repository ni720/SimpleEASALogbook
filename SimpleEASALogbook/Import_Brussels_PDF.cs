using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace SimpleEASALogbook
{
    internal class Import_Brussels_PDF
    {
        private bool _ErrorOccured = false;
        private string aircraft = "";
        private DateTime begin, end;
        private TimeSpan begin_time = TimeSpan.Zero;
        private TimeSpan CoPilotTime = TimeSpan.Zero;
        private TimeSpan end_time = TimeSpan.Zero;
        private List<Flight> Flights = new List<Flight>();
        private string from = "";
        private TimeSpan IFRTime = TimeSpan.Zero;
        private TimeSpan InstructorTime = TimeSpan.Zero;
        private int ldg_day = 0;
        private int ldg_night = 0;
        private TimeSpan multiPilotTime = TimeSpan.Zero;

        // is not stated in the pdf. total time is taken instead
        private TimeSpan nightTime = TimeSpan.Zero;

        private string PIC = "";
        private TimeSpan PICTime = TimeSpan.Zero;
        private string remarks = "";

        // is not parsed
        private TimeSpan SimTime = TimeSpan.Zero;

        private string to = "";
        private string type = "";

        public Import_Brussels_PDF(string textToParse)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            try
            {
                Regex regexAllFlights = new Regex(@"\d\d\w\w\w\d\d\d\d\D*\w\w\w\ *\d\d\:\d\d\ *\w\w\w\ *\d\d\:\d\d\ *\d\d\d\ *\w\w\w\w\w\ .*");
                Regex regexSimFLigts = new Regex(@"\d\d\/\d\d/\d\d\ *\d\d\d\ *\d\:\d\d\ .*");

                foreach (Match flightmatch in regexAllFlights.Matches(textToParse))
                {
                    // default values have to be re-initialized for each flight
                    ldg_day = 0;
                    ldg_night = 0;
                    from = "";
                    to = "";
                    aircraft = "";
                    type = "";
                    PIC = "";
                    remarks = "";
                    begin = DateTime.MinValue;
                    end = DateTime.MinValue;
                    begin_time = TimeSpan.Zero;
                    end_time = TimeSpan.Zero;
                    multiPilotTime = TimeSpan.Zero;
                    CoPilotTime = TimeSpan.Zero;
                    nightTime = TimeSpan.Zero;
                    IFRTime = TimeSpan.Zero;
                    PICTime = TimeSpan.Zero;
                    SimTime = TimeSpan.Zero;

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
                    // default values have to be re-initialized for each flight
                    type = "";
                    remarks = "";
                    begin = DateTime.MinValue;
                    SimTime = TimeSpan.Zero;

                    begin = DateTime.ParseExact(flightmatch.Value.Substring(0, 8), "dd/MM/yy", provider);
                    type = flightmatch.Value.Replace(" ", "").Substring(8, 3);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(11, 4), "h\\:mm", provider, out SimTime);
                    remarks = flightmatch.Value.Substring(28).Trim();

                    Flights.Add(new Flight(begin, "", TimeSpan.MinValue, "", TimeSpan.MinValue, "", "", TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, begin, type, SimTime, remarks, false));
                }
                if (Flights.Count < 1)
                {
                    _ErrorOccured = true;
                    File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + " Import_Brussels_PDF: found no Flights to parse.\n");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("_easa_errorlog.txt", DateTime.Now.ToString() + " Import_Brussels_PDF:\n" + e.ToString() + "\n");
                _ErrorOccured = true;
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
    }
}