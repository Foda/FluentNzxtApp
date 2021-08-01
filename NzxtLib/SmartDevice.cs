using RBGLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace NzxtLib
{
    public class SmartDevice : INzxtDevice, IDisposable
    {
        public readonly int ChannelCount = 2;

        private const int DEVICE_BUFFER_SIZE = 64;
        private const int HUE2_MAX_ACCESSORIES_IN_CHANNEL = 6;
        private const int MAX_EFFECT_COLORS = 8;
        private const int NZXTVendorId = 7793;
        private const int SmartDeviceV2ProductId = 8198;

        private HidDevice _device;
        public HidDevice Device => _device;

        public string Name => "Smart Device v2";
        public string RawName { get; private set; }

        public List<INzxtAccessory> LedAccessories { get; private set; }

        /// <summary>
        /// mval | mod3 | moving flag
        /// </summary>
        private List<Hue2EffectMode> _effectModes = new()
        {
            new Hue2EffectMode("Off",           new byte[3] { 0x00, 0x00, 0x00 }, 0, 0, -1, -1),
            new Hue2EffectMode("Fixed",         new byte[3] { 0x00, 0x00, 0x00 }, 1, 1, -1, -1),
            new Hue2EffectMode("Fading",        new byte[3] { 0x01, 0x00, 0x00 }, 1, 8, 0, 4),
            new Hue2EffectMode("Spectrum Wave", new byte[3] { 0x02, 0x00, 0x00 }, 0, 0, 0, 4),
            new Hue2EffectMode("Pulse",         new byte[3] { 0x06, 0x00, 0x00 }, 1, 8, 0, 4),
            new Hue2EffectMode("Breathing",     new byte[3] { 0x07, 0x00, 0x00 }, 1, 8, 0, 4),
        };

        public List<Hue2EffectMode> EffectModes => _effectModes;

        public SmartDevice()
        {
            LedAccessories = new List<INzxtAccessory>();
        }

        public async Task<bool> FindDevice()
        {
            if (_device != null)
            {
                _device.Dispose();
            }

            string selector = "System.Devices.InterfaceClassGuid:=\"{4D1E55B2-F16F-11CF-88CB-001111000030}\" AND " +
                              "System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True AND " +
                              $"System.DeviceInterface.Hid.VendorId:={NZXTVendorId} AND " +
                              $"System.DeviceInterface.Hid.ProductId:={SmartDeviceV2ProductId}";

            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Count == 0)
                return false;

            RawName = devices[0].Name;

            _device = await HidDevice.FromIdAsync(devices[0].Id, FileAccessMode.ReadWrite);

            if (_device != null)
            {
                try
                {
                    LedAccessories.Clear();

                    List<INzxtAccessory> accessories = await GetAccessories();

                    LedAccessories.AddRange(accessories);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        private async Task<List<INzxtAccessory>> GetAccessories()
        {
            Stopwatch sw = Stopwatch.StartNew();
            TaskCompletionSource<List<INzxtAccessory>> getAccessories = new();
            Windows.Foundation.TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs> reportEvent = null;

            reportEvent = (s, e) =>
            {
                HidInputReport inputReport = e.Report;

                byte[] data = new byte[DEVICE_BUFFER_SIZE];
                DataReader dataReader = DataReader.FromBuffer(inputReport.Data);
                dataReader.ReadBytes(data);

                List<INzxtAccessory> items = ParseChannelAccessories(data);
                if (items.Any() || sw.ElapsedMilliseconds > 6000)
                {
                    // If 2 seconds pass without a result, just return an empty list
                    getAccessories.SetResult(items);
                    _device.InputReportReceived -= reportEvent;
                }
            };

            _device.InputReportReceived += reportEvent;

            // Now make the request for what things are attached
            // Build the lighting info request
            byte[] req = new byte[DEVICE_BUFFER_SIZE];
            req[0] = 0x20;
            req[1] = 0x03;

            bool didWrite = await WriteToDevice(req);
            if (!didWrite)
            {
                throw new Exception("Failed to request LED status");
            }

            return await getAccessories.Task;
        }

        private List<INzxtAccessory> ParseChannelAccessories(byte[] data)
        {
            List<INzxtAccessory> accessories = new();
            for (int i = 0; i < ChannelCount; i++)
            {
                int offset = 0x0F + (6 * i);
                for (int a = 0; a < HUE2_MAX_ACCESSORIES_IN_CHANNEL; a++)
                {
                    byte accessoryId = data[offset + a];
                    if (accessoryId == 0)
                    {
                        break;
                    }

                    try
                    {
                        accessories.Add(Hue2Accessory.GetAccessoryFromId(accessoryId, i + 1));
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("Invalid device id!");
                    }
                }
            }

            return accessories;
        }

        private async Task<bool> WriteToDevice(byte[] buffer)
        {
            try
            {
                HidOutputReport outReport = _device.CreateOutputReport();

                DataWriter dataWriter = new DataWriter();
                dataWriter.WriteBytes(buffer);

                outReport.Data = dataWriter.DetachBuffer();

                await _device.SendOutputReportAsync(outReport);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public Task<bool> ApplyFixedColor(Color color)
        {
            return Apply(new List<Color> { color },
                EffectModes.FirstOrDefault(mode => mode.Name == "Fixed"));
        }

        public Task<bool> ApplyFixedColor(List<Color> colors)
        {
            return Apply(colors,
                EffectModes.FirstOrDefault(mode => mode.Name == "Fixed"));
        }

        public async Task<bool> Apply(List<Color> colors, EffectMode effect)
        {
            if (_device == null)
            {
                return false;
            }

            if (colors.Count > MAX_EFFECT_COLORS)
            {
                throw new ArgumentException($"Color count cannot be greater than {MAX_EFFECT_COLORS}");
            }

            if (colors.Count < effect.MinColors || colors.Count > effect.MaxColors)
            {
                throw new ArgumentException(
                    $"Invalid colors count for effect '{effect.Name}'. Must be greater than {effect.MinColors} and less than {effect.MaxColors}");
            }

            for (int channel = 0; channel < ChannelCount; channel++)
            {
                // Only write to the channel if there's an accessory on it
                if (!LedAccessories.Any(accessory => accessory.Channel == channel))
                {
                    continue;
                }

                // Effect packet
                byte[] toWrite = new byte[DEVICE_BUFFER_SIZE];
                toWrite[0x00] = 0x28;
                toWrite[0x01] = 0x03;
                toWrite[0x02] = (byte)(channel + 1);   // channel id
                toWrite[0x03] = 0x28;      // ???

                // Effect mode
                toWrite[0x04] = effect.Mode;

                // Speed
                toWrite[0x05] = (byte)effect.Speed;

                // Direction
                toWrite[0x06] = 0;
                toWrite[0x07] = 0;   // Backward flag (0,1)

                // Color count
                toWrite[0x08] = (byte)colors.Count;

                // Colors
                int pixelIdx = 0;
                for (int i = 0; i < colors.Count; i++)
                {
                    pixelIdx = 10 + (i * 3); // Start index is 10
                    toWrite[pixelIdx + 0x00] = colors[i].G;
                    toWrite[pixelIdx + 0x01] = colors[i].R;
                    toWrite[pixelIdx + 0x02] = colors[i].B;
                }

                bool result = await WriteToDevice(toWrite);
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        public void Dispose()
        {
            _device?.Dispose();
        }
    }
}
