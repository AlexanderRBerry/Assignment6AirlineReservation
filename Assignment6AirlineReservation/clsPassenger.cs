using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Defines the details of a passenger object
    /// </summary>
    public class clsPassenger
    {
        /// <summary>
        /// Passenger ID
        /// </summary>
        private string sID;

        /// <summary>
        /// Passenger first name
        /// </summary>
        private string sFirstName;

        /// <summary>
        /// Passenger last name
        /// </summary>
        private string sLastName;

        /// <summary>
        /// Passenger seat number
        /// </summary>
        private string sSeatNumber;

        /// <summary>
        /// Passenger ID getter/Setter
        /// </summary>
        public string id
        {
            get { return sID; }
            set { sID = value; }
        }

        /// <summary>
        /// Passenger first name getter/Setter
        /// </summary>
        public string firstName
        {
            get { return sFirstName; }
            set { sFirstName = value; }
        }

        /// <summary>
        /// Passenger last name getter/Setter
        /// </summary>
        public string lastName
        {
            get { return sLastName; }
            set { sLastName = value; }
        }

        /// <summary>
        /// Passenger seat number getter/Setter
        /// </summary>
        public string seatNumber
        {
            get { return sSeatNumber; }
            set { sSeatNumber = value; }
        }

        /// <summary>
        /// clsPassenger ToString override
        /// </summary>
        /// <returns>'Firstname LastName'</returns>
        public override string ToString()
        {
            return sFirstName + " " + sLastName;
        }
    }
}
