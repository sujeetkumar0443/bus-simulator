#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

// Tools > Bus > Measure Cabin : logs driver area positions of the new bus and the old camera objects.
public static class MeasureCabin
{
    [MenuItem("Tools/Bus/Measure Cabin")]
    public static void Run()
    {
        StringBuilder sb = new StringBuilder("[MeasureCabin]\n");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/mb_371R_pbr/prefab/mb_371R_pbr.prefab");
        GameObject tmp = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        foreach (Renderer r in tmp.GetComponentsInChildren<Renderer>(true))
        {
            MeshFilter mf = r.GetComponent<MeshFilter>(); if (mf == null || mf.sharedMesh == null) continue;
            string n = r.name.ToLower();
            if (!(n.Contains("steering") || n.Contains("driver") || n.Contains("velocity") || n.StartsWith("panel") || n.Contains("interior") || n.Contains("glasses") || n.Contains("floor") || n.Contains("body") || n.Contains("mirror") || n.Contains("wiper"))) continue;
            Matrix4x4 mat = tmp.transform.worldToLocalMatrix * r.transform.localToWorldMatrix;
            Vector3[] v = null; try { v = mf.sharedMesh.vertices; } catch { }
            if (v == null || v.Length == 0) { sb.AppendLine(r.name + " unreadable"); continue; }
            Bounds b = new Bounds(mat.MultiplyPoint3x4(v[0]), Vector3.zero);
            foreach (Vector3 p in v) b.Encapsulate(mat.MultiplyPoint3x4(p));
            sb.AppendLine(r.name + " min=" + b.min.ToString("F2") + " max=" + b.max.ToString("F2") + " center=" + b.center.ToString("F2"));
            if (n.Contains("glasses"))
            {   // windshield = glass vertices near the front (z>3.5)
                bool any = false; Bounds g = new Bounds();
                foreach (Vector3 p in v) { Vector3 q = mat.MultiplyPoint3x4(p); if (q.z < 3.5f) continue; if (!any) { g = new Bounds(q, Vector3.zero); any = true; } else g.Encapsulate(q); }
                if (any) sb.AppendLine("   windshield(z>3.5) min=" + g.min.ToString("F2") + " max=" + g.max.ToString("F2"));
            }
        }
        Object.DestroyImmediate(tmp);
        GameObject vehicles = GameObject.Find("Vehicles");
        if (vehicles != null && vehicles.transform.childCount > 0)
        {
            Transform bus = vehicles.transform.GetChild(0);
            Transform chassis = bus.Find("Chassis");
            sb.AppendLine("Chassis children (localPos / localEuler):");
            if (chassis != null)
                foreach (Transform t in chassis)
                    sb.AppendLine("  " + t.name + " " + t.localPosition.ToString("F2") + " / " + t.localEulerAngles.ToString("F0") + " comps=" + string.Join(",", System.Array.ConvertAll(t.GetComponents<Component>(), c => c == null ? "null" : c.GetType().Name)));
            foreach (Camera c in bus.GetComponentsInChildren<Camera>(true))
                sb.AppendLine("  CAMERA " + c.name + " localPosInChassis=" + (chassis ? chassis.InverseTransformPoint(c.transform.position).ToString("F2") : "?") + " fov=" + c.fieldOfView + " near=" + c.nearClipPlane);
        }
        Debug.Log(sb.ToString());
    }
}
#endif
