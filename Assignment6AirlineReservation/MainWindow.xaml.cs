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
        //clsDataAccess clsData;
        wndAddPassenger wndAddPass;

        clsFlightManager flightManager;
        clsPassengerManager passengerManager;

        // Determines if the program is in "Add Passenger Mode"
        public static bool bAddingPassenger;

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
                passengerManager = new clsPassengerManager();

                // This list holds all passengers on the current flight
                List<clsPassenger> passengers = passengerManager.GetPassengers(flightID);

                // Bind passengers to the passenger combo box
                cbChoosePassenger.ItemsSource = passengers;

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
                            //TODO: This if won't ever be true. Remove
                            if (seat.Background == new SolidColorBrush(Colors.Red))
                            {
                                break;
                            }

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
            catch (Exception ex)
            {
                //Throw an exception
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
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
                wndAddPass.ShowDialog();

                // If not in add passenger mode, don't disable controls
                if (!bAddingPassenger)
                {
                    return;
                }

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


        private void cmdChangeSeat_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Seat_Click(object sender, MouseButtonEventArgs e)
        {
            // If not in Add passenger mode return
            // TODO: Display the selected seats passenger if applicable
            if (!bAddingPassenger)
            {
                return;
            }

            Label seat = sender as Label;
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

            // Seat is already taken do nothing
            if (flightManager.SeatTaken(sSeatNumber, flightID))
            {
                return;
            }

            // Create a passenger object 
            clsPassenger passenger = new clsPassenger();
            passenger.firstName = wndAddPass.sFirstName;
            passenger.lastName = wndAddPass.sLastName;
            passenger.seatNumber = sSeatNumber;

            // Add passenger to database
            passengerManager.AddPassenger(passenger, flightID);

            
            UpdateFlightDetails(flightID);

            // Enable all disabled controls
            cbChooseFlight.IsEnabled = true;
            cbChoosePassenger.IsEnabled = true;
            cmdAddPassenger.IsEnabled = true;
            cmdChangeSeat.IsEnabled = true;
            cmdDeletePassenger.IsEnabled = true;

            // Reset boolean for adding a passenger
            bAddingPassenger = false;
        }

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

            // Delete the passenger from the database
            passengerManager.DeletePassenger(passenger, flightID);

            UpdateFlightDetails(flightID);
        }
    }
}
