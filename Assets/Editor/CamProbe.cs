#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CamProbe
{
    static double t0; static int step; static RCC_CarControllerV3 car; static Transform hood; static Vector3 p0;
    static readonly Vector3[] offs = { new Vector3(-0.5f,0,0.8f), new Vector3(-0.5f,0.3f,1.3f), new Vector3(-0.9f,0,0.5f), new Vector3(0.5f,0,0.8f), new Vector3(0,0.8f,0.5f), new Vector3(-0.5f,-0.4f,0.5f), new Vector3(-0.5f,0,1.5f), new Vector3(0,0,-1.5f) };

    [MenuItem("Tools/Bus/Cam Probe (Play mode)")]
    static void Run()
    {
        car = Object.FindObjectsOfType<RCC_CarControllerV3>().FirstOrDefault(c => c.gameObject.activeInHierarchy);
        if (car == null) return;
        hood = car.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name.StartsWith("HoodCamera"));
        if (hood == null) { Debug.Log("[CamProbe] no hood"); return; }
        p0 = hood.position; step = 0; t0 = EditorApplication.timeSinceStartup;
        var cam = Object.FindObjectOfType<RCC_Camera>(); if (cam) cam.ChangeCamera(RCC_Camera.CameraMode.FPS);
        System.IO.Directory.CreateDirectory("BusTest");
        Debug.Log("[CamProbe] start hood=" + hood.name + " p0=" + p0.ToString("F2"));
        EditorApplication.update += Tick;
    }
    static int applied = -1, shot = -1;
    static void Tick()
    {
        if (!Application.isPlaying || hood == null) { EditorApplication.update -= Tick; applied = shot = -1; return; }
        double t = EditorApplication.timeSinceStartup - t0;
        int k = (int)(t / 1.5);
        if (k >= offs.Length) { Debug.Log("[CamProbe] done"); EditorApplication.update -= Tick; applied = shot = -1; return; }
        if (applied < k) { hood.position = p0 + car.transform.right * offs[k].x + car.transform.up * offs[k].y + car.transform.forward * offs[k].z; applied = k; }
        if (shot < k && t - k * 1.5 > 0.7) { ScreenCapture.CaptureScreenshot("BusTest/probe_" + k + ".png"); shot = k; Debug.Log("[CamProbe] shot " + k + " off=" + offs[k].ToString("F1")); }
    }
}
#endif
