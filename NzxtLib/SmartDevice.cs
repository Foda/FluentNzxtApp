﻿using HidLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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

        private Dictionary<int, byte> _channelIndex = new();

        private HidDevice _device;
        public HidDevice Device => _device;

        public string Name => "Smart Device v2";

        public List<INzxtAccessory> LedAccessories { get; private set; }

        /// <summary>
        /// mval | mod3 | moving flag
        /// </summary>
        private List<INzxtEffectMode> _effectModes = new()
        {
            new Hue2EffectMode("Off",           new byte[3] { 0x00, 0x00, 0x00 }, 0, 0 , false),
            new Hue2EffectMode("Fixed",         new byte[3] { 0x00, 0x00, 0x00 }, 1, 1 , false),
            new Hue2EffectMode("Fading",        new byte[3] { 0x01, 0x00, 0x00 }, 1, 8 , true),
            new Hue2EffectMode("Spectrum Wave", new byte[3] { 0x02, 0x00, 0x00 }, 0, 0 , true),
            new Hue2EffectMode("Pulse",         new byte[3] { 0x06, 0x00, 0x00 }, 1, 8 , true),
            new Hue2EffectMode("Breathing",     new byte[3] { 0x07, 0x00, 0x00 }, 1, 8 , true),
        };

        public List<INzxtEffectMode> EffectModes => _effectModes;

        public List<Hue2EffectSpeed> EffectSpeeds => new()
        {
            new Hue2EffectSpeed("Slowest", 0x00),
            new Hue2EffectSpeed("Slow", 0x01),
            new Hue2EffectSpeed("Normal", 0x02),
            new Hue2EffectSpeed("Fast", 0x03),
            new Hue2EffectSpeed("Fastest", 0x04),
        };

        public SmartDevice()
        {
            LedAccessories = new List<INzxtAccessory>();
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

                _channelIndex.Clear();
                for (int i = 0; i < ChannelCount; i++)
                {
                    _channelIndex.Add(i + 1, (byte)Math.Pow(2.0, i));
                }

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
            List<INzxtAccessory> accessories = new();

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
                        accessories.Add(Hue2Accessory.GetAccessoryFromId(accessoryId));
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("Invalid device id!");
                    }
                }
            }

            return accessories;
        }

        public Task<bool> ApplyFixedColor(Color color)
        {
            return Apply(1, new List<Color> { color }, 
                EffectModes.FirstOrDefault(mode => mode.Name == "Fixed"),
                EffectSpeeds.FirstOrDefault(speed => speed.Name == "Normal"));
        }

        public Task<bool> ApplyFixedColor(List<Color> colors)
        {
            return Apply(1, colors,
                EffectModes.FirstOrDefault(mode => mode.Name == "Fixed"),
                EffectSpeeds.FirstOrDefault(speed => speed.Name == "Normal"));
        }

        public Task<bool> Apply(byte channel, List<Color> colors, INzxtEffectMode effect, Hue2EffectSpeed speed)
        {
            if (_device == null)
            {
                return Task.FromResult(false);
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

            byte[] toWrite = new byte[DEVICE_BUFFER_SIZE];

            // Effect packet
            toWrite[0x00] = 0x28;
            toWrite[0x01] = 0x03;
            toWrite[0x02] = channel;   // channel id
            toWrite[0x03] = 0x28;      // ???

            // Effect mode
            toWrite[0x04] = effect.Mode;

            // Speed
            toWrite[0x05] = speed.Value;

            // Direction
            toWrite[0x06] = 0;
            toWrite[0x07] = 0;   // Backward flag (0,1)

            // Color count
            toWrite[0x08] = (byte)colors.Count;

            int pixelIdx = 0;
            for (int i = 0; i < colors.Count; i++)
            {
                pixelIdx = 10 + (i * 3); // Start index is 10
                toWrite[pixelIdx + 0x00] = colors[i].G;
                toWrite[pixelIdx + 0x01] = colors[i].R;
                toWrite[pixelIdx + 0x02] = colors[i].B;
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
