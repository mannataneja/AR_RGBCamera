using UnityEngine;
using UnityEngine.XR.Management;

[XRConfigurationData("Disk Camera", "DiskCamera.DiskCameraSettings")]
[CreateAssetMenu(menuName = "DiskCameraSettings")]
public class DiskCameraSettings : ScriptableObject
{
    public string relativeFolder = "DiskCam"; // under StreamingAssets
    public float fps = 30f;
    public bool loop = true;
    public Vector2Int imageSize = new Vector2Int(1920, 1080);
    public Vector2 focal = new Vector2(1000, 1000);   // fx, fy (pixels)
    public Vector2 principal = new Vector2(960, 540); // cx, cy (pixels)
}
