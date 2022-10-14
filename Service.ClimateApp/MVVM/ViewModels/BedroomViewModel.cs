using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices;
using Service.ClimateApp.MVVM.Cores;
using Service.ClimateApp.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Service.ClimateApp.MVVM.ViewModels
{
    internal class BedroomViewModel : ObservableObject
    {
        private DispatcherTimer timer;
        private ObservableCollection<DeviceItem> _deviceItems;
        private List<DeviceItem> _tempList;
        private readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString("HostName=SystemutvecklingIotHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=tbFZhgNsePCnDBO9HhjsckNF8gPWKvV9nnfFXG/6RO0=");


        public BedroomViewModel()
        {
            _tempList = new List<DeviceItem>();
            _deviceItems = new ObservableCollection<DeviceItem>();

            PopulateDeviceItemsAsync().ConfigureAwait(false);
            SetInterval(TimeSpan.FromSeconds(3));
        }

        public string Title { get; set; } = "Bedroom";
        public IEnumerable<DeviceItem> DeviceItems => _deviceItems;


        private void SetInterval(TimeSpan interval)
        {
            timer = new DispatcherTimer()
            {
                Interval = interval
            };

            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }

        private async void timer_tick(object sender, EventArgs e)
        {
            await PopulateDeviceItemsAsync();
            await UpdateDeviceItemsAsync();
        }



        private async Task UpdateDeviceItemsAsync()
        {
            _tempList.Clear();

            foreach (var item in _deviceItems)
            {
                var device = await registryManager.GetDeviceAsync(item.DeviceId);
                if (device == null)
                    _tempList.Add(item);
            }

            foreach (var item in _tempList)
            {
                _deviceItems.Remove(item);
            }
        }

        private async Task PopulateDeviceItemsAsync()
        {
            var result = registryManager.CreateQuery("select * from devices where properties.reported.location = 'bedroom'");

            if (result.HasMoreResults)//Om det finns något i result...
            {
                foreach (Twin twin in await result.GetNextAsTwinAsync())//...för varje twin i resultlistan...
                {
                    var device = _deviceItems.FirstOrDefault(x => x.DeviceId == twin.DeviceId);//Ta den första där DeviceId == twinnens deviceId

                    if (device == null)//Om det inte finns någon device som har samma Id som device twinnen
                    {
                        device = new DeviceItem//Skapa en ny device
                        {
                            DeviceId = twin.DeviceId,//Sätt deviceId till deviceTwinnens Id
                        };

                        try { device.DeviceName = twin.Properties.Reported["deviceName"]; }
                        catch { device.DeviceName = device.DeviceId; }//Om den inte har ett Name sätt Id till Name
                        try
                        {
                            device.DeviceType = twin.Properties.Reported["deviceType"];
                        }
                        catch { }

                        switch (device.DeviceType.ToLower())
                        {
                            case "fan":
                                device.IconActive = "\uf863";
                                device.IconInActive = "\uf863";
                                device.StateActive = "On";
                                device.StateInActive = "Off";
                                break;

                            case "light":
                                device.IconActive = "\uf672";
                                device.IconInActive = "\uf0eb";
                                device.StateActive = "On";
                                device.StateInActive = "Off";
                                break;

                            default:
                                device.IconActive = "\uf2db";
                                device.IconInActive = "\uf2db";
                                device.StateActive = "Enable";
                                device.StateInActive = "Disable";
                                break;
                        }
                        _deviceItems.Add(device);

                    }
                    else { }
                }
            }
            else
            {
                _deviceItems.Clear();
            }
        }
    }
}
