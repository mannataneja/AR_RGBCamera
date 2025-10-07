using UnityEngine;
using UnityEngine.XR.Management;

public class ForceLoader : MonoBehaviour
{
    void Awake()
    {
        var settings = XRGeneralSettings.Instance.Manager;
        var loader = ScriptableObject.CreateInstance<CameraLoader>();

        if (!settings.loaders.Contains(loader))
        {
            settings.loaders.Add(loader);
        }

        else
        {
            settings.InitializeLoaderSync();
            settings.StartSubsystems();

            Debug.Log("[ForceLoader] Mock camera loader started");
        }

    }
}
