using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Custom/OverdrawMonitor")]
    public class OverdrawMonitorComponent : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter CountOverdrawRatio = new BoolParameter(true);
        public BoolParameter DisplayOverDrawResultOnScreen = new BoolParameter(false);
        public bool IsActive() => (bool)CountOverdrawRatio;
        public bool IsTileCompatible() => false;
    }
}