using UnityEngine;
using UnityEngine.XR.Management;

public class ForceLoader : MonoBehaviour
{
    void Awake()
    {
        var settings = XRGeneralSettings.Instance.Manager;
        var loader = ScriptableObject.CreateInstance<SimpleCameraLoader>();

        if (!settings.loaders.Contains(loader))
        {
            settings.loaders.Add(loader);
        }

        settings.InitializeLoaderSync();
        settings.StartSubsystems();

        Debug.Log("[ForceLoader] Mock camera loader started");
    }
}
