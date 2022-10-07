using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ConnectFan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _connectUrl = "http://localhost:7225/api/devices/connect";
        private readonly string _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\love_\\source\\repos\\Inlämningsuppgift\\ConnectingDevice\\Data\\device_DB.mdf;Integrated Security=True;Connect Timeout=30";

        private DeviceClient _deviceClient;
        
        private string _deviceId = "";
        private bool _lightState = false;
        private bool _lightPreviusState = false;
        private bool _connected = false;
        private int _interval = 1000;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
