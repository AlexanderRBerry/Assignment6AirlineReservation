using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Defines the details of a flight object
    /// </summary>
    public class clsFlight
    {
        /// <summary>
        /// Flight ID
        /// </summary>
        private string sID;
        /// <summary>
        /// Flight number
        /// </summary>
        private string sFlightNumber;
        /// <summary>
        /// Aircraft type
        /// </summary>
        private string sAircraftType;
        /// <summary>
        /// Flight ID getter/Setter
        /// </summary>
        public string id
        {
            get { return sID; } set {  sID = value; }
        }
        /// <summary>
        /// Flight number getter/Setter
        /// </summary>
        public string flightNumber
        {
            get { return sFlightNumber; } set { sFlightNumber = value; }
        }
        /// <summary>
        /// Aircraft type getter/Setter
        /// </summary>
        public string aircraftType
        {
            get { return sAircraftType; } set { sAircraftType = value; }
        }
        /// <summary>
        /// clsFlight ToString override
        /// </summary>
        /// <returns>'FlightNumber AircraftType'</returns>
        public override string ToString()
        {
            return sFlightNumber + " " + sAircraftType;
        }
    }
}
