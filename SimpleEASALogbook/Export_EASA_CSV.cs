using System.Collections.Generic;

namespace SimpleEASALogbook
{
    class Export_EASA_CSV
    {
        protected string stringbuilder = "";
        public Export_EASA_CSV(List<Flight> flights)
        {
            foreach (Flight flight in flights)
            {
                stringbuilder += flight.getDateString() + ";";
                stringbuilder += flight.getDepartureString() + ";";
                stringbuilder += flight.getOffBlockTimeString() + ";";
                stringbuilder += flight.getDestinationString() + ";";
                stringbuilder += flight.getOnBlockTimeString() + ";";
                stringbuilder += flight.getTypeOfAircraftString() + ";";
                stringbuilder += flight.getRegistrationString() + ";";
                stringbuilder += flight.getSEPTimeString() + ";";
                stringbuilder += flight.getMEPTimeString() + ";";
                stringbuilder += flight.getMultiPilotTimeString() + ";";
                stringbuilder += flight.getTotalTimeString() + ";";
                stringbuilder += flight.getPICNameString() + ";";
                stringbuilder += flight.getDayLDGString() + ";";
                stringbuilder += flight.getNightLDGString() + ";";
                stringbuilder += flight.getNightTimeString() + ";";
                stringbuilder += flight.getIFRTimeString() + ";";
                stringbuilder += flight.getPICTimeString() + ";";
                stringbuilder += flight.getCopilotTimeString() + ";";
                stringbuilder += flight.getDualTimeString() + ";";
                stringbuilder += flight.getInstructorTimeString() + ";";
                stringbuilder += flight.getSimDateString() + ";";
                stringbuilder += flight.getSimTypeString() + ";";
                stringbuilder += flight.getSimTimeString() + ";";
                stringbuilder += flight.getRemarksString() + ";";
                stringbuilder += flight.getPageBreakString();
                stringbuilder += "\n";
            }
        }
        public string GetCSV()
        {
            return stringbuilder;
        }
    }
}
