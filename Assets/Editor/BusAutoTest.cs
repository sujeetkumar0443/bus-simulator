#if UNITY_EDITOR
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class BusAutoTest
{
    static double t0; static int step; static RCC_CarControllerV3 car; static string dir = "BusTest";

    [MenuItem("Tools/Bus/Auto Test Active Car (Play mode)")]
    static void Run()
    {
        if (!Application.isPlaying) { Debug.Log("[AutoTest] enter Play mode first"); return; }
        System.IO.Directory.CreateDirectory(dir);
        car = Object.FindObjectsOfType<RCC_CarControllerV3>().FirstOrDefault(c => c.gameObject.activeInHierarchy);
        if (car == null) { Debug.Log("[AutoTest] no active car"); return; }
        t0 = EditorApplication.timeSinceStartup; step = 0;
        car.externalController = true; car.canControl = true; car.engineRunning = true; car.fuelInput = 1f;
        Debug.Log("[AutoTest] start " + car.name);
        EditorApplication.update += Tick;
    }

    static void Shot(string tag) { ScreenCapture.CaptureScreenshot(dir + "/" + car.name + "_" + tag + ".png"); }

    static void Wheels(string tag)
    {
        var sb = new StringBuilder("[AutoTest] " + car.name + " " + tag + " speed=" + car.speed.ToString("F1") + " steer=" + car.steerInput.ToString("F2"));
        foreach (var wc in car.GetComponentsInChildren<RCC_WheelCollider>())
        {
            var col = wc.GetComponent<WheelCollider>();
            sb.Append(" | " + wc.name + " g=" + col.isGrounded + " rpm=" + col.rpm.ToString("F0") + " steerAng=" + col.steerAngle.ToString("F0") + " model=" + (wc.wheelModel ? wc.wheelModel.name + " ly=" + wc.wheelModel.localEulerAngles.y.ToString("F0") : "null"));
        }
        Debug.Log(sb.ToString());
    }

    static void Tick()
    {
        if (!Application.isPlaying || car == null) { EditorApplication.update -= Tick; return; }
        double t = EditorApplication.timeSinceStartup - t0;
        var cam = Object.FindObjectOfType<RCC_Camera>();
        if (step == 0 && t > 1) { car.gasInput = 1f; step++; }
        if (step == 1) { car.gasInput = 1f; if (t > 7) { Shot("1_tps_drive"); Wheels("drive"); step++; } }
        if (step == 2 && t > 7.5) { car.steerInput = 0.8f; car.gasInput = 0.6f; step++; }
        if (step == 3) { car.steerInput = 0.8f; if (t > 9.5) { Shot("2_tps_steer"); Wheels("steer"); step++; } }
        if (step == 4 && t > 10) { car.steerInput = 0f; car.gasInput = 0f; car.brakeInput = 0.6f; car.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Left; car.lowBeamHeadLightsOn = true; step++; }
        if (step == 5 && t > 11.0) { Shot("3_lights_a"); step++; }
        if (step == 6 && t > 11.45) { Shot("4_lights_b"); step++; }
        if (step == 7 && t > 12) { car.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.All; if (cam) cam.ChangeCamera(RCC_Camera.CameraMode.FPS); step++; }
        if (step == 8 && t > 14) { Shot("5_cabin"); Debug.Log("[AutoTest] " + car.name + " camMode=" + (cam ? cam.cameraMode.ToString() : "null") + " camPos=" + (cam ? cam.transform.position.ToString("F2") : "") + " carPos=" + car.transform.position.ToString("F2")); step++; }
        if (step == 9 && t > 14.6) { if (cam) cam.ChangeCamera(RCC_Camera.CameraMode.TPS); step++; }
        if (step == 10 && t > 15.5) { Shot("6_tps_hazard"); step++; }
        if (step == 11 && t > 16.5) { Debug.Log("[AutoTest] done " + car.name); EditorApplication.update -= Tick; }
    }
}
#endif
