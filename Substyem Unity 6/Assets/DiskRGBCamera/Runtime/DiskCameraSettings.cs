using UnityEngine;
using UnityEngine.XR.Management;

[XRConfigurationData("Disk Camera", "DiskCamera.DiskCameraSettings")]
[CreateAssetMenu(menuName = "Disk Camera/Settings", fileName = "DiskCameraSettings")]
public class DiskCameraSettings : ScriptableObject
{
    public string imageFolder = "DiskCam";        // folder path (absolute or under StreamingAssets)
    public float targetFPS = 30f;                 // ✅ renamed from fps
    public bool loopSequence = true;
    public Vector2Int imageSize = new(1920, 1080);
    public Vector2 focal = new(1000, 1000);
    public Vector2 principal = new(960, 540);
}
