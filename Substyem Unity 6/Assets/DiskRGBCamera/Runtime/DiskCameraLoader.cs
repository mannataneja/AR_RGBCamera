using UnityEngine;
using UnityEngine.XR.Management;

namespace DiskCamera
{
    public class DiskCameraLoader : XRLoader
    {
        public override bool Initialize() => true;
        public override bool Start() => true;
        public override bool Stop() => true;
        public override bool Deinitialize() => true;

        // ✅ New requirement in Unity 6 / XR Management 4.5+
        public override T GetLoadedSubsystem<T>()
        {
            // Your loader doesn’t manage multiple subsystems,
            // so just return default.
            return default;
        }
    }
}
