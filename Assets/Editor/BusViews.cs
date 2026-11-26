#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BusViews
{
    [MenuItem("Tools/Bus/Capture Views (Play mode)")]
    static void Run()
    {
        var car = Object.FindObjectsOfType<RCC_CarControllerV3>().FirstOrDefault(c => c.gameObject.activeInHierarchy);
        if (car == null) { Debug.Log("[Views] no active car"); return; }
        System.IO.Directory.CreateDirectory("BusTest");
        var go = new GameObject("_ViewCam"); var cam = go.AddComponent<Camera>();
        cam.fieldOfView = 40; cam.clearFlags = CameraClearFlags.Skybox; cam.nearClipPlane = 0.3f; cam.farClipPlane = 200f;
        var rt = new RenderTexture(1100, 620, 24);
        Vector3 c = car.transform.position + Vector3.up * 1.6f;
        Vector3 f = car.transform.forward, r = car.transform.right;
        var views = new (string n, Vector3 off)[] {
            ("front34", f * 13f - r * 9f + Vector3.up * 2.5f),
            ("rear34", -f * 13f + r * 9f + Vector3.up * 3f),
            ("side", -r * 15f + Vector3.up * 1.5f),
            ("front", f * 15f + Vector3.up * 1.5f + r * 1.5f) };
        foreach (var vw in views)
        {
            go.transform.position = c + vw.off; go.transform.LookAt(c);
            cam.targetTexture = rt; cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); tex.Apply();
            System.IO.File.WriteAllBytes("BusTest/view_" + car.name + "_" + vw.n + ".png", tex.EncodeToPNG());
            Object.Destroy(tex);
        }
        RenderTexture.active = null; cam.targetTexture = null; Object.Destroy(rt); Object.Destroy(go);
        Debug.Log("[Views] done " + car.name);
    }
}
#endif
