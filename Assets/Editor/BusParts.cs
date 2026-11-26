#if UNITY_EDITOR
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class BusParts
{
    [MenuItem("Tools/Bus/Dump Car Children (all buses)")]
    static void Run()
    {
        var sb = new StringBuilder("[Parts]\n");
        foreach (var car in Object.FindObjectsOfType<RCC_CarControllerV3>(true))
        {
            sb.AppendLine("== " + car.name);
            foreach (var t in car.GetComponentsInChildren<Transform>(true))
            {
                if (t == car.transform) continue;
                bool inNew = false; for (var p = t; p != null && p != car.transform; p = p.parent) if (p.name.StartsWith("NewBus_")) inNew = true;
                if (inNew) continue;
                var comps = t.GetComponents<Component>().Where(c => c != null && !(c is Transform)).Select(c => c.GetType().Name).ToArray();
                Vector3 lp = car.transform.InverseTransformPoint(t.position);
                sb.AppendLine("  " + (t.gameObject.activeSelf ? "ON " : "off") + " " + GetPath(t, car.transform) + " pos=" + lp.ToString("F2") + " [" + string.Join(",", comps) + "]");
            }
        }
        System.IO.Directory.CreateDirectory("BusTest");
        System.IO.File.WriteAllText("BusTest/parts.txt", sb.ToString());
        Debug.Log("[Parts] written");
    }
    static string GetPath(Transform t, Transform root) { string s = t.name; for (var p = t.parent; p != null && p != root; p = p.parent) s = p.name + "/" + s; return s; }
}
#endif
