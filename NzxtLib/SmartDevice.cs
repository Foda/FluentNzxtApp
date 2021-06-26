using HidLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace NzxtLib
{
    public class SmartDevice : IDisposable
    {
        public readonly int ChannelCount = 2;
        private readonly int DeviceBufferSize = 65;

        private const int HUE2_MAX_ACCESSORIES_IN_CHANNEL = 6;

        private HidDevice _device;
        public HidDevice Device => _device;

        public string Name => "Smart Device v2";

        private Dictionary<int, byte> _channelIndex;

        private const int NZXTVendorId = 7793;
        private const int SmartDeviceV2ProductId = 8198;

        public List<Hue2Accessory> LedAccessories { get; private set; }

        /// <summary>
        /// mval | mod3 | moving flag | min colors | max colors
        /// </summary>
        private List<Hue2EffectMode> _effectModes = new()
        {
            new Hue2EffectMode("Off",           new byte[5] { 0x00, 0x00, 0x00, 0, 0 }),
            new Hue2EffectMode("Fixed",         new byte[5] { 0x00, 0x00, 0x00, 1, 1 }),
            new Hue2EffectMode("Fading",        new byte[5] { 0x01, 0x00, 0x00, 1, 8 }),
            new Hue2EffectMode("Spectrum Wave", new byte[5] { 0x02, 0x00, 0x00, 0, 0 }),
            new Hue2EffectMode("Pulse",         new byte[5] { 0x06, 0x00, 0x00, 1, 8 }),
            new Hue2EffectMode("Breathing",     new byte[5] { 0x07, 0x00, 0x00, 1, 8 }),
        };

        public List<Hue2EffectMode> EffectModes => _effectModes;

        public SmartDevice()
        {
            LedAccessories = new List<Hue2Accessory>();
        }

        public async Task<bool> FindDevice()
        {
            _device = HidDevices.Enumerate(NZXTVendorId)
                .ToList()
                .FirstOrDefault(device =>
                    device.Attributes.ProductId == SmartDeviceV2ProductId &&
                    device.Attributes.VendorId == NZXTVendorId);

            if (_device != null)
            {
                _device.OpenDevice();

                _channelIndex = new Dictionary<int, byte>();
                for (int i = 0; i < ChannelCount; i++)
                {
                    _channelIndex.Add(i + 1, (byte)Math.Pow(2.0, i));
                }

                try
                {
                    await ReadStatus();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        private async Task ReadStatus()
        {
            LedAccessories.Clear();

            // Request lighting info
            bool didWrite = await Task.Run(() => _device.Write(new byte[2] { 0x20, 0x03 }));
            if (!didWrite)
            {
                throw new Exception("Failed to request LED status");
            }

            HidDeviceData data = await Task.Run(() => _device.Read());
            int channelCount = data.Data[14];
            int offset = 15;
            
            for (int i = 0; i < channelCount; i++)
            {
                for (int a = 0; a < HUE2_MAX_ACCESSORIES_IN_CHANNEL; a++)
                {
                    byte accessoryId = data.Data[offset + i * HUE2_MAX_ACCESSORIES_IN_CHANNEL + a];
                    if (accessoryId == 0)
                    {
                        break;
                    }

                    try
                    {
                        LedAccessories.Add(Hue2Accessory.GetAccessoryFromId(accessoryId));
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("Invalid device id!");
                    }
                }
            }
        }

        public Task<bool> ApplyFixedColor(Color color)
        {
            return Apply(color, EffectModes.FirstOrDefault(mode => mode.Name == "Fixed"));
        }

        public Task<bool> Apply(Color color, Hue2EffectMode effect)
        {
            if (_device == null)
            {
                return Task.FromResult(false);
            }

            List<Color> colorList = new()
            {
                color
            };

            byte[] toWrite = new byte[DeviceBufferSize];
            toWrite[0] = 40;
            toWrite[1] = 3;
            toWrite[2] = 1;   // channel id (channel 1)
            toWrite[3] = 0;   // Always 0
            toWrite[4] = effect.Data[4]; // Effect mode

            toWrite[6] = 0;   // Moving flag
            toWrite[7] = 0;   // Backward flag (0,1)
            toWrite[8] = (byte)colorList.Count;
            toWrite[9] = 0;   //LED group size for alternating modes

            int idx = 10;
            for (int i = 0; i < colorList.Count; i++)
            {
                toWrite[idx] = colorList[i].G;
                toWrite[idx + 1] = colorList[i].R;
                toWrite[idx + 2] = colorList[i].B;

                idx += 3;
            }

            // Note: we can't use WriteAsync because it's not supported in .NET 5 (library issue)
            return Task.Run(() => _device.Write(toWrite));
        }

        public void Dispose()
        {
            _device?.Dispose();
        }
    }
}
