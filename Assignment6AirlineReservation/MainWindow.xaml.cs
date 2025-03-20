using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        clsDataAccess clsData;
        wndAddPassenger wndAddPass;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

                // Initialize a flight manager
                clsFlightManager flightManager = new clsFlightManager();

                // Load the choose flight combo box with available flights
                cbChooseFlight.ItemsSource = flightManager.GetFlights();

            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void cbChooseFlight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // This represents the flights id
                int selection = cbChooseFlight.SelectedIndex + 1;
                cbChoosePassenger.IsEnabled = true;
                gPassengerCommands.IsEnabled = true;

                clsPassengerManager passengerManager = new clsPassengerManager();
                
                // This list holds all passengers on the current flight
                List<clsPassenger> passengers = passengerManager.GetPassengers(selection);

                // Update the choose passenger combo box
                cbChoosePassenger.ItemsSource = passengers;
            
                // Updates the flight plane (left side of the screen)
                UpdateFlightPane(selection, ref passengers);
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Updates flight pane based on chosen flight
        /// </summary>
        /// <param name="flightID">The flight id of the chosen flight</param>
        private void UpdateFlightPane(int flightID, ref List<clsPassenger> passengers)
        {
            try
            {
                if (flightID == 1)
                {
                    // Display appropriate flight 
                    CanvasA380.Visibility = Visibility.Hidden;
                    Canvas767.Visibility = Visibility.Visible;
                    // Mark taken seats red
                    foreach (clsPassenger passenger in passengers)
                    {
                        foreach (Label seat in c767_Seats.Children)
                        {
                            if (seat.Name == ("Seat" + passenger.seatNumber))
                            {
                                seat.Background = new SolidColorBrush(Colors.Red);
                            }
                        }
                    }
                }
                else 
                {
                    // Display appropriate flight
                    Canvas767.Visibility = Visibility.Hidden;
                    CanvasA380.Visibility = Visibility.Visible;
                    // Mark taken seats red
                    foreach (clsPassenger passenger in passengers)
                    {
                        foreach (Label seat in cA380_Seats.Children)
                        {
                            if (seat.Name == ("SeatA" + passenger.seatNumber))
                            {
                                seat.Background = new SolidColorBrush(Colors.Red);
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

        private void cmdAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wndAddPass = new wndAddPassenger();
                wndAddPass.ShowDialog();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }
    }
}
