using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGLib
{
    public abstract record EffectMode(string Name, byte[] Data, int MinColors, int MaxColors, int MinSpeed, int MaxSpeed)
    {
        public abstract byte Mode { get; }
        public bool HasColorModes => MaxColors > 0;
        public bool HasSpeedSetting => MinSpeed > -1;
        public int Speed { get; set; }
    }
}
