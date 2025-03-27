using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Exception_Handler;

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The Add Passenger window
        /// </summary>
        wndAddPassenger wndAddPass;

        /// <summary>
        /// Flight Manager
        /// </summary>
        clsFlightManager flightManager;

        /// <summary>
        /// Passenger Manager
        /// </summary>
        clsPassengerManager passengerManager;

        /// <summary>
        /// Determines if the program is in "Add Passenger Mode"
        /// </summary>
        public static bool bAddingPassenger = false;

        /// <summary>
        /// Determines if the program is in "Change Seat Mode"
        /// </summary>
        public bool bChangingSeat = false;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose; // Ensures complete shutdown

                // Initialize a flight manager
                flightManager = new clsFlightManager();

                // Load the choose flight combo box with available flights
                cbChooseFlight.ItemsSource = flightManager.GetFlights();

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Displays appropriate flight details and activates relevant controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChooseFlight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // This represents the flights id
                int selection = cbChooseFlight.SelectedIndex + 1;

                // Enable relevant controlls
                cbChoosePassenger.IsEnabled = true;
                gPassengerCommands.IsEnabled = true;

                UpdateFlightDetails(selection);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Updates flight details based on the selected flight
        /// </summary>
        /// <param name="flightID">The flight ID of the selected flight</param>
        private void UpdateFlightDetails(int flightID)
        {
            try
            {
                // Initialize passenger manager
                passengerManager = new clsPassengerManager();

                // This list holds all passengers on the current flight
                List<clsPassenger> passengers = passengerManager.GetPassengers(flightID);

                // Bind passengers to the passenger combo box
                cbChoosePassenger.ItemsSource = passengers;

                // Reset the color of all seats to blue and mark taken seats red
                ResetSeats(flightID, ref passengers);
            }
            catch (Exception ex)
            {
                //Throw an exception
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Resets all seats to blue and then marks taken seats as red
        /// </summary>
        /// <param name="flightID">Flight ID of the current flight</param>
        /// <param name="passengers">Reference to a list of passengers</param>
        public void ResetSeats(int flightID, ref List<clsPassenger> passengers)
        {
            if (flightID == 1)
            {
                // Display appropriate flight 
                CanvasA380.Visibility = Visibility.Hidden;
                Canvas767.Visibility = Visibility.Visible;

                // Mark taken seats red and avaliable seats blue
                foreach (Label seat in c767_Seats.Children)
                {
                    // Reset each seat to blue once
                    seat.Background = new SolidColorBrush(Colors.Blue);

                    // Iterate through passengers
                    foreach (clsPassenger passenger in passengers)
                    {
                        // The seat is taken
                        if (seat.Name == ("Seat" + passenger.seatNumber))
                        {
                            seat.Background = new SolidColorBrush(Colors.Red);
                            break;
                        }
                    }
                }
            }
            else
            {
                // Display appropriate flight
                Canvas767.Visibility = Visibility.Hidden;
                CanvasA380.Visibility = Visibility.Visible;

                // Mark taken seats red and avaliable seats blue
                foreach (Label seat in cA380_Seats.Children)
                {
                    // Reset each seat to blue once
                    seat.Background = new SolidColorBrush(Colors.Blue);

                    // Iterate through passengers
                    foreach (clsPassenger passenger in passengers)
                    {
                        //TODO: This if won't ever be true. Remove
                        if (seat.Background == new SolidColorBrush(Colors.Red))
                        {
                            break;
                        }

                        // The seat is taken
                        if (seat.Name == ("SeatA" + passenger.seatNumber))
                        {
                            seat.Background = new SolidColorBrush(Colors.Red);
                            break;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Displays the add passenger form
        /// Disables most controls so a user must select a seat for the added passenger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize add passenger window
                wndAddPass = new wndAddPassenger();

                // Display the add passenger window
                wndAddPass.ShowDialog();

                // If not in add passenger mode, don't disable controls
                if (!bAddingPassenger)
                {
                    return;
                }

                // Disable all controls except those for choosing a seat
                cbChooseFlight.IsEnabled = false;
                cbChoosePassenger.IsEnabled = false;
                cmdAddPassenger.IsEnabled = false;
                cmdChangeSeat.IsEnabled = false;
                cmdDeletePassenger.IsEnabled = false;

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// If in normal mode it will display who is assigned to a seat if someone is.
        /// If in add passenger or change seat mode the selected seat will be assigned to the passenger.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Seat_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Label representing clicked seat
                Label seat = sender as Label;

                // Current flights ID
                int flightID = cbChooseFlight.SelectedIndex + 1;

                /*
                 * Isolate the seat number of the selected label
                 * Regular expression: 
                 * \d finds a digit between 0 and 9
                 * 1, 2 means it will find between 1 and 2 occurances of a digit
                 * $ finds matches at the end of the string
                 */
                Match seatMatch = Regex.Match(seat.Name, @"\d{1,2}$");

                // Isolate the value (seat number) of seatMatch
                string sSeatNumber = seatMatch.Value;

                // Seat is already taken, select the passenger belonging to the clicked seat
                if (flightManager.SeatTaken(sSeatNumber, flightID))
                {
                    // Iterate through passengers
                    foreach (clsPassenger passenger in cbChoosePassenger.ItemsSource)
                    {
                        if (sSeatNumber == passenger.seatNumber)
                        {
                            // Select chosen passenger in combo box
                            cbChoosePassenger.SelectedItem = passenger;
                            
                            // Display passengers seat number in seat number label
                            lblPassengersSeatNumber.Content = passenger.seatNumber;

                            // Do nothing else
                            return;
                        }
                    }
                }

                // Adding passenger mode is active. Create a new passenger.
                if (bAddingPassenger)
                {
                    // Create a passenger object 
                    clsPassenger passenger = new clsPassenger();
                    passenger.firstName = wndAddPass.sFirstName;
                    passenger.lastName = wndAddPass.sLastName;
                    passenger.seatNumber = sSeatNumber;

                    // Add passenger to database
                    passengerManager.AddPassenger(passenger, flightID);

                    // Exit adding passenger mode
                    bAddingPassenger = false;
                }

                // Changing seat mode is active Update passenger seat in passenger list and database
                if (bChangingSeat)
                {
                    // Get the selected passenger
                    clsPassenger passenger = (clsPassenger)cbChoosePassenger.SelectedItem;

                    // Set the passengers seat number 
                    passenger.seatNumber = sSeatNumber;

                    // Update the link table
                    passengerManager.UpdateSeat(passenger, sSeatNumber, flightID);//Int32.Parse(sSeatNumber));

                    // Exit changing seat mode
                    bChangingSeat = false;
                }

                UpdateFlightDetails(flightID);

                // Enable all disabled controls
                cbChooseFlight.IsEnabled = true;
                cbChoosePassenger.IsEnabled = true;
                cmdAddPassenger.IsEnabled = true;
                cmdChangeSeat.IsEnabled = true;
                cmdDeletePassenger.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }

        }

        /// <summary>
        /// Activate "Seat Changing Mode"
        /// Deactivate most controls so user must pick a seat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdChangeSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // No passenger is selected. Don't do anything
                if (cbChoosePassenger.SelectedItem == null)
                {
                    return;
                }

                // Enter seat changing mode
                bChangingSeat = true;

                // Disable all controls except choosing a seat
                cbChooseFlight.IsEnabled = false;
                cbChoosePassenger.IsEnabled = false;
                cmdAddPassenger.IsEnabled = false;
                cmdChangeSeat.IsEnabled = false;
                cmdDeletePassenger.IsEnabled = false;

            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Deletes the selected passenger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            // No passenger is selected. Don't do anything
            if (cbChoosePassenger.SelectedItem == null)
            {
                return;
            }

            // Get the selected person
            clsPassenger passenger = (clsPassenger)cbChoosePassenger.SelectedItem;

            // Get the flight ID
            int flightID = cbChooseFlight.SelectedIndex + 1;

            // A confirmation message string
            string sConfirmationMessage = $"Are you sure you would like to delete {passenger.firstName} {passenger.lastName} " +
                $"from flight {flightID}? This action cannot be undone.";

            // Stores the result of the confirmation message
            MessageBoxResult result = MessageBox.Show
                (sConfirmationMessage, "Delete Passenger", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Delete the passenger from the database
                passengerManager.DeletePassenger(passenger, flightID);

                UpdateFlightDetails(flightID);
            }
        }

        /// <summary>
        /// Highlights the selected passengers seat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChoosePassenger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // No passenger is selected. Do nothing.
            if (cbChoosePassenger.SelectedItem == null || cbChoosePassenger.SelectedIndex == -1)
            {
                return;
            }

            // ID of the current flight
            int flightID = cbChooseFlight.SelectedIndex + 1;

            // All passengers on the current flight
            List<clsPassenger> passengers = passengerManager.GetPassengers(flightID);

            // The currently selected passenger
            clsPassenger selectedPassenger = (clsPassenger)cbChoosePassenger.SelectedItem;

            // Reset flight details
            ResetSeats(flightID, ref passengers);

            // Set the selected passengers seat to selected (green)
            UpdateFlightDetails(flightID, selectedPassenger.seatNumber);
        }

        /// <summary>
        /// Marks the currently selected passengers seat to green
        /// </summary>
        /// <param name="flightID"></param>
        /// <param name="seatNumber"></param>
        private void UpdateFlightDetails(int flightID, string seatNumber)
        {
            if (flightID == 1)
            {
                // Iterate through seats
                foreach (Label seat in c767_Seats.Children)
                {
                        // Seat matches the selected passenger
                        if (seat.Name == ("Seat" + seatNumber))
                        {
                            // Mark Seat green (selected)
                            seat.Background = new SolidColorBrush(Colors.Green);
                            return;
                        }
                    }
            }
            else
            {
                // Iterate through seats
                foreach (Label seat in cA380_Seats.Children)
                {

                    // Seat matches the selected passenger
                    if (seat.Name == ("SeatA" + seatNumber))
                    {
                    // Mark Seat green (selected)
                    seat.Background = new SolidColorBrush(Colors.Green);
                    return;
                    }

                }
            }
        }
    }
}
