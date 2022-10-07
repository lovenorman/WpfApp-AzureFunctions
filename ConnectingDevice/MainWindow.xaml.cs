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
using System.Diagnostics;

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
            tbStateMessage.Text = "Initializing device.Please wait...";

            using IDbConnection conn = new SqlConnection(_connectionString);//Ändra till att söka i CosmosDB?
            _deviceId = await conn.QueryFirstOrDefaultAsync<string>("SELECT DeviceId FROM DeviceInfo");
            if (string.IsNullOrEmpty(_deviceId))
            {
                tbStateMessage.Text = "Generating new DeviceId";//Genererar ett nytt deviceId.
                _deviceId = Guid.NewGuid().ToString();
                await conn.ExecuteAsync("INSERT INTO DeviceInfo(DeviceId,DeviceName,DeviceType,Location,Owner) Values (@DeviceId, @DeviceName, @DeviceType, @Location, @Owner)", new { DeviceId = _deviceId, DeviceName = "WPF Device", DeviceType = "light", Location = "kitchen", Owner = "Love Norman" });//Insert values på första lediga plats i tabellen DeviceInfo
            }

            var device_ConnectionString = await conn.QueryFirstOrDefaultAsync<string>("SELECT ConnectionString FROM DeviceInfo WHERE DeviceId = @DeviceId", new {DeviceId = _deviceId});
            
            if (string.IsNullOrEmpty(device_ConnectionString))
            {
                tbStateMessage.Text = "Intializing connectionstring.Please wait...";

                using var http = new HttpClient();

                try
                {
                    await Task.Delay(3000);
                    var result = await http.PostAsJsonAsync(_connectUrl, new { deviceId = _deviceId });//Behöver jag sätta deviceId här? (deviceId är property i twin)
                    device_ConnectionString = await result.Content.ReadAsStringAsync();
                    await conn.ExecuteAsync("UPDATE DeviceInfo SET ConnectionString = @ConnectionsString WHERE DeviceId = @DeviceId", new { DeviceId = _deviceId, ConnectionString = device_ConnectionString });
                }
                catch (Exception ex)
                {

                    Debug.WriteLine(ex.Message);
                }
                
            }
            
            _deviceClient = DeviceClient.CreateFromConnectionString(device_ConnectionString);//Vi skapar en deviceClient med connectionstringen som vi fått från DB el APIet

            tbStateMessage.Text = "Updating Twin Properties.Please wait...";

            _deviceInfo = await conn.QueryFirstOrDefaultAsync<DeviceInfo>("SELECT * FROM DeviceInfo WHERE DeviceId = @DeviceId", new { DeviceId = _deviceId });//Letar efter första Devicen som matchar och returnar en DeviceInfo

            var twinCollection = new TwinCollection();      
            twinCollection["deviceName"] = _deviceInfo.DeviceName; //Sätter twinnens propertys till de värden som hämtats fråm DB
            twinCollection["deviceType"] = _deviceInfo.DeviceType;
            twinCollection["location"] = _deviceInfo.Location;
            twinCollection["owner"] = _deviceInfo.Owner;
            twinCollection["lightState"] = _lightState;

            await _deviceClient.UpdateReportedPropertiesAsync(twinCollection);//Uppdaterar twinnen 

            _connected = true;//så att vi kommer in i rätt if sats nedan
            tbStateMessage.Text = "Device connected";
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
                            message.Properties.Add("deviceName", _deviceInfo.DeviceName);
                            message.Properties.Add("deviceType", _deviceInfo.DeviceType);
                            message.Properties.Add("location", _deviceInfo.Location);
                            message.Properties.Add("owner", _deviceInfo.Owner);

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
