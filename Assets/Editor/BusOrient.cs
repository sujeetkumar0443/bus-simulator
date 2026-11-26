#if UNITY_EDITOR
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class BusOrient
{
    [MenuItem("Tools/Bus/Report Orientation (all buses)")]
    static void Run()
    {
        var sb = new StringBuilder("[Orient]\n");
        foreach (var car in Object.FindObjectsOfType<RCC_CarControllerV3>(true))
        {
            var t = car.transform;
            Bounds b = new Bounds(); bool has = false;
            foreach (var r in car.GetComponentsInChildren<Renderer>(true))
            {
                if (!r.enabled || r is ParticleSystemRenderer || r.GetComponentInParent<RCC_Light>() != null) continue;
                if (!r.transform.IsChildOf(t) || r.bounds.size.magnitude > 20f) continue;
                if (r.name.ToLower().Contains("passenger")) continue;
                Vector3 c = t.InverseTransformPoint(r.bounds.center); Vector3 e = r.bounds.extents;
                if (!has) { b = new Bounds(c, e * 2); has = true; } else b.Encapsulate(new Bounds(c, e * 2));
            }
            float fz = 0.5f * (t.InverseTransformPoint(car.FrontLeftWheelCollider.transform.position).z + t.InverseTransformPoint(car.FrontRightWheelCollider.transform.position).z);
            float rz = 0.5f * (t.InverseTransformPoint(car.RearLeftWheelCollider.transform.position).z + t.InverseTransformPoint(car.RearRightWheelCollider.transform.position).z);
            sb.AppendLine(car.name + " bodyZ=[" + b.min.z.ToString("F2") + "," + b.max.z.ToString("F2") + "] frontAxleZ=" + fz.ToString("F2") + " rearAxleZ=" + rz.ToString("F2") + " frontOverhang=" + (b.max.z - fz).ToString("F2") + " rearOverhang=" + (rz - b.min.z).ToString("F2"));
            foreach (var r in car.GetComponentsInChildren<Renderer>(true))
            {
                if (!r.enabled) continue;
                if (r.GetComponentInParent<RCC_Light>() != null) continue;
                var mf = r.GetComponent<MeshFilter>(); if (mf == null || mf.sharedMesh == null) continue;
                
                var ms = r.sharedMaterials;
                sb.AppendLine("   rend " + r.name + " mesh=" + mf.sharedMesh.name + " v=" + mf.sharedMesh.vertexCount + " subs=" + mf.sharedMesh.subMeshCount + " mats=" + string.Join(",", ms.Select(m => m == null ? "null" : m.name + "(" + m.shader.name.Replace("Universal Render Pipeline/", "URP/") + "," + (m.HasProperty("_Color") ? ColorUtility.ToHtmlStringRGB(m.color) : (m.HasProperty("_BaseColor") ? ColorUtility.ToHtmlStringRGB(m.GetColor("_BaseColor")) : "-")) + ")").ToArray()));
            }
            foreach (var l in car.GetComponentsInChildren<RCC_Light>(true))
            {
                var so = new SerializedObject(l); var lt = so.FindProperty("lightType");
                Vector3 p = t.InverseTransformPoint(l.transform.position);
                sb.AppendLine("   light " + (lt != null ? lt.enumDisplayNames[lt.enumValueIndex] : "?") + " local=" + p.ToString("F2") + " inBodyZ=" + (p.z >= b.min.z && p.z <= b.max.z) + " x/halfW=" + (p.x / Mathf.Max(0.01f, b.extents.x)).ToString("F2"));
            }
        }
        System.IO.Directory.CreateDirectory("BusTest"); System.IO.File.WriteAllText("BusTest/orient.txt", sb.ToString()); Debug.Log("[Orient] written");
    }
}
#endif
