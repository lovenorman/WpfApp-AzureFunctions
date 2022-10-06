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
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using ConnectingDevice.Models;

namespace ConnectingDevice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _connectUrl = "http://localhost:7225/api/devices/connect";//URL som fås från APIet (vilket jag inte har än)
        private readonly string _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\love_\\source\\repos\\Inlämningsuppgift\\ConnectingDevice\\Data\\device_DB.mdf;Integrated Security=True;Connect Timeout=30";

        private DeviceClient _deviceClient;
        private DeviceInfo _deviceInfo;

        private string _deviceId = "";
        private bool _lightState = false;
        private bool _lightPreviusState = false;
        private bool _connected = false;
        private int _interval = 1000;

        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }

        public async Task Setup()
        {
            using IDbConnection conn = new SqlConnection(_connectionString);
            var deviceId = await conn.QueryFirstOrDefaultAsync<string>("SELECT DeviceId FROM DeviceInfo");
            if (string.IsNullOrEmpty(deviceId))//Om id är null, skapa nytt Guid, gör om till string
            {
                deviceId = Guid.NewGuid().ToString();
                await conn.ExecuteAsync("INSERT INTO DeviceInfo (DeviceId) VALUES (@DeviceId)", new { DeviceId = deviceId });
            }

            var device_ConnectionString = await conn.QueryFirstOrDefaultAsync<string>("SELECT ConnectionString FROM DeviceInfo WHERE DeviceId = @DeviceId", new {DeviceId = deviceId});
            if (string.IsNullOrEmpty(device_ConnectionString))//Om null, behöver vi göra en förfrågan till APIet
            {
                using var http = new HttpClient();
                var result = await http.PostAsJsonAsync(_connectUrl, new { deviceId });
                device_ConnectionString = await result.Content.ReadAsStringAsync();
                await conn.ExecuteAsync("UPDATE DeviceInfo SET ConnectionString = @ConnectionsString WHERE DeviceId = @DeviceId", new { DeviceId = deviceId, ConnectionString = device_ConnectionString });
            }
            
            _deviceClient = DeviceClient.CreateFromConnectionString(device_ConnectionString);//Vi skapar en deviceClient med connectionstringen som vi fått från DB el APIet
            
        }

        private void Loop()
        {

        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
