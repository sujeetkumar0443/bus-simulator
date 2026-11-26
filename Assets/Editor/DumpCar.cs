#if UNITY_EDITOR
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class DumpCar
{
    static double next; static int left;
    [MenuItem("Tools/Bus/Sample Active Car 10s (Play mode)")]
    static void Sample()
    {
        left = 10; next = EditorApplication.timeSinceStartup + 6.0; // 6 s to press the pedal first
        EditorApplication.update += Tick;
    }
    static void Tick()
    {
        if (EditorApplication.timeSinceStartup < next) return;
        next = EditorApplication.timeSinceStartup + 1.0;
        var car = Object.FindObjectsOfType<RCC_CarControllerV3>().FirstOrDefault(c => c.gameObject.activeInHierarchy);
        if (car == null || --left < 0) { EditorApplication.update -= Tick; return; }
        var rb = car.GetComponent<Rigidbody>();
        var sb = new StringBuilder("[Sample] ");
        var scm = Object.FindObjectOfType<RCC_SceneManager>();
        sb.Append("activePlayer=" + (scm != null && scm.activePlayerVehicle != null ? scm.activePlayerVehicle.name + "(active=" + scm.activePlayerVehicle.gameObject.activeInHierarchy + ")" : "null") + " ctrl=" + RCC_Settings.Instance.controllerType + "/" + RCC_Settings.Instance.mobileController + " ");
        sb.Append("v=" + rb.velocity.magnitude.ToString("F2") + " pos=" + car.transform.position.ToString("F1"));
        foreach (string f in new[] { "gasInput", "brakeInput", "handbrakeInput", "speed", "currentGear", "engineRPM", "isSleeping" })
        {
            var fi = typeof(RCC_CarControllerV3).GetField(f, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fi != null) sb.Append(" " + f + "=" + fi.GetValue(car));
        }
        foreach (var wc in car.GetComponentsInChildren<RCC_WheelCollider>())
        {
            var col = wc.GetComponent<WheelCollider>();
            sb.Append(" | " + wc.name + " g=" + col.isGrounded + " m=" + col.motorTorque.ToString("F0") + " b=" + col.brakeTorque.ToString("F0") + " rpm=" + col.rpm.ToString("F0"));
        }
        Debug.Log(sb.ToString());
    }

    [MenuItem("Tools/Bus/Dump Active Car (Play mode)")]
    static void Dump()
    {
        var sb = new StringBuilder("[DumpCar]\n");
        var car = Object.FindObjectsOfType<RCC_CarControllerV3>().FirstOrDefault(c => c.gameObject.activeInHierarchy);
        if (car == null) { Debug.Log("[DumpCar] no active car"); return; }
        var rb = car.GetComponent<Rigidbody>();
        sb.AppendLine(car.name + " pos=" + car.transform.position.ToString("F2") + " vel=" + rb.velocity.ToString("F2") + " kinematic=" + rb.isKinematic + " sleeping=" + rb.IsSleeping() + " constraints=" + rb.constraints + " COMworld=" + rb.worldCenterOfMass.ToString("F2") + " mass=" + rb.mass);
        foreach (string f in new[] { "gasInput", "brakeInput", "handbrakeInput", "steerInput", "canControl", "engineRunning", "speed", "currentGear", "engineRPM", "fuelInput", "isSleeping", "overTurn", "handbrakeInput_V" })
        {
            var fi = typeof(RCC_CarControllerV3).GetField(f, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fi != null) sb.Append(f + "=" + fi.GetValue(car) + " ");
        }
        sb.AppendLine();
        foreach (var wc in car.GetComponentsInChildren<RCC_WheelCollider>(true))
        {
            var col = wc.GetComponent<WheelCollider>();
            WheelHit hit; bool g = col.GetGroundHit(out hit);
            sb.AppendLine("wheel " + wc.name + " active=" + wc.gameObject.activeInHierarchy + " pos=" + wc.transform.position.ToString("F2") + " r=" + col.radius.ToString("F2") + " susp=" + col.suspensionDistance.ToString("F2") + " grounded=" + col.isGrounded + (g ? " hitPoint=" + hit.point.ToString("F2") : "") + " motor=" + col.motorTorque.ToString("F0") + " brake=" + col.brakeTorque.ToString("F0") + " rpm=" + col.rpm.ToString("F0") + " model=" + (wc.wheelModel != null ? wc.wheelModel.name + "@" + wc.wheelModel.position.ToString("F2") : "null"));
        }
        foreach (var c in car.GetComponentsInChildren<Collider>(true))
            if (!(c is WheelCollider)) sb.AppendLine("collider " + c.name + " " + c.GetType().Name + " enabled=" + c.enabled + " trigger=" + c.isTrigger + " active=" + c.gameObject.activeInHierarchy + " bounds min=" + c.bounds.min.ToString("F2") + " max=" + c.bounds.max.ToString("F2"));
        Debug.Log(sb.ToString());
    }
}
#endif
