using System;
using System.Drawing;

namespace NzxtLib
{
    public record Hue2Accessory(string Name, byte Id, int LEDCount, int Channel) : INzxtAccessory
    {
        public static INzxtAccessory GetAccessoryFromId(byte id, int channel)
        {
            return id switch
            {
                0x01 => new Hue2Accessory("Hue+ LED Strip", 0x01, 10, channel),
                0x02 => new Hue2Accessory("Aer 1 Fan", 0x02, 8, channel),
                0x04 => new Hue2Accessory("Hue 2 LED Strip 300 mm", 0x04, 10, channel),
                0x05 => new Hue2Accessory("Hue 2 LED Strip 250 mm", 0x05, 8, channel),
                0x06 => new Hue2Accessory("Hue 2 LED Strip 200 mm", 0x06, 6, channel),
                0x0A => new Hue2Accessory("Hue 2 Underglow 200 mm", 0x0A, 10, channel),
                0x0B => new Hue2Accessory("Aer 2 fan 120 mm", 0x0B, 8, channel),
                0x0C => new Hue2Accessory("Aer 2 fan 140 mm", 0x0C, 8, channel),
                0x10 => new Hue2Accessory("Kraken X3 ring", 0x10, 8, channel),
                0x11 => new Hue2Accessory("Kraken X3 logo", 0x11, 1, channel),
                0x08 => new Hue2Accessory("Hue 2 Cable Comb", 0x08, 14, channel),
                _ => throw new ArgumentException(nameof(id), "Unknown device id"),
            };
        }
    }

    public record Hue2EffectSpeed(string Name, byte Value);

    public record Hue2EffectMode(string Name, byte[] Data, int MinColors, int MaxColors, bool HasSpeedSetting) : INzxtEffectMode
    {
        public byte Mode => Data[0];
        public bool HasColorModes => MinColors > 0;
    }
}