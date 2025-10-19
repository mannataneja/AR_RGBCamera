using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.ARSubsystems;

namespace DiskCamera
{
    public sealed class DiskCameraSubsystem : XRCameraSubsystem
    {
        const string SubsystemId = "DiskCamera-Camera";

        class Provider : XRCameraSubsystem.Provider
        {
            List<string> frames = new();
            Texture2D currentFrame;
            Material _cameraMaterial;
            double nextFrameTime;
            double frameInterval = 1.0 / 30.0;
            int index = 0;
            long timestampNs = 0;
            DiskCameraSettings settings; // ✅ new settings reference

            public override Material cameraMaterial
                => _cameraMaterial ??= new Material(Shader.Find("Unlit/Texture"));

            public override bool permissionGranted => true;

            public override void Start()
            {
                // ✅ Load DiskCameraSettings from Resources (new workflow)
                settings = Resources.Load<DiskCameraSettings>("DiskCameraSettings");
                if (settings == null)
                {
                    Debug.LogError("DiskCameraSettings.asset not found in Resources/");
                    return;
                }

                frameInterval = 1.0 / Math.Max(1, settings.targetFPS);

                // Handle absolute or StreamingAssets-relative folder
                string folder = settings.imageFolder;
                if (!Path.IsPathRooted(folder))
                    folder = Path.Combine(Application.streamingAssetsPath, folder);

                if (!Directory.Exists(folder))
                {
                    Debug.LogError($"DiskCamera: folder not found: {folder}");
                    return;
                }

                frames.Clear();
                foreach (var file in Directory.GetFiles(folder))
                    if (file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                        frames.Add(file);

                frames.Sort(StringComparer.Ordinal);
                if (frames.Count == 0)
                {
                    Debug.LogError($"DiskCamera: no image files found in {folder}");
                    return;
                }

                LoadFrame(0);
                nextFrameTime = Time.realtimeSinceStartupAsDouble + frameInterval;
            }

            void LoadFrame(int i)
            {
                if (i < 0 || i >= frames.Count) return;
                byte[] bytes = File.ReadAllBytes(frames[i]);
                currentFrame ??= new Texture2D(2, 2, TextureFormat.RGBA32, false);
                currentFrame.LoadImage(bytes);
                cameraMaterial.mainTexture = currentFrame;
                timestampNs += (long)(frameInterval * 1_000_000_000L);
            }

            public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame frame)
            {
                if (Time.realtimeSinceStartupAsDouble >= nextFrameTime)
                {
                    index++;
                    if (index >= frames.Count)
                        index = settings.loopSequence ? 0 : frames.Count - 1;
                    LoadFrame(index);
                    nextFrameTime = Time.realtimeSinceStartupAsDouble + frameInterval;
                }

                frame = default;
                return true;
            }

            public override NativeArray<XRTextureDescriptor> GetTextureDescriptors(
                XRTextureDescriptor defaultDescriptor, Allocator allocator)
            {
                if (currentFrame == null)
                    return new NativeArray<XRTextureDescriptor>(0, allocator);

                var desc = new XRTextureDescriptor(
                    currentFrame.GetNativeTexturePtr(),
                    currentFrame.width,
                    currentFrame.height,
                    currentFrame.mipmapCount,
                    currentFrame.format,
                    0,
                    0,
                    TextureDimension.Tex2D);

                var array = new NativeArray<XRTextureDescriptor>(1, allocator);
                array[0] = desc;
                return array;
            }

            public override void Stop()
            {
                if (currentFrame != null)
                    UnityEngine.Object.Destroy(currentFrame);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRCameraSubsystem.Register(new XRCameraSubsystemCinfo
            {
                id = SubsystemId,
                providerType = typeof(Provider),
                subsystemTypeOverride = typeof(DiskCameraSubsystem),
                supportsDisplayMatrix = true,
                supportsTimestamp = true,
                supportsCameraConfigurations = true
            });
        }
    }
}
