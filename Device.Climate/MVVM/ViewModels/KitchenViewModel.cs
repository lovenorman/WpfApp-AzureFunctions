using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Climate.MVVM.ViewModels
{
    internal class KitchenViewModel
    {
        public string Title { get; set; } = "Kitchen";
        public string TemperatureTitle { get; set; } = "Temperatur:";
        public string Temperature { get; set; } = "23";
        public string TemperatureScale { get; set; } = "°C";
        public string HumidityTitle { get; set; } = "Humidity:";
        public string Humidity { get; set; } = "33";
        public string HumidityScale { get; set; } = "%";

    }
}
