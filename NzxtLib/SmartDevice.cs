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
        private const int TotalLightChannel = 2;
        private const int DeviceBufferSize = 65;

        private HidDevice _device;
        public HidDevice Device => _device;

        private Dictionary<int, byte> _channelIndex;

        private const int NZXTVendorId = 7793;
        private const int SmartDeviceV2ProductId = 8198;

        /// <summary>
        /// mval | mod3 | moving flag | min colors | max colors
        /// </summary>
        private List<Tuple<string, byte[]>> _colorModes = new List<Tuple<string, byte[]>>
        {
            new Tuple<string, byte[]>("Off",           new byte[5] { 0x00, 0x00, 0x00, 0, 0 }),
            new Tuple<string, byte[]>("Fixed",         new byte[5] { 0x00, 0x00, 0x00, 1, 1 }),
            new Tuple<string, byte[]>("Fading",        new byte[5] { 0x01, 0x00, 0x00, 1, 8 }),
            new Tuple<string, byte[]>("Spectrum Wave", new byte[5] { 0x02, 0x00, 0x00, 0, 0 }),
            new Tuple<string, byte[]>("Pulse",         new byte[5] { 0x06, 0x00, 0x00, 1, 8 }),
            new Tuple<string, byte[]>("Breathing",     new byte[5] { 0x07, 0x00, 0x00, 1, 8 }),
        };

        public SmartDevice()
        {
        }

        public bool FindDevice()
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
                for (int i = 0; i < TotalLightChannel; i++)
                {
                    _channelIndex.Add(i + 1, (byte)Math.Pow(2.0, i));
                }

                return true;
            }
            return false;
        }

        public Task<bool> ApplyFixedColor(Color color)
        {
            if (_device == null)
            {
                return Task.FromResult(false);
            }

            List<Color> colorList = new List<Color>
            {
                color
            };

            byte[] colorMode = _colorModes[1].Item2;

            byte[] toWrite = new byte[DeviceBufferSize];
            toWrite[0] = 40;
            toWrite[1] = 3;
            toWrite[2] = 1;   // channel id (channel 1)
            toWrite[3] = 0;   // Always 0
            toWrite[4] = colorMode[4]; // Effect mode

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

            return _device.WriteAsync(toWrite);
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
