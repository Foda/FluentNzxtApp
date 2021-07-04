using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NzxtLib
{
    public interface INzxtEffectMode
    {
        string Name { get; }
        byte Mode { get; }
        int MinColors { get; }
        int MaxColors { get; }
        bool HasSpeedSetting { get; }
        bool HasColorModes { get; }
    }
}
