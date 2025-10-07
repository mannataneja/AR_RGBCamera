using UnityEngine;
public static class DiskCamConfig
{
    public static DiskCameraSettings Settings =>
        Resources.Load<DiskCameraSettings>("DiskCameraSettings");
}
