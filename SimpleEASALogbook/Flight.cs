using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleEASALogbook
{
    public class Flight : IComparable
    {
        public DateTime OffBlockTime = DateTime.MinValue;
        public string DepartureAirport = "";
        public DateTime OnBlockTime = DateTime.MinValue;
        public string DestinationAirport = "";
        public string TypeOfAircraft = "";
        public string AircraftRegistration = "";
        public TimeSpan SEPTime = TimeSpan.Zero;
        public TimeSpan MEPTime = TimeSpan.Zero;
        public TimeSpan MultiPilotTime = TimeSpan.Zero;
        public TimeSpan TotalTimeOfFlight = TimeSpan.Zero;
        public string PilotInCommand = "";
        public int DayLandings = 0;
        public int NightLandings = 0;
        public TimeSpan NightTime = TimeSpan.Zero;
        public TimeSpan IFRTime = TimeSpan.Zero;
        public TimeSpan PICTime = TimeSpan.Zero;
        public TimeSpan CopilotTime = TimeSpan.Zero;
        public TimeSpan DualTime = TimeSpan.Zero;
        public TimeSpan InstructorTime = TimeSpan.Zero;
        public DateTime DateOfSim = DateTime.MinValue;
        public string TypeOfSim = "";
        public TimeSpan SimTime = TimeSpan.Zero;
        public string Remarks = "";
        public bool nextpageafter = false;


        public Flight(DateTime offblock, string dep, DateTime onblock, string dest, string type, string reg, TimeSpan septime, TimeSpan meptime, TimeSpan multitime, TimeSpan totaltime, string pic, int ldgday, int ldgnight, TimeSpan nighttime, TimeSpan ifrtime, TimeSpan pictime, TimeSpan copitime, TimeSpan dualtime, TimeSpan instructortime, DateTime dateofsim, string typeofsim, TimeSpan simtime, string remarks, bool nextpage)
        {
            OffBlockTime = offblock;
            DepartureAirport = dep;
            OnBlockTime = onblock;
            DestinationAirport = dest;
            TypeOfAircraft = type;
            AircraftRegistration = reg;
            SEPTime = septime;
            MEPTime = meptime;
            MultiPilotTime = multitime;
            TotalTimeOfFlight = totaltime;
            PilotInCommand = pic;
            DayLandings = ldgday;
            NightLandings = ldgnight;
            NightTime = nighttime;
            IFRTime = ifrtime;
            PICTime = pictime;
            CopilotTime = copitime;
            DualTime = dualtime;
            InstructorTime = instructortime;
            DateOfSim = dateofsim;
            TypeOfSim = typeofsim;
            SimTime = simtime;
            Remarks = remarks;
            nextpageafter = nextpage;
        }

        public int CompareTo(object obj)
        {
            Flight orderToCompare = obj as Flight;
            if (orderToCompare.OffBlockTime < OffBlockTime)
            {
                return 1;
            }
            if (orderToCompare.OffBlockTime > OffBlockTime)
            {
                return -1;
            }
            // The orders are equivalent.
            return 0;
        }
    }
}
