using System;
using System.Collections.Generic;

namespace SimpleEASALogbook
{
    internal class Export_EASA_HTML_2Pages
    {
        private TimeSpan copi = TimeSpan.Zero;
        private int dayldg = 0;
        private TimeSpan dual = TimeSpan.Zero;
        private string HTMLFooter = "</body>\n\n</html>";
        private string HTMLHeader = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\n\n<html>\n<head>\n\t\n\t<meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\"/>\n\t<title>EASA Logbook</title>\n\t<meta name=\"generator\" content=\"LibreOffice 6.3.4.2 (Linux)\"/>\n\t\n\t<style type=\"text/css\">\n\t\tbody,div,p { font-family:\"Liberation Sans\"; font-size:x-small;}\n  \t\ttable,thead,tbody,tfoot,tr,th,td { border: 1px solid black; border-collapse: separate;font-size:x-small; }\n\t\t@media print {\n        table {page-break-after:always;}\nbody > table:last-of-type{page-break-after:auto}\n\t</style>\n\t\n</head>\n\n<body>";
        private TimeSpan ifr = TimeSpan.Zero;
        private TimeSpan instructor = TimeSpan.Zero;
        private int linesOnPage = 18;   // EASA Logbook has 18 lines per page
        private int nghtldg = 0;
        private TimeSpan night = TimeSpan.Zero;
        private List<Flight> pageFlights = new List<Flight>();
        private string PageHeader = "<table cellspacing=\"0\" border=\"0\">\n\t<colgroup width=\"76\"></colgroup>\n\t<colgroup span=\"4\" width=\"38\"></colgroup>\n\t<colgroup span=\"2\" width=\"76\"></colgroup>\n\t<colgroup span=\"6\" width=\"38\"></colgroup>\n\t<colgroup width=\"76\"></colgroup>\n\t<colgroup span=\"14\" width=\"38\"></colgroup>\n\t<colgroup span=\"2\" width=\"76\"></colgroup>\n\t<colgroup span=\"2\" width=\"38\"></colgroup>\n\t<colgroup width=\"151\"></colgroup>\n\t<tr>\n\t\t<td height=\"11\" align=\"center\" valign=middle >1</td>\n\t\t<td colspan=2 align=\"center\" valign=middle >2</td>\n\t\t<td colspan=2 align=\"center\" valign=middle >3</td>\n\t\t<td colspan=2 align=\"center\" valign=middle >4</td>\n\t\t<td colspan=4 align=\"center\" valign=middle >5</td>\n\t\t<td colspan=2 align=\"center\" valign=middle >6</td>\n\t\t<td align=\"center\" valign=middle >7</td>\n\t\t<td colspan=2 align=\"center\" valign=middle >8</td>\n\t\t<td colspan=4 align=\"center\" valign=middle >9</td>\n\t\t<td colspan=8 align=\"center\" valign=middle >10</td>\n\t\t<td colspan=4 align=\"center\" valign=middle >11</td>\n\t\t<td align=\"center\" valign=middle >12</td>\n\t</tr>\n\t<tr>\n\t\t<td rowspan=2 height=\"38\" align=\"center\" valign=middle>Date<br>(dd.mm.yy)</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Departure</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Arrival</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Aircraft</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Single Pilot Time</td>\n\t\t<td colspan=2 rowspan=2 align=\"center\" valign=middle>Multi-Pilot Time</td>\n\t\t<td colspan=2 rowspan=2 align=\"center\" valign=middle>Total Time of Flight</td>\n\t\t<td rowspan=2 align=\"center\" valign=middle>Name PIC</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Landings</td>\n\t\t<td colspan=4 align=\"center\" valign=middle>Operational condition time</td>\n\t\t<td colspan=8 align=\"center\" valign=middle>Pilot Function Time</td>\n\t\t<td colspan=4 align=\"center\" valign=middle>FSTD Session</td>\n\t\t<td rowspan=2 align=\"center\" valign=middle>Remarks and Endorsements</td>\n\t</tr>\n\t<tr>\n\t\t<td align=\"center\" valign=middle>Place</td>\n\t\t<td align=\"center\" valign=middle>Time</td>\n\t\t<td align=\"center\" valign=middle>Place</td>\n\t\t<td align=\"center\" valign=middle>Time</td>\n\t\t<td align=\"center\" valign=middle>Make, Model, Variant</td>\n\t\t<td align=\"center\" valign=middle>Registration</td>\n\t\t<td align=\"center\" valign=middle>SE</td>\n\t\t<td align=\"center\" valign=middle>ME</td>\n\t\t<td align=\"center\" valign=middle>Day</td>\n\t\t<td align=\"center\" valign=middle>Night</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Night</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>IFR</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Pilot – in Command</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Co-Pilot</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Dual</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Instructor</td>\n\t\t<td align=\"center\" valign=middle>Date</td>\n\t\t<td align=\"center\" valign=middle>Type</td>\n\t\t<td colspan=2 align=\"center\" valign=middle>Total of Session</td>\n\t\t</tr>";
        private int pagenumber = 0;
        private TimeSpan pictime = TimeSpan.Zero;
        private TimeSpan simtime = TimeSpan.Zero;
        private string stringBuilder = "";
        private TimeSpan total = TimeSpan.Zero;

        public Export_EASA_HTML_2Pages(List<Flight> flights)
        {
            stringBuilder += HTMLHeader;
            stringBuilder += PageHeader;

            int j = 0;
            for (int i = 0; i < flights.Count; i++)
            {
                // if saved pagebreak detected add empty lines
                if (flights[i].PageBreak)
                {
                    if (j % linesOnPage > 0)
                    {
                        pagenumber++;
                        for (int k = 0; k < linesOnPage - (j % linesOnPage); k++)
                        {
                            // add empty line
                            stringBuilder += EasaOutputLine(new Flight());
                        }
                        j = 0;
                        stringBuilder += CalculatePageFooter(pageFlights, pagenumber);
                        stringBuilder += PageHeader;
                        pageFlights = new List<Flight>();
                    }
                }
                pageFlights.Add(flights[i]);
                stringBuilder += EasaOutputLine(flights[i]);
                j++;
                if (j % linesOnPage == 0)
                {
                    pagenumber++;
                    stringBuilder += CalculatePageFooter(pageFlights, pagenumber);
                    stringBuilder += PageHeader;
                    pageFlights = new List<Flight>();
                }
            }
            // add empty lines on last page
            pagenumber++;
            for (int k = 0; k < linesOnPage - (j % linesOnPage); k++)
            {
                stringBuilder += EasaOutputLine(new Flight());
            }
            stringBuilder += CalculatePageFooter(pageFlights, pagenumber);
            stringBuilder += HTMLFooter;
        }

        public string CalculatePageFooter(List<Flight> pageFlights, int pagenumber)
        {
            string stringbuilder_pageFooter = "";

            TimeSpan total_page = TimeSpan.Zero;
            TimeSpan night_page = TimeSpan.Zero;
            TimeSpan ifr_page = TimeSpan.Zero;
            TimeSpan pictime_page = TimeSpan.Zero;
            TimeSpan copi_page = TimeSpan.Zero;
            TimeSpan dual_page = TimeSpan.Zero;
            TimeSpan instructor_page = TimeSpan.Zero;
            TimeSpan simtime_page = TimeSpan.Zero;
            int dayldg_page = 0;
            int nghtldg_page = 0;

            foreach (Flight flight in pageFlights)
            {
                if (flight.TotalTimeOfFlight.HasValue)
                    total_page = total_page.Add(flight.TotalTimeOfFlight.Value);
                if (flight.NightTime.HasValue)
                    night_page = night_page.Add(flight.NightTime.Value);
                if (flight.IFRTime.HasValue)
                    ifr_page = ifr_page.Add(flight.IFRTime.Value);
                if (flight.PICTime.HasValue)
                    pictime_page = pictime_page.Add(flight.PICTime.Value);
                if (flight.CopilotTime.HasValue)
                    copi_page = copi_page.Add(flight.CopilotTime.Value);
                if (flight.DualTime.HasValue)
                    dual_page = dual_page.Add(flight.DualTime.Value);
                if (flight.InstructorTime.HasValue)
                    instructor_page = instructor_page.Add(flight.InstructorTime.Value);
                if (flight.SimTime.HasValue)
                    simtime_page = simtime_page.Add(flight.SimTime.Value);
                if (flight.DayLandings.HasValue)
                    dayldg_page += flight.DayLandings.Value;
                if (flight.NightLandings.HasValue)
                    nghtldg_page += flight.NightLandings.Value;
            }

            stringbuilder_pageFooter += "<tr>\n<td colspan=7 rowspan=3 height=\"113\" align=\"left\" valign=bottom><br>Page " + pagenumber.ToString() + "</td>\n\t\t<td colspan=4 align=\"left\" valign=bottom>TOTAL THIS PAGE</td>\n\t\t<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)total_page.TotalHours).ToString() + ":" + total_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"left\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += dayldg_page.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += nghtldg_page.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)night_page.TotalHours).ToString() + ":" + night_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)ifr_page.TotalHours).ToString() + ":" + ifr_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)pictime_page.TotalHours).ToString() + ":" + pictime_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)copi_page.TotalHours).ToString() + ":" + copi_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)dual_page.TotalHours).ToString() + ":" + dual_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)instructor_page.TotalHours).ToString() + ":" + instructor_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "\t\t<td align=\"left\" valign=bottom><br></td>\n\t\t<td align=\"left\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)simtime_page.TotalHours).ToString() + ":" + simtime_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td rowspan=3 align=\"center\" valign=top><br><small>I certify that the entries in this log are true.</small><br><br><br>_____________________<br>PILOT'S SIGNATURE</td></tr>";

            stringbuilder_pageFooter += "<td colspan=4 align=\"left\" valign=bottom>TOTAL FROM PREVIOUS PAGES</td>\n\t\t<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)total.TotalHours).ToString() + ":" + total.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"left\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += dayldg.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += nghtldg.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)night.TotalHours).ToString() + ":" + night.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)ifr.TotalHours).ToString() + ":" + ifr.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)pictime.TotalHours).ToString() + ":" + pictime.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)copi.TotalHours).ToString() + ":" + copi.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)dual.TotalHours).ToString() + ":" + dual.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)instructor.TotalHours).ToString() + ":" + instructor.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "\t\t<td align=\"left\" valign=bottom><br></td>\n\t\t<td align=\"left\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)simtime.TotalHours).ToString() + ":" + simtime.Minutes.ToString("00") + "</td>\n</tr>\n";

            total = total.Add(total_page);
            night = night.Add(night_page);
            ifr = ifr.Add(ifr_page);
            pictime = pictime.Add(pictime_page);
            copi = copi.Add(copi_page);
            dual = dual.Add(dual_page);
            instructor = instructor.Add(instructor_page);
            simtime = simtime.Add(simtime_page);
            dayldg += dayldg_page;
            nghtldg += nghtldg_page;

            stringbuilder_pageFooter += "<td colspan=4 align=\"left\" valign=bottom>TOTAL TIME</td>\n\t\t<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)total.TotalHours).ToString() + ":" + total.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"left\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += dayldg.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += nghtldg.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)night.TotalHours).ToString() + ":" + night.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)ifr.TotalHours).ToString() + ":" + ifr.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)pictime.TotalHours).ToString() + ":" + pictime.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)copi.TotalHours).ToString() + ":" + copi.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)dual.TotalHours).ToString() + ":" + dual.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)instructor.TotalHours).ToString() + ":" + instructor.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "\t\t<td align=\"left\" valign=bottom><br></td>\n\t\t<td align=\"left\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)simtime.TotalHours).ToString() + ":" + simtime.Minutes.ToString("00") + "</td>\n</tr>\n";

            return stringbuilder_pageFooter;
        }

        public string EasaOutputLine(Flight flight)
        {
            string _stringbuilder = "";
            _stringbuilder += "<tr>\n<td height=\"19\" align=\"center\" valign=middle >";
            if (!flight.FlightDate.HasValue || flight.FlightDate.Equals(DateTime.MinValue))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.FlightDate.Value.ToString("dd.MM.yy") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            _stringbuilder += flight.DepartureAirport + "</td>\n";

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.OffBlockTime.HasValue || flight.OffBlockTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.OffBlockTime.Value.ToString("%h\\:mm") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            _stringbuilder += flight.DestinationAirport + "</td>\n";

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.OnBlockTime.HasValue || flight.OnBlockTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.OnBlockTime.Value.ToString("%h\\:mm") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            _stringbuilder += flight.TypeOfAircraft + "</td>\n";

            _stringbuilder += "<td align=\"center\" valign=middle>";
            _stringbuilder += flight.AircraftRegistration + "</td>\n";

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.SEPTime)
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += "X" + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.MEPTime)
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += "X" + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.MultiPilotTime.HasValue || flight.MultiPilotTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.MultiPilotTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.MultiPilotTime.HasValue || flight.MultiPilotTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.MultiPilotTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.TotalTimeOfFlight.HasValue || flight.TotalTimeOfFlight.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.TotalTimeOfFlight.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.TotalTimeOfFlight.HasValue || flight.TotalTimeOfFlight.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.TotalTimeOfFlight.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            _stringbuilder += flight.PilotInCommand + "</td>\n";

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.DayLandings.HasValue || flight.DayLandings < 1)
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.DayLandings.Value + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.NightLandings.HasValue || flight.NightLandings < 1)
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.NightLandings.Value + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.NightTime.HasValue || flight.NightTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.NightTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.NightTime.HasValue || flight.NightTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.NightTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.IFRTime.HasValue || flight.IFRTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.IFRTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.IFRTime.HasValue || flight.IFRTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.IFRTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.PICTime.HasValue || flight.PICTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.PICTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.PICTime.HasValue || flight.PICTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.PICTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.CopilotTime.HasValue || flight.CopilotTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.CopilotTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.CopilotTime.HasValue || flight.CopilotTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.CopilotTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.DualTime.HasValue || flight.DualTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.DualTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.DualTime.HasValue || flight.DualTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.DualTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.InstructorTime.HasValue || flight.InstructorTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.InstructorTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.InstructorTime.HasValue || flight.InstructorTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.InstructorTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.DateOfSim.HasValue || flight.DateOfSim.Equals(DateTime.MinValue))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.DateOfSim.Value.ToString("dd.MM.yy") + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            _stringbuilder += flight.TypeOfSim + "</td>\n";

            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.SimTime.HasValue || flight.SimTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.SimTime.Value.Hours.ToString() + "</td>\n";
            }
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.SimTime.HasValue || flight.SimTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += flight.SimTime.Value.Minutes.ToString("00") + "</td>\n";
            }

            _stringbuilder += "<td align=\"center\" valign=middle>";
            _stringbuilder += flight.Remarks + "</td>\n";
            return _stringbuilder;
        }

        public string GetHTML()
        {
            return stringBuilder;
        }
    }
}