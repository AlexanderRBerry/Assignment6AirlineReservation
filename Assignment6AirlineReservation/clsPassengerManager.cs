using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Manages Passenger objects
    /// Extracts passenger information from a database
    /// </summary>
    public class clsPassengerManager
    {
        /// <summary>
        /// List of all passengers
        /// </summary>
        private List<clsPassenger> passengers;
        /// <summary>
        /// Extracts passenger information for a specified flight from a database
        /// Creates a list of clsPassenger objects
        /// </summary>
        /// <param name="flightID">The flight the passengers are assigned to</param>
        /// <returns>A list of clsPassenger objects assigned to the provided flight</returns>
        public List<clsPassenger> GetPassengers(int flightID)
        {
            try
            {
                // Initialize passengers
                passengers = new List<clsPassenger>();
                // This connects to the database
                clsDataAccess database = new clsDataAccess();
                // Data set to hold passenger data
                DataSet dsPassengers = new DataSet();
                // SQL statement to be executed
                // Grabs passengers on a given flight
                string sSQL = "SELECT PASSENGER.Passenger_ID, First_Name, Last_Name, Seat_Number " +
                    "FROM FLIGHT_PASSENGER_LINK, FLIGHT, PASSENGER " +
                    "WHERE FLIGHT.FLIGHT_ID = FLIGHT_PASSENGER_LINK.FLIGHT_ID AND " +
                    "FLIGHT_PASSENGER_LINK.PASSENGER_ID = PASSENGER.PASSENGER_ID AND " +
                    "FLIGHT.FLIGHT_ID = " + flightID.ToString();

                // This will be the returned row count
                int rowCount = 0;
                // Execute the SQL statement
                dsPassengers = database.ExecuteSQLStatement(sSQL, ref rowCount);

                // Add all passengers to a list
                foreach (DataRow row in dsPassengers.Tables[0].Rows)
                {
                    clsPassenger passenger = new clsPassenger();
                    passenger.id = row[0].ToString();
                    passenger.firstName = row[1].ToString();
                    passenger.lastName = row[2].ToString();
                    passenger.seatNumber = row[3].ToString();

                    passengers.Add(passenger);
                }

                return passengers;
            }
            catch (Exception ex)
            {
                //Throw an exception
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }
    }
}
