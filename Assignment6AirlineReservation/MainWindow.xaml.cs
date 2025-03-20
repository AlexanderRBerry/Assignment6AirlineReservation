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

                DataSet ds = new DataSet();
                //Should probably not have SQL statements behind the UI
                string sSQL = "SELECT Flight_ID, Flight_Number, Aircraft_Type FROM FLIGHT";
                int iRet = 0;
                clsData = new clsDataAccess();

                //This should probably be in a new class.  Would be nice if this new class
                //returned a list of Flight objects that was then bound to the combo box
                //Also should show the flight number and aircraft type together
                ds = clsData.ExecuteSQLStatement(sSQL, ref iRet);

                //Should probably bind a list of flights to the combo box
                for(int i = 0; i < iRet; i++)
                {
                    cbChooseFlight.Items.Add(ds.Tables[0].Rows[i][0]);
                }
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
                string selection = cbChooseFlight.SelectedItem.ToString();  //This is wrong, if a list of flights was in the combo box, then could get the selected flight in an object
                cbChoosePassenger.IsEnabled = true;
                gPassengerCommands.IsEnabled = true;
                DataSet ds = new DataSet();                
                int iRet = 0;

                //Should be using a flight object to get the flight ID here
                if (selection == "1")
                {
                    CanvasA380.Visibility = Visibility.Hidden;
                    Canvas767.Visibility = Visibility.Visible;
                }
                else
                {
                    Canvas767.Visibility = Visibility.Hidden;
                    CanvasA380.Visibility = Visibility.Visible;
                }

                //I think this should be in a new class to hold SQL statments
                string sSQL = "SELECT Passenger.Passenger_ID, First_Name, Last_Name, FPL.Seat_Number " +
                              "FROM Passenger, Flight_Passenger_Link FPL " +
                              "WHERE Passenger.Passenger_ID = FPL.Passenger_ID AND " +
                              "Flight_ID = " + cbChooseFlight.SelectedItem.ToString();//If the cbChooseFlight was bound to a list of Flights, the selected object would have the flight ID
                //Probably put in a new class
                ds = clsData.ExecuteSQLStatement(sSQL, ref iRet);

                cbChoosePassenger.Items.Clear();//Don't need if assigning a list of passengers to the combo box

                //Would be nice if code from another class executed the SQL above, added each passenger into a Passenger object,
                //then into a list of Passengers to be returned and bound to the combo box
                for (int i = 0; i < iRet; i++)
                {
                    cbChoosePassenger.Items.Add(ds.Tables[0].Rows[i][1] + " " + ds.Tables[0].Rows[i][2]);
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
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
