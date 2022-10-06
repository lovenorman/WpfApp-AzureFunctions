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
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Shared;

namespace ConnectingDevice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {                                          //https://sysytemutvecklingfunctionapp.azurewebsites.net
        private readonly string _connectUrl = "http://localhost:7225/api/devices/connect";
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
            SetupAsync().ConfigureAwait(false);
            SendMessageAsync().ConfigureAwait(false);
        }

        public async Task SetupAsync()
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

        private async Task SendMessageAsync()
        {
            while(true)
            {
                if(_connected)
                {
                    if(_lightState != _lightPreviusState)
                    {
                        _lightPreviusState = _lightState;

                        //d2c
                        var json = JsonConvert.SerializeObject(new {lightState = _lightState});
                        var message = new Message(Encoding.UTF8.GetBytes(json));
                            message.Properties.Add("deviceName", deviceInfo.DeviceName);
                            message.Properties.Add("deviceType", deviceInfo.DeviceType);
                            message.Properties.Add("location", deviceInfo.Location);
                            message.Properties.Add("owner", deviceInfo.Owner);

                        await _deviceClient.SendEventAsync(message);
                        tbStateMessage.Text = $"Message sent at {DateTime.Now}.";

                        //device twin (reported)
                        var twinCollection = new TwinCollection();
                        twinCollection["lightState"] = _lightState;
                        await _deviceClient.UpdateReportedPropertiesAsync(twinCollection);
                    }
                }
                await Task.Delay(_interval);
            }
        }

        private void btnOnOff_Click(object sender, RoutedEventArgs e)
        {
            _lightState = !_lightState;

            if (_lightState)
            {
                btnOnOff.Content = "Turn Off";
            }
            else
            {
                btnOnOff.Content = "Turn On";
            }

        }
    }
}
