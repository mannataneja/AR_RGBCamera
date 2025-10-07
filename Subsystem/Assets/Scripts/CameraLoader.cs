using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

public class CameraLoader : XRLoaderHelper
{
    static readonly List<XRCameraSubsystemDescriptor> s_Descriptors = new List<XRCameraSubsystemDescriptor>();

    public override bool Initialize()
    {
        SubsystemManager.GetSubsystemDescriptors(s_Descriptors);
        // Create the subsystem whose descriptor id matches "Simple-Camera"
        CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_Descriptors, "Simple-Camera");
        return true;
    }

    public override bool Start()
    {
        StartSubsystem<XRCameraSubsystem>();
        return true;
    }

    public override bool Stop()
    {
        StopSubsystem<XRCameraSubsystem>();
        return true;
    }

    public override bool Deinitialize()
    {
        DestroySubsystem<XRCameraSubsystem>();
        return true;
    }
}
