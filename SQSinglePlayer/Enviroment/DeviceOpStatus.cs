using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQSinglePlayer.Enviroment
{
    class DeviceOpStatus
    {
        double TotalRamMemory = (double)DeviceStatus.DeviceTotalMemory / 1048576;

        public static bool IsInternetConnected()
        {
            if ((DeviceNetworkInformation.IsCellularDataEnabled == false) && (DeviceNetworkInformation.IsWiFiEnabled == false))
            {
                return false;
            }
            else if (DeviceNetworkInformation.IsNetworkAvailable == false)
            {
                return false;
            }

            return true;
        }
    }
}
