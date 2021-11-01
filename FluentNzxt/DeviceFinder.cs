using FluentNzxt.ViewModel;
using NzxtLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;

namespace FluentNzxt
{
    public class DeviceFinder
    {
        private const int NZXTVendorId = 7793;

        public static async Task<List<IDeviceViewModel>> FindDevices()
        {
            List<IDeviceViewModel> foundDevices = new();

            string selector = "System.Devices.InterfaceClassGuid:=\"{4D1E55B2-F16F-11CF-88CB-001111000030}\" AND " +
                              "System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True AND " +
                             $"System.DeviceInterface.Hid.VendorId:={NZXTVendorId}";

            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

            foreach (DeviceInformation deviceInfo in devices)
            {
                using HidDevice device = await HidDevice.FromIdAsync(devices[0].Id, FileAccessMode.Read);
                if (SmartDevice.ProductIds.Any(id => id == device.ProductId))
                {
                    foundDevices.Add(
                        new SmartDeviceViewModel(new SmartDevice(deviceInfo)));
                }
            }

            return foundDevices;
        }
    }
}
