#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

// Tools > Bus > Measure Lights : logs where the new bus light meshes are and where the old RCC lights sit.
public static class MeasureLights
{
    [MenuItem("Tools/Bus/Measure Lights")]
    public static void Run()
    {
        StringBuilder sb = new StringBuilder("[MeasureLights]\n");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/mb_371R_pbr/prefab/mb_371R_pbr.prefab");
        GameObject tmp = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Bounds all = new Bounds(); bool has = false;
        foreach (Renderer r in tmp.GetComponentsInChildren<Renderer>(true))
        {
            MeshFilter mf = r.GetComponent<MeshFilter>(); if (mf == null || mf.sharedMesh == null) continue;
            string n = r.name.ToLower();
            Mesh m = mf.sharedMesh; Matrix4x4 mat = tmp.transform.worldToLocalMatrix * r.transform.localToWorldMatrix;
            Bounds b = m.bounds; Vector3[] v = null;
            try { v = m.vertices; } catch { }
            if (!n.StartsWith("wheel") && !has) { all = new Bounds(mat.MultiplyPoint3x4(b.center), Vector3.zero); has = true; }
            if (!n.StartsWith("wheel")) for (int i = 0; i < 8; i++) all.Encapsulate(mat.MultiplyPoint3x4(b.center + Vector3.Scale(b.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1))));
            if (n.Contains("light") || n.Contains("arrow") || n.Contains("lamp") || n.Contains("plate") || n.Contains("package"))
            {
                sb.Append(r.name + " verts=" + (v != null ? v.Length.ToString() : "unreadable"));
                if (v != null && v.Length > 0)
                {
                    foreach (int side in new[] { -1, 1 })
                    {
                        bool any = false; Bounds sbnd = new Bounds();
                        foreach (Vector3 p in v) { Vector3 q = mat.MultiplyPoint3x4(p); if (q.x * side <= 0.05f) continue; if (!any) { sbnd = new Bounds(q, Vector3.zero); any = true; } else sbnd.Encapsulate(q); }
                        if (any) sb.Append("\n   side " + (side < 0 ? "L" : "R") + " center=" + sbnd.center.ToString("F2") + " size=" + sbnd.size.ToString("F2"));
                    }
                }
                sb.AppendLine();
            }
        }
        sb.AppendLine("NEW BUS (prefab space, unscaled) min=" + all.min.ToString("F2") + " max=" + all.max.ToString("F2"));
        Object.DestroyImmediate(tmp);
        GameObject vehicles = GameObject.Find("Vehicles");
        if (vehicles != null && vehicles.transform.childCount > 0)
        {
            Transform bus = vehicles.transform.GetChild(0);
            Transform chassis = bus.Find("Chassis");
            Transform lights = chassis != null ? chassis.Find("Lights") : null;
            sb.AppendLine("Bus0 Chassis localScale=" + (chassis ? chassis.localScale.ToString("F2") : "?") + " Lights parent localPos=" + (lights ? lights.localPosition.ToString("F2") : "?"));
            if (lights != null)
                foreach (Transform t in lights) sb.AppendLine("  old light " + t.name + " localPos(in Chassis)=" + chassis.InverseTransformPoint(t.position).ToString("F2"));
        }
        Debug.Log(sb.ToString());
    }
}
#endif
