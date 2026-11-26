#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

// Tools > Bus > Measure Buses : logs sizes of the new bus prefab and every bus under Vehicles.
public static class MeasureBus
{
    static Bounds MeshBounds(Renderer r)
    {
        MeshFilter mf = r.GetComponent<MeshFilter>();
        Mesh m = mf != null ? mf.sharedMesh : (r is SkinnedMeshRenderer ? ((SkinnedMeshRenderer)r).sharedMesh : null);
        if (m == null) return new Bounds(r.transform.position, Vector3.zero);
        Bounds lb = m.bounds; Matrix4x4 mat = r.transform.localToWorldMatrix;
        Bounds wb = new Bounds(mat.MultiplyPoint3x4(lb.center), Vector3.zero);
        for (int i = 0; i < 8; i++)
            wb.Encapsulate(mat.MultiplyPoint3x4(lb.center + Vector3.Scale(lb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1))));
        return wb;
    }

    static void Report(StringBuilder sb, string label, Transform root, bool onlyEnabled)
    {
        bool has = false; Bounds b = new Bounds();
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true))
        {
            if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)) continue;
            if (onlyEnabled && !r.enabled) continue;
            Bounds rb = MeshBounds(r);
            if (!has) { b = rb; has = true; } else b.Encapsulate(rb);
        }
        sb.AppendLine(label + ": " + (has ? "size(x,y,z)=" + b.size.ToString("F2") + " center=" + root.InverseTransformPoint(b.center).ToString("F2") : "no meshes"));
    }

    [MenuItem("Tools/Bus/Measure Buses")]
    public static void Measure()
    {
        StringBuilder sb = new StringBuilder("[MeasureBus]\n");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/mb_371R_pbr/prefab/mb_371R_pbr.prefab");
        if (prefab != null)
        {
            GameObject tmp = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Report(sb, "NEW prefab (all)", tmp.transform, false);
            foreach (Renderer r in tmp.GetComponentsInChildren<Renderer>(true))
                if (r.name.ToLower().StartsWith("wheel_") || r.name.ToLower().StartsWith("wheelcc"))
                { Bounds b = MeshBounds(r); sb.AppendLine("  " + r.name + " center=" + b.center.ToString("F2") + " size=" + b.size.ToString("F2")); }
            Object.DestroyImmediate(tmp);
        }
        GameObject vehicles = GameObject.Find("Vehicles");
        if (vehicles != null)
            for (int i = 0; i < vehicles.transform.childCount; i++)
            {
                Transform v = vehicles.transform.GetChild(i);
                sb.AppendLine("Vehicle " + i + " " + v.name);
                Transform old = v.Find("Chassis/Bus5"); if (old == null) old = v;
                Report(sb, "  visible old/other meshes", old, false);
                RCC_CarControllerV3 car = v.GetComponent<RCC_CarControllerV3>();
                if (car != null)
                {
                    foreach (RCC_WheelCollider wc in v.GetComponentsInChildren<RCC_WheelCollider>(true))
                    {
                        WheelCollider c = wc.GetComponent<WheelCollider>();
                        sb.AppendLine("  wheelCollider " + wc.name + " localPos=" + v.InverseTransformPoint(wc.transform.position).ToString("F2") + " radius=" + (c ? c.radius.ToString("F2") : "?") + " susp=" + (c ? c.suspensionDistance.ToString("F2") : "?") + " model=" + (wc.wheelModel ? wc.wheelModel.name : "NULL"));
                    }
                    Collider[] cols = v.GetComponentsInChildren<Collider>(true);
                    foreach (Collider c in cols) if (c is BoxCollider) { BoxCollider bc = (BoxCollider)c; sb.AppendLine("  boxCollider " + c.name + " size=" + bc.size.ToString("F2") + " center=" + bc.center.ToString("F2") + " lossyScale=" + c.transform.lossyScale.ToString("F2")); }
                }
            }
        Debug.Log(sb.ToString());
    }
}
#endif
