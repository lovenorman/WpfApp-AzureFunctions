using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Climate.MVVM.ViewModels
{
    internal class LivingroomViewModel
    {
        public string Title { get; set; } = "Living room";
        public string Temperature { get; set; } = "24";
        public string TemperatureScale { get; set; } = "°C";
        public string Humidity { get; set; } = "31";
        public string HumidityScale { get; set; } = "%";
    }
}
