using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraBackground))]
public class DiskCameraBackgroundFeed : MonoBehaviour
{
    public string relativeFolder = "DiskCam"; // under StreamingAssets
    public float fps = 30f;
    public bool loop = true;
    public Material overrideMaterial; // assign the shader material

    Texture2D[] _frames;
    RenderTexture _rt;
    ARCameraBackground _bg;
    int _idx;
    float _accum;
    float _interval;

    void Awake()
    {
        _bg = GetComponent<ARCameraBackground>();
        _interval = 1f / Mathf.Max(1e-3f, fps);
        if (overrideMaterial != null) _bg.customMaterial = overrideMaterial;
        LoadFrames();
        SetupRT();
        BlitCurrent();
    }

    void LoadFrames()
    {
        var root = Path.Combine(Application.streamingAssetsPath, relativeFolder);
        var files = Directory.Exists(root)
            ? Directory.GetFiles(root).Where(p => p.EndsWith(".png") || p.EndsWith(".jpg"))
              .OrderBy(p => p).ToArray()
            : new string[0];

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
            Debug.LogWarning($"No images found in {root}");
    }

    void SetupRT()
    {
        if (_frames.Length == 0) return;
        _rt = new RenderTexture(_frames[0].width, _frames[0].height, 0, RenderTextureFormat.ARGB32);
        _rt.Create();
        _bg.customMaterial.SetTexture("_MainTex", _rt);
    }

    void BlitCurrent()
    {
        if (_rt && _frames != null && _frames.Length > 0)
            Graphics.Blit(_frames[_idx], _rt);
    }

    void Update()
    {
        if (_frames == null || _frames.Length == 0) return;

        _accum += Time.deltaTime;
        if (_accum >= _interval)
        {
            _accum -= _interval;
            _idx++;
            if (_idx >= _frames.Length)
            {
                if (loop) _idx = 0;
                else _idx = _frames.Length - 1;
            }
            BlitCurrent();
        }
    }
}
