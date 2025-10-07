/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;

public sealed class DiskCameraSubsystem : XRCameraSubsystem
{
    ProviderImpl provider = new  ProviderImpl();

    class ProviderImpl : Provider
    {
        Texture2D[] _frames;
        RenderTexture _rt;
        Material _material;
        int _idx;
        double _tPrev;
        double _interval;
        int _mainTexId = Shader.PropertyToID("_MainTex");
        List<XRTextureDescriptor> _tmp = new List<XRTextureDescriptor>(1);

        public override void Start()
        {
            var s = DiskCamConfig.Settings;
            _interval = 1.0 / Math.Max(1e-3f, s.fps);

            // material (background shader)
            _material = new Material(Shader.Find("Unlit/DiskCameraBackground"));

            LoadFrames(s);
            SetupRT();
            BlitCurrent();
            _tPrev = Time.realtimeSinceStartupAsDouble;
        }

        public override void Stop() { }

        public override void Destroy()
        {
            if (_rt) UnityEngine.Object.Destroy(_rt);
            if (_material) UnityEngine.Object.Destroy(_material);
            if (_frames != null)
                foreach (var t in _frames) if (t) UnityEngine.Object.Destroy(t);
        }

        void LoadFrames(DiskCameraSettings s)
        {
            string folder = Path.Combine(Application.streamingAssetsPath, s.relativeFolder);
            var files = Directory.Exists(folder)
                ? Directory.GetFiles(folder)
                    .Where(p => p.EndsWith(".png") || p.EndsWith(".jpg"))
                    .OrderBy(p => p).ToArray()
                : Array.Empty<string>();

            _frames = files.Select(p =>
            {
                var data = File.ReadAllBytes(p);
                var t = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                t.LoadImage(data);
                t.Apply(false, true);
                t.wrapMode = TextureWrapMode.Clamp;
                t.filterMode = FilterMode.Bilinear;
                return t;
            }).ToArray();

            if (_frames.Length == 0)
                Debug.LogWarning($"DiskCameraSubsystem: No images found in {folder}");
        }

        void SetupRT()
        {
            if (_frames.Length == 0) return;
            _rt = new RenderTexture(_frames[0].width, _frames[0].height, 0, RenderTextureFormat.ARGB32);
            _rt.Create();
            _material.SetTexture(_mainTexId, _rt);
        }

        void BlitCurrent()
        {
            if (_rt && _frames != null && _frames.Length > 0)
                Graphics.Blit(_frames[_idx], _rt);
        }

        public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame frame)
        {
            if (_rt == null)
            {
                frame = default;
                return false;
            }

            var now = Time.realtimeSinceStartupAsDouble;
            if (now - _tPrev >= _interval)
            {
                _tPrev = now;
                _idx = (_idx + 1) % Math.Max(1, _frames.Length);
                BlitCurrent();
            }

            // compute projection
            float aspect = cameraParams.screenWidth / cameraParams.screenHeight;
            float fov = 60f;
            var proj = Matrix4x4.Perspective(fov, aspect, cameraParams.zNear, cameraParams.zFar);

            // create a minimal frame manually
            frame = default;
            frame.timestamp = (long)(now * 1_000_000_000L);
            frame.projectionMatrix = proj;
            frame.displayMatrix = Matrix4x4.identity;
            frame.trackingState = TrackingState.Tracking;

            return true;
        }


        public override bool TryGetIntrinsics(out XRCameraIntrinsics intrinsics)
        {
            var s = DiskCamConfig.Settings;
            var size = _rt ? new Vector2Int(_rt.width, _rt.height) : s.imageSize;
            intrinsics = new XRCameraIntrinsics(s.focal, s.principal, size);
            return true;
        }

        public override Material cameraMaterial => _material;

        public override NativeArray<XRTextureDescriptor> GetTextureDescriptors(
            XRTextureDescriptor defaultDescriptor, Allocator allocator)
        {
            if (_rt == null)
                return new NativeArray<XRTextureDescriptor>(0, allocator);

            _tmp.Clear();

#if UNITY_2020_2_OR_NEWER
            var desc = new XRTextureDescriptor(
                _rt.GetNativeTexturePtr(),
                _rt.width,
                _rt.height,
                _rt.mipmapCount,
                _rt.graphicsFormat,
                _mainTexId,
                _rt.depth,
                TextureDimension.Tex2D);
#else
            var desc = new XRTextureDescriptor(
                _rt.GetNativeTexturePtr(),
                _rt.width,
                _rt.height,
                _rt.mipmapCount,
                _rt.format,
                _mainTexId,
                _rt.depth,
                TextureDimension.Tex2D);
#endif
            _tmp.Add(desc);

            var arr = new NativeArray<XRTextureDescriptor>(_tmp.Count, allocator, NativeArrayOptions.UninitializedMemory);
            arr[0] = _tmp[0];
            return arr;
        }

        // Keep things simple
        public override bool permissionGranted => true;
        public override Feature currentCamera => Feature.None;
        public override bool autoFocusRequested => false;
        public override bool autoFocusEnabled => false;
    }

    // --------- Registration ----------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Register()
    {
        var cinfo = new XRCameraSubsystemCinfo
        {
            id = "DiskCamera",
            providerType = typeof(DiskCameraSubsystem.ProviderImpl),
            subsystemTypeOverride = typeof(DiskCameraSubsystem),

            // We don't provide CPU images
            supportsCameraImage = false,

            // Minimal flags for background rendering + timing
            supportsAverageBrightness = false,
            supportsAverageColorTemperature = false,
            supportsAverageIntensityInLumens = false,
            supportsColorCorrection = false,
            supportsFocusModes = false,
            supportsCameraGrain = false,
            supportsDisplayMatrix = true,
            supportsProjectionMatrix = true,
            supportsTimestamp = true,
            supportsCameraConfigurations = false,

            // Use our unlit background shader by name
            backgroundShaderName = "Unlit/DiskCameraBackground",
        };

        if (!XRCameraSubsystem.Register(cinfo))
            Debug.LogError("DiskCameraSubsystem: registration failed");
    }
}
*/