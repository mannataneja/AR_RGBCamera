using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public sealed class CameraSubsystem : XRCameraSubsystem
{
    // Provider your descriptor will instantiate
    public sealed class ProviderImpl : Provider
    {
        Material _material;

        // Minimal: return a valid frame using 4.2.x constructor
        public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame frame)
        {
            frame = new XRCameraFrame();
            return true;
        }

        // Many samples provide a material; simple unlit is fine for a stub
        public override Material cameraMaterial =>
            _material != null ? _material : (_material = new Material(Shader.Find("Unlit/Texture")));
    }

    // Register a descriptor that points to your Provider
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Register()
    {
        var cinfo = new XRCameraSubsystemCinfo
        {
            id = "Simple-Camera",
            providerType = typeof(ProviderImpl),
            subsystemTypeOverride = typeof(CameraSubsystem),

            // Capabilities (keep minimal/true)
            supportsAverageBrightness = true,
            supportsAverageColorTemperature = true,
            supportsColorCorrection = true,
            supportsCameraConfigurations = false,
            supportsCameraImage = false,
            supportsAverageIntensityInLumens = false
        };

        if (!XRCameraSubsystem.Register(cinfo))
            Debug.LogError("[CameraSubsystem] Register failed");
        else
            Debug.Log("[CameraSubsystem] Registered 'Simple-Camera'");
    }
}
