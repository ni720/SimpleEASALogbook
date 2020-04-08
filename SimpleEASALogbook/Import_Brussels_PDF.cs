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
        private TimeSpan begin_time = TimeSpan.Zero;
        private TimeSpan CoPilotTime = TimeSpan.Zero;
        private DateTime Date = DateTime.MinValue;
        private TimeSpan DualTime = TimeSpan.Zero;
        private DateTime END = DateTime.MinValue;
        private TimeSpan end_time = TimeSpan.Zero;
        private List<Flight> Flights = new List<Flight>();
        private string from = "";
        private TimeSpan IFRTime = TimeSpan.Zero;
        private TimeSpan InstructorTime = TimeSpan.Zero;    // always zero because no test data available
        private int ldg_day = 0;
        private int ldg_night = 0;
        private TimeSpan multiPilotTime = TimeSpan.Zero;
        private TimeSpan nightTime = TimeSpan.Zero;
        private string PIC = "";
        private TimeSpan PICTime = TimeSpan.Zero;           // always zero because no test data available
        private string remarks = "";
        private TimeSpan SimTime = TimeSpan.Zero;
        private string to = "";
        private TimeSpan TotalTime = TimeSpan.Zero;
        private string type = "";

        public Import_Brussels_PDF(string textToParse)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            try
            {
                Regex regexAllFlights = new Regex(@"\d\d\w\w\w\d\d\d\d\D*\w\w\w\ *\d\d\:\d\d\ *\w\w\w\ *\d\d\:\d\d.*");
                Regex regexSimFLigts = new Regex(@"\d\d\/\d\d/\d\d\ *\d\d\d\ *\d\:\d\d\ .*");
                Regex regexNight_and_Landings = new Regex(@"\d\d\d\:\d\d");

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
                    Date = DateTime.MinValue;
                    END = DateTime.MinValue;
                    TotalTime = TimeSpan.Zero;
                    begin_time = TimeSpan.Zero;
                    end_time = TimeSpan.Zero;
                    multiPilotTime = TimeSpan.Zero;
                    CoPilotTime = TimeSpan.Zero;
                    nightTime = TimeSpan.Zero;
                    IFRTime = TimeSpan.Zero;
                    SimTime = TimeSpan.Zero;

                    Date = DateTime.ParseExact(flightmatch.Value.Substring(0, 9), "ddMMMyyyy", provider);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(12, 5), "hh\\:mm", provider, out begin_time);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(20, 5), "hh\\:mm", provider, out end_time);
                    from = flightmatch.Value.Replace(" ", "").Substring(9, 3);
                    to = flightmatch.Value.Replace(" ", "").Substring(17, 3);
                    type = flightmatch.Value.Replace(" ", "").Replace("+", "").Substring(25, 3);
                    aircraft = flightmatch.Value.Replace(" ", "").Replace("+", "").Substring(28, 5);
                    PIC = flightmatch.Value.Replace(" ", "").Replace("+", "").Substring(37);
                    string[] numbersSplit = Regex.Split(PIC, @"\d");
                    PIC = numbersSplit[0];
                    remarks = numbersSplit[numbersSplit.Length - 1];
                    // unfortunately it is only possible to detect IF a LVO has taken place, not how many
                    if (numbersSplit.Length == 10)
                    {
                        remarks = numbersSplit[numbersSplit.Length - 2];
                        remarks += "1 LVO APCH";
                    }
                    remarks = remarks.Replace("\r", "").Replace("\n", "");// linebreaks can be harmful to database, since this will be a csv file

                    foreach (Match ldgmatch in regexNight_and_Landings.Matches(flightmatch.Value.Replace(" ", "")))
                    {
                        int.TryParse(ldgmatch.Value.Replace(" ", "").Substring(0, 1), out ldg_day);
                        int.TryParse(ldgmatch.Value.Replace(" ", "").Substring(1, 1), out ldg_night);
                    }
                    foreach (Match nightmatch in regexNight_and_Landings.Matches(flightmatch.Value.Replace(" ", "")))
                    {
                        TimeSpan.TryParseExact(nightmatch.Value.Replace(" ", "").Substring(2, 4), "h\\:mm", provider, out nightTime);
                    }
                    // if flight goes past midnight
                    Date = Date.Add(begin_time);
                    if (end_time.Hours < begin_time.Hours)
                    {
                        END = Date.Add(end_time.Subtract(begin_time)).AddHours(24);
                    }
                    else
                    {
                        END = Date.Add(end_time.Subtract(begin_time));
                    }
                    // we are faking the values and not actually parsing
                    TotalTime = END.Subtract(Date);
                    multiPilotTime = END.Subtract(Date);
                    CoPilotTime = END.Subtract(Date);
                    IFRTime = END.Subtract(Date);
                    // Ignore times for observer flights
                    if (regexNight_and_Landings.Matches(flightmatch.Value.Replace(" ", "")).Count < 1)
                    {
                        TotalTime = TimeSpan.Zero;
                        CoPilotTime = TimeSpan.Zero;
                        IFRTime = TimeSpan.Zero;
                        multiPilotTime = TimeSpan.Zero;
                    }
                    Flights.Add(new Flight(Date, from, begin_time, to, end_time, type, aircraft, false, false, multiPilotTime, TotalTime, PIC, ldg_day, ldg_night, nightTime, IFRTime, PICTime, CoPilotTime, DualTime, InstructorTime, DateTime.MinValue, "", TimeSpan.Zero, remarks, false));
                }
                foreach (Match flightmatch in regexSimFLigts.Matches(textToParse))
                {
                    // default values have to be re-initialized for each flight
                    type = "";
                    remarks = "";
                    Date = DateTime.MinValue;
                    SimTime = TimeSpan.Zero;

                    Date = DateTime.ParseExact(flightmatch.Value.Substring(0, 8), "dd/MM/yy", provider);
                    type = flightmatch.Value.Replace(" ", "").Substring(8, 3);
                    TimeSpan.TryParseExact(flightmatch.Value.Replace(" ", "").Substring(11, 4), "h\\:mm", provider, out SimTime);
                    remarks = flightmatch.Value.Substring(28).Trim();

                    Flights.Add(new Flight(Date, "", TimeSpan.Zero, "", TimeSpan.Zero, "", "", false, false, TimeSpan.Zero, TimeSpan.Zero, "", 0, 0, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, Date, type, SimTime, remarks, false));
                }
                if (Flights.Count < 1)
                {
                    _ErrorOccured = true;
                    File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_Brussels_PDF: found no Flights to parse.\n");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "_easa_errorlog.txt"), DateTime.Now.ToString() + " Import_Brussels_PDF:\n" + e.ToString() + "\n");
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