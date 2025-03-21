using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Manages Flight objects
    /// Extracts Flight information from a database
    /// </summary>
    public class clsFlightManager
    {
        /// <summary>
        /// A list of flight objects
        /// </summary>
        private List<clsFlight> flights;

        /// <summary>
        /// Extracts all flights from the database and compiles them in a list
        /// </summary>
        /// <returns>A List<clsFlights> containing all flights</returns>
        /// <exception cref="Exception"></exception>
        public List<clsFlight> GetFlights()
        {
            try
            {
                // This links to the database
                clsDataAccess dataBase = new clsDataAccess();
                // Get the flights dataset
                DataSet dsFlights = new DataSet();
                // The SQL command to get all flights
                string sSQL = "SELECT Flight_ID, Flight_Number, Aircraft_Type FROM FLIGHT";
                // The number of rows returned
                int rowCount = 0;
                // Dataset holding flights
                dsFlights = dataBase.ExecuteSQLStatement(sSQL, ref rowCount);
                // Initialize flights
                flights = new List<clsFlight>();

                // Add all flight objects to a list
                foreach (DataRow dr in dsFlights.Tables[0].Rows)
                {
                    clsFlight flight = new clsFlight();
                    // dr[i] will hold the value for the ith column of the given row
                    flight.id = dr[0].ToString();
                    flight.flightNumber = dr[1].ToString();
                    flight.aircraftType = dr[2].ToString();

                    flights.Add(flight);
                }

                return flights;
            }
            catch (Exception ex)
            {
                //Throw an exception
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Verifies if a seat number is already in the database link table
        /// </summary>
        /// <param name="seatNumber">The seat number to test for</param>
        /// <param name="flightID">The ID of the flight</param>
        /// <returns>A boolean, false if the seat is open, true if the seat is taken</returns>
        public bool SeatTaken(string seatNumber, int flightID)
        {
            // SQL to check if a seat number is already in the database
            string sSQL = "SELECT Seat_Number FROM Flight_Passenger_Link WHERE Seat_Number = '" +
                   seatNumber + "' AND Flight_ID = " + flightID ;

            clsDataAccess database = new clsDataAccess();

            // dbRetVal will be an empty string if the seat number was not found
            string dbRetVal = database.ExecuteScalarSQL(sSQL);

            // Seat not taken
            if (dbRetVal == "")
            {
                return false;
            }
            // Seat taken
            return true;
        }
    }
}
