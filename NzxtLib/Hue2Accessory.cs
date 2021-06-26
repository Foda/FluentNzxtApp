using System;
using System.Drawing;

namespace NzxtLib
{
    public record Hue2Accessory(string Name, byte Id, int LEDCount)
    {
        public static Hue2Accessory GetAccessoryFromId(byte id)
        {
            return id switch
            {
                0x01 => new Hue2Accessory("HUE+ LED Strip", 0x01, 1),
                0x02 => new Hue2Accessory("AER RGB 1", 0x02, 1),
                0x04 => new Hue2Accessory("HUE 2 LED Strip 300 mm", 0x04, 10),
                0x05 => new Hue2Accessory("HUE 2 LED Strip 250 mm", 0x05, 8),
                0x06 => new Hue2Accessory("HUE 2 LED Strip 200 mm", 0x06, 6),
                _ => throw new ArgumentException(nameof(id), "Unknown device id"),
            };
        }
    }

    public record Hue2EffectMode(string Name, byte[] Data);
}