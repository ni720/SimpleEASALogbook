using System;
using System.Collections.Generic;

namespace SimpleEASALogbook
{
    internal class Export_EASA_HTML_2Pages
    {
        private TimeSpan copi = TimeSpan.Zero;
        private int dayldg = 0;
        private TimeSpan dual = TimeSpan.Zero;
        private string HTMLFooter = "\n</body>\n</html>";
        private readonly string HTMLHeader = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\n\n<html>\n<head>\n\n<meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\"/>\n<title>EASA Logbook</title>\n\n\n<style type=\"text/css\">\n\ntable,thead,tbody,tfoot,tr,th,td {white-space: nowrap;overflow: hidden;text-overflow: ellipsis;border: 1px solid black; border-collapse: collapse; font-family:\"Open Sans Condensed Light\"; font-size:x-small; table-layout: fixed; line-height: 100%}\n@page {\nsize: A4;\nmargin: 0.7mm;\n}\n@media print {\nhtml, body {\nwidth: 210mm;\nheight: 297mm;\n}\n.pagebreak {\nclear: both;\npage-break-after: always;\n}\n.pagebreak:last-child {\npage-break-after: avoid;\n}\n</style>\n\n</head>\n\n<body>";
        private TimeSpan ifr = TimeSpan.Zero;
        private TimeSpan instructor = TimeSpan.Zero;
        private int linesPerPage = 18;   // EASA Logbook has 18 lines per page
        private int nghtldg = 0;
        private TimeSpan night = TimeSpan.Zero;
        private List<Flight> pageFlights = new List<Flight>();
        private readonly string PageHeaderLeft = "\n<!--- left side --->\n\n<table cellspacing=\"0\" border=\"0\" style=\"width:195mm\">\n\n\n<tr style=\"height:4mm\">\n<td style=\"width:19mm\" align=\"center\" valign=middle >1</td>\n<td style=\"width:20mm\" colspan=2 align=\"center\" valign=middle >2</td>\n<td style=\"width:20mm\" colspan=2 align=\"center\" valign=middle >3</td>\n<td style=\"width:39mm\" colspan=2 align=\"center\" valign=middle >4</td>\n<td style=\"width:40mm\" colspan=4 align=\"center\" valign=middle >5</td>\n<td style=\"width:20mm\" colspan=2 align=\"center\" valign=middle >6</td>\n<td style=\"width:20mm\" align=\"center\" valign=middle >7</td>\n<td style=\"width:14mm\" colspan=2 align=\"center\" valign=middle >8</td>\n\n</tr>\n<tr style=\"height:8mm\">\n<td style=\"width:20mm\" rowspan=2  align=\"center\" valign=middle>DATE<br>(dd.mm.yy)</td>\n<td style=\"width:20mm\" colspan=2 align=\"center\" valign=middle>DEPARTURE</td>\n<td style=\"width:20mm\" colspan=2 align=\"center\" valign=middle>ARRIVAL</td>\n<td style=\"width:40mm\" colspan=2 align=\"center\" valign=middle>AIRCRAFT</td>\n<td style=\"width:20mm\" colspan=2 align=\"center\" valign=middle>SINGLE<br>PILOT TIME</td>\n<td style=\"width:20mm\" colspan=2 rowspan=2 align=\"center\" valign=middle>MULTI-PILOT<br>TIME</td>\n<td style=\"width:20mm\" colspan=2 rowspan=2 align=\"center\" valign=middle>TOTAL TIME<br>OF<br>FLIGHT</td>\n<td style=\"width:20mm\" rowspan=2 align=\"center\" valign=middle>NAME PIC</td>\n<td style=\"width:14mm\" colspan=2 align=\"center\" valign=middle>LANDINGS</td>\n\n</tr>\n<tr style=\"height:3mm\">\n<td style=\"width:10mm\" align=\"center\" valign=middle>PLACE</td>\n<td style=\"width:10mm\" align=\"center\" valign=middle>TIME</td>\n<td style=\"width:10mm\" align=\"center\" valign=middle>PLACE</td>\n<td style=\"width:10mm\" align=\"center\" valign=middle>TIME</td>\n<td style=\"width:19mm; font-size-adjust: 0.4;\" align=\"center\" valign=middle>MAKE, MODEL, VARIANT</td>\n<td style=\"width:19mm\" align=\"center\" valign=middle>REGISTRATION</td>\n<td style=\"width:10mm\" align=\"center\" valign=middle>SE</td>\n<td style=\"width:10mm\" align=\"center\" valign=middle>ME</td>\n<td style=\"width:7mm\" align=\"center\" valign=middle>DAY</td>\n<td style=\"width:7mm\" align=\"center\" valign=middle>NIGHT</td>\n\n\n</tr>\n\n<!--- payload --->\n";
        private readonly string PageHeaderRight = "<!--- right side --->\n<br>\n<br>\n\n<table cellspacing=\"0\" border=\"0\" style=\"width:195mm\">\n\n<tr style=\"height:4mm\">\n<td style=\"width:28mm;\" colspan=4 align=\"center\" valign=middle >9</td>\n<td style=\"width:64mm;\" colspan=8 align=\"center\" valign=middle >10</td>\n<td style=\"width:15mm; border-right: 0mm;\" align=\"center\" valign=middle ></td>\n<td style=\"width:25mm; border-right: 0mm;border-left: 0mm;\" align=\"center\" valign=middle >11</td>\n<td style=\"width:8mm; border-right: 0mm;border-left: 0mm;\" align=\"center\" valign=middle ></td>\n<td style=\"width:7mm; border-left: 0mm;\" align=\"center\" valign=middle ></td>\n<td style=\"width:47mm;\" align=\"center\" valign=middle >12</td>\n</tr>\n<tr style=\"height:6mm\">\n<td style=\"width:28mm;\" colspan=4 align=\"center\" valign=middle>OPERATIONAL CONDITION<br>TIME</td>\n<td style=\"width:64mm;\" colspan=8 align=\"center\" valign=middle>PILOT FUNCTION TIME</td>\n<td style=\"width:55mm;\" colspan=4 align=\"center\" valign=middle>FSTD SESSION</td>\n<td style=\"width:47mm;\" rowspan=2 align=\"center\" valign=middle>REMARKS<br>AND ENDORSEMENTS</td>\n</tr>\n<tr style=\"height:6mm\">\n<td style=\"width:14mm;\" colspan=2 align=\"center\" valign=middle>NIGHT</td>\n<td style=\"width:14mm;\" colspan=2 align=\"center\" valign=middle>IFR</td>\n<td style=\"width:16mm;\" colspan=2 align=\"center\" valign=middle>PILOT -<br>IN-COMMAND</td>\n<td style=\"width:16mm;\" colspan=2 align=\"center\" valign=middle>CO-PILOT</td>\n<td style=\"width:16mm;\" colspan=2 align=\"center\" valign=middle>DUAL</td>\n<td style=\"width:16mm;\" colspan=2 align=\"center\" valign=middle>INSTRUCTOR</td>\n<td style=\"width:15mm;\" align=\"center\" valign=middle>DATE<br>(dd.mm.yy)</td>\n<td style=\"width:25mm;\" align=\"center\" valign=middle>TYPE</td>\n<td style=\"width:15mm;\" colspan=2 align=\"center\" valign=middle>TOTAL TIME<br>OF SESSION</td>\n</tr>\n\n<!--- payload --->\n";
        private readonly string PageFooter = "\n<div class=\"pagebreak\"> </div>";
        private int pagenumber = 0;
        private TimeSpan pictime = TimeSpan.Zero;
        private TimeSpan simtime = TimeSpan.Zero;
        private string stringBuilder = "";
        private TimeSpan total = TimeSpan.Zero;

        public Export_EASA_HTML_2Pages(List<Flight> flights)
        {
            stringBuilder += HTMLHeader;

            int _linesOnPage = 0;
            string _strBldLeft = PageHeaderLeft, _strBldRight = PageHeaderRight;

            for (int i = 0; i < flights.Count; i++)
            {


                // if saved pagebreak detected add empty lines
                if (flights[i].PageBreak)
                {
                    if (_linesOnPage % linesPerPage > 0)
                    {
                        pagenumber++;
                        for (int k = 0; k < linesPerPage - (_linesOnPage % linesPerPage); k++)
                        {
                            // add empty line
                            _strBldLeft += EasaOutputLineLeft(new Flight());
                            _strBldRight += EasaOutputlineRight(new Flight());
                        }
                        _linesOnPage = 0;
                        _strBldLeft += CalculatePageFooterLeft(pageFlights, pagenumber);
                        _strBldRight += CalculatePageFooterRight(pageFlights, pagenumber);
                        stringBuilder += _strBldLeft+_strBldRight;
                        pageFlights = new List<Flight>();
                    }

                    _strBldLeft = PageHeaderLeft;
                    _strBldRight = PageHeaderRight;

                }
                pageFlights.Add(flights[i]);
                _strBldLeft += EasaOutputLineLeft(flights[i]);
                _strBldRight += EasaOutputlineRight(flights[i]);

                _linesOnPage++;
                if (_linesOnPage % linesPerPage == 0)
                {
                    pagenumber++;
                    _strBldLeft += CalculatePageFooterLeft(pageFlights, pagenumber);
                    _strBldRight += CalculatePageFooterRight(pageFlights, pagenumber);
                    stringBuilder += _strBldLeft + _strBldRight; ;
                    pageFlights = new List<Flight>();
                    _strBldLeft = PageHeaderLeft;
                    _strBldRight = PageHeaderRight;
                }
            }
            // add empty lines on last page
            pagenumber++;
            for (int k = 0; k < linesPerPage - (_linesOnPage % linesPerPage); k++)
            {
                _strBldLeft += EasaOutputLineLeft(new Flight());
                _strBldRight += EasaOutputlineRight(new Flight());
            }
            stringBuilder += _strBldLeft+CalculatePageFooterLeft(pageFlights, pagenumber)+ _strBldRight + CalculatePageFooterRight(pageFlights, pagenumber);
            stringBuilder += HTMLFooter;
        }

        public string CalculatePageFooterLeft(List<Flight> pageFlights, int pagenumber)
        {
            string stringbuilder_pageFooter = "";

            TimeSpan total_page = TimeSpan.Zero;
            int dayldg_page = 0;
            int nghtldg_page = 0;

            foreach (Flight flight in pageFlights)
            {
                if (flight.TotalTimeOfFlight.HasValue)
                    total_page = total_page.Add(flight.TotalTimeOfFlight.Value);
                if (flight.DayLandings.HasValue)
                    dayldg_page += flight.DayLandings.Value;
                if (flight.NightLandings.HasValue)
                    nghtldg_page += flight.NightLandings.Value;
            }

            stringbuilder_pageFooter += "<!--- /payload --->\n\n\t\t\t<tr style=\"height: 10mm\">\n\t\t\t\t<td style=\"padding:5px;\" colspan=7 rowspan=3  align=\"left\" valign=bottom><br>Page " + pagenumber.ToString() + "</td>\n\t\t\t\t\t<td style=\"padding:1px;\" colspan=4 align=\"center\" valign=bottom>TOTAL THIS PAGE</td>\n\t\t\t\t<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)total_page.TotalHours).ToString() + ":" + total_page.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += dayldg_page.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += nghtldg_page.ToString() + "</td>\n</tr>";

            stringbuilder_pageFooter += "<tr style=\"height: 10mm\">\n\t\t\t\t<td style=\"padding:1px;\" colspan=4 align=\"center\" valign=bottom>TOTAL FROM PREVIOUS PAGES</td>\n\t\t\t\t<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)total.TotalHours).ToString() + ":" + total.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += dayldg.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += nghtldg.ToString() + "</td>\n</tr>";

            total = total.Add(total_page);
            dayldg += dayldg_page;
            nghtldg += nghtldg_page;

            stringbuilder_pageFooter += "<tr style=\"height: 10mm\">\n\t\t\t\t<td style=\"padding:1px;\" colspan=4 align=\"center\" valign=bottom>TOTAL TIME</td>\n\t\t\t\t<td colspan=2 align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += ((int)total.TotalHours).ToString() + ":" + total.Minutes.ToString("00") + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom><br></td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += dayldg.ToString() + "</td>\n";
            stringbuilder_pageFooter += "<td align=\"center\" valign=bottom >";
            stringbuilder_pageFooter += nghtldg.ToString() + "</td>\n</tr>\n</table>";

            return stringbuilder_pageFooter;
        }
        public string CalculatePageFooterRight(List<Flight> pageFlights, int pagenumber)
        {
            string stringbuilder_pageFooter = "";

            TimeSpan night_page = TimeSpan.Zero;
            TimeSpan ifr_page = TimeSpan.Zero;
            TimeSpan pictime_page = TimeSpan.Zero;
            TimeSpan copi_page = TimeSpan.Zero;
            TimeSpan dual_page = TimeSpan.Zero;
            TimeSpan instructor_page = TimeSpan.Zero;
            TimeSpan simtime_page = TimeSpan.Zero;


            foreach (Flight flight in pageFlights)
            {

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

            }


            stringbuilder_pageFooter += "<!--- /payload --->\n\n\t\t\t<tr style=\"height: 10mm\">\n\t\t\t\t<td colspan=2 align=\"center\" valign=bottom >";
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
            stringbuilder_pageFooter += "<td rowspan=3 align=\"center\" valign=top><br><small>I certify that the entries in this log are true.</small><br><br><br><br><br>_____________________<br>PILOT'S SIGNATURE</td>";


            stringbuilder_pageFooter += "<tr style=\"height: 10mm\">\n\t\t\t\t<td colspan=2 align=\"center\" valign=bottom >";
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


            night = night.Add(night_page);
            ifr = ifr.Add(ifr_page);
            pictime = pictime.Add(pictime_page);
            copi = copi.Add(copi_page);
            dual = dual.Add(dual_page);
            instructor = instructor.Add(instructor_page);
            simtime = simtime.Add(simtime_page);


            stringbuilder_pageFooter += "<tr style=\"height: 10mm\">\n\t\t\t\t<td colspan=2 align=\"center\" valign=bottom >";
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
            stringbuilder_pageFooter += ((int)simtime.TotalHours).ToString() + ":" + simtime.Minutes.ToString("00") + "</td>\n</tr>\n</table>";

            stringbuilder_pageFooter += PageFooter;

            return stringbuilder_pageFooter;
        }

        public string EasaOutputLineLeft(Flight flight)
        {
            string _stringbuilder = "";
            _stringbuilder += "<tr style=\"height:5mm\"><td align=\"center\" valign=middle >";
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
                _stringbuilder += ((int)flight.MultiPilotTime.Value.TotalHours).ToString() + "</td>\n";
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
                _stringbuilder += ((int)flight.TotalTimeOfFlight.Value.TotalHours).ToString() + "</td>\n";
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

            _stringbuilder += "</tr>";
            return _stringbuilder;
        }

        public string EasaOutputlineRight(Flight flight)
        {
            string _stringbuilder = "";

            _stringbuilder += "<tr style=\"height:5mm\">\n";
            _stringbuilder += "<td align=\"center\" valign=middle>";
            if (!flight.NightTime.HasValue || flight.NightTime.Equals(TimeSpan.Zero))
            {
                _stringbuilder += "</td>\n";
            }
            else
            {
                _stringbuilder += ((int)flight.NightTime.Value.TotalHours).ToString() + "</td>\n";
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
                _stringbuilder += ((int)flight.IFRTime.Value.TotalHours).ToString() + "</td>\n";
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
                _stringbuilder += ((int)flight.PICTime.Value.TotalHours).ToString() + "</td>\n";
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
                _stringbuilder += ((int)flight.CopilotTime.Value.TotalHours).ToString() + "</td>\n";
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
                _stringbuilder += ((int)flight.DualTime.Value.TotalHours).ToString() + "</td>\n";
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
                _stringbuilder += ((int)flight.InstructorTime.Value.TotalHours).ToString() + "</td>\n";
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
                _stringbuilder += ((int)flight.SimTime.Value.TotalHours).ToString() + "</td>\n";
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

            _stringbuilder += "<td align=\"left\" valign=middle>";
            _stringbuilder += flight.Remarks + "</td>\n";
            
            _stringbuilder += "</tr>";
            return _stringbuilder;
        }

        public string GetHTML()
        {
            return stringBuilder;
        }
    }
}