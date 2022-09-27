﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Climate.MVVM.Models
{
    internal class DeviceItem
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }

        public string ConnectionState { get; set; }
    }
}
